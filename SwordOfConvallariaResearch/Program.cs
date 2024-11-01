using NetworkProtocol;
using Protos;

var guestAccount = await Auth.NewGuest();
var client = await Game.InitOrGet(guestAccount.Item1, guestAccount.Item2);
await Task.Delay(250); // should wait for login response instead
await client.SendPacket(new CSOnlinePlayerSetAvatar { AvatarId = 2 }); // set to female
await client.SendPacket(new CSOnlineGacha { Id = 2, Times = 10, Consume = new DBConsume { Type = 114, Param0 = 1, Param1 = 10 } }); // test gacha summons
Console.WriteLine("fin");
Console.ReadLine();