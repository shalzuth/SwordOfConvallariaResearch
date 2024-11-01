using System.Security.Cryptography;
using System.Text;

namespace NetworkProtocol;

public class Auth
{
    static readonly string Host = "example.com";
    static readonly string ClientId = "abcd";
    internal static readonly int AppId = 1234;
    static readonly string DeviceId = string.Concat(Enumerable.Range(0, 20).Select(_ => RandomNumberGenerator.GetInt32(0, 256).ToString("x2")));
    static readonly string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    static readonly string State = string.Concat(Enumerable.Range(0, 8).Select(_ => Characters[RandomNumberGenerator.GetInt32(0, Characters.Length)]));
    static object param = new
    {
        clientId = ClientId,
        appId = AppId,
        region = "Global",
        did = DeviceId,
        sdkLang = "en_US",
        time = CurrentDateStamp(), // note, this should be updated outside of ctor
        chn = "PC",
        locationInfoType = "ip",
        mem = "8GB",
        res = "1515_852",
        mod = "System Product Name (ASUS)",
        sdkVer = "6.20.2",
        pkgName = "",
        brand = "System Product Name (ASUS)",
        os = "Windows 11  (10.0.22631) 64bit",
        pt = "Windows",
        appVer = "1.12.2.17531",
        appVerCode = "1.12.2.17531",
        cpu = "12th Gen Intel(R) Core(TM) i9-12900K",
        lang = "en-US",
        loc = "US"
    };
    public static async Task<(string, string)> NewGuest()
    {
        var union = new { token = State, type = 0 };
        var accessToken = await Client.Post($"https://{Host}/api/login/v1/union?{param.ToFormUrlEncoded()}", union);
        return (accessToken["data"]["kid"].ToString(), accessToken["data"]["macKey"].ToString());
    }
    public static async Task<(string, string)> GetLoginParams(string googleToken, string state)
    {
        var union = new { token = googleToken, type = 3, grantType = "authorization_code", state = state };
        var accessToken = await Client.Post($"https://{Host}/api/login/v1/signin?{param.ToFormUrlEncoded()}", union);
        return (accessToken["data"]["kid"].ToString(), accessToken["data"]["macKey"].ToString());
    }
    static string CurrentDateStamp()
    {
        return (((long)((TimeSpan)(DateTime.UtcNow - new DateTime(1970, 1, 1))).TotalMilliseconds) / 1000).ToString();
    }
    static string Sign(string url, string macKey, string ts, string nonce)
    {
        var keyBytes = Encoding.UTF8.GetBytes(macKey);
        var dataBytes = Encoding.UTF8.GetBytes($@"{ts}
{nonce}
GET
{url}
{Host}
443
".Replace("\r\n", "\n"));
        using (var hmac = new HMACSHA1(keyBytes))
        {
            byte[] hashBytes = hmac.ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
