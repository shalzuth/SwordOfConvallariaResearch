using Google.Protobuf;
using Protos;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;

namespace NetworkProtocol;
public class Game : IDisposable
{
    static string UserCenterHost = "example.com";
    static string GateHost = "example.com";
    TcpClient client;
    NetworkStream stream;
    CancellationTokenSource cts;
    int TimeStart = Environment.TickCount;
    string AccessToken;
    string MacKey;
    public Func<Task> Recv;
    public static ConcurrentDictionary<string, Game> Games = new ConcurrentDictionary<string, Game>();
    public static async Task<Game> InitOrGet(string accessToken, string macKey)
    {
        var exists = Games.TryGetValue(accessToken + macKey, out Game game);
        if (exists) return game;
        game = new Game();
        Games.TryAdd(accessToken + macKey, game);
        await game.Login(accessToken, macKey);
        return game;
    }
    public async Task Login(string accessToken, string macKey)
    {
        AccessToken = accessToken;
        MacKey = macKey;
        client = new TcpClient(GateHost, 20000);
        client.ReceiveBufferSize = 0x8000;
        stream = client.GetStream();
        cts = new CancellationTokenSource();
        ReceiveLoopAsync(cts.Token);

        var loginParams = new { access_token = accessToken, mac_key = macKey, device = "pc", app_id = Auth.AppId };
        var gameTokenResp = await Client.client.PostAsync($"https://{UserCenterHost}/xd_login", new StringContent(loginParams.ToFormUrlEncoded(), Encoding.UTF8, "application/x-www-form-urlencoded"));
        var gameTokenNode = await gameTokenResp.Content.ReadFromJsonAsync<JsonNode>();
        var gameToken = gameTokenNode["token"].ToString();

        await SendPacket(new CSLogin { Token = gameToken, Device = "pc", Os = "Windows" });
    }
    async Task SCLogin(SCLogin login)
    {
        await SendPacket(new CSOnlinePlayerInfo { });
        _ = Task.Run(() => PingLoop(cts.Token));
    }
    async Task SCOnlinePlayerInfo(SCOnlinePlayerInfo obj)
    {
        Console.WriteLine("Player Nickname : " + obj.Player.Nickname);
    }
    async Task PingLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await SendPacket(new CSTCPPing { Timestamp = (uint)(Environment.TickCount - TimeStart) });
                await Task.Delay(15000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ping loop: {ex.Message}");
        }
        finally
        {
            Dispose();
        }

    }
    async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[0x8000];
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                if (bytesRead == 0) break;
                var payload = new byte[bytesRead];
                Array.Copy(buffer, payload, bytesRead);
                await ProcessDataAsync(payload);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in receive loop: {ex.Message}");
        }
        finally
        {
            Dispose();
        }
    }
    byte[] packetsBuffer = new byte[0];
    async Task ProcessDataAsync(byte[] buffer)
    {
        var origSize = packetsBuffer.Length;
        Array.Resize(ref packetsBuffer, packetsBuffer.Length + buffer.Length);
        buffer.CopyTo(packetsBuffer, origSize);
        var packetSize = BitConverter.ToUInt16(packetsBuffer);
        while (packetSize <= packetsBuffer.Length)
        {
            var packetBytes = new byte[packetSize];
            Array.Copy(packetsBuffer, 0, packetBytes, 0, packetSize);
            var packet = Packet.ParseS2C(packetBytes);
            Console.WriteLine("S2C " + packet.GetType() + " : " + packet);
            var handler = GetType().GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).FirstOrDefault(m => packet.GetType().ToString() == "Protos." + m.Name);
            if (handler != null)
                handler.Invoke(this, new object[] { packet });
            packetsBuffer = packetsBuffer.AsSpan().Slice(packetSize).ToArray();
            Recv?.Invoke();
            if (packetsBuffer.Length == 0) break;
            packetSize = BitConverter.ToUInt16(packetsBuffer);
        }
    }
    int counter = 1;
    public async Task SendPacket(IMessage packet)
    {
        Console.WriteLine("C2S" + packet.GetType() + " : " + packet);
        var payload = Packet.ToArray(packet, counter++);
        await stream.WriteAsync(payload, 0, payload.Length);
    }
    public void Dispose()
    {
        cts.Cancel();
        stream?.Dispose();
        client?.Dispose();
        Games.TryRemove(AccessToken + MacKey, out _);
    }
}