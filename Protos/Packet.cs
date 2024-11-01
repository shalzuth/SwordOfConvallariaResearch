using Google.Protobuf;
using System.Reflection;
using System.Text;

namespace Protos
{
    public class Packet
    {
        internal static byte[] Decrypt(byte[] data, byte[] key)
        {
            if (key == null) return data;
            var xx = Encoding.UTF8.GetString(key);
            var offset = 10;
            for (var i = 0; i < data.Length - offset; i++)
                data[i + offset] = (byte)(data[i + offset] ^ key[i % key.Length]);
            return data;
        }
        public static void DumpUnknownFields(IMessage message)
        {
            if (message == null)
            {
                Console.WriteLine("Message is null.");
                return;
            }
            var type = message.GetType();
            var unknownFieldsField = type.GetField("_unknownFields", BindingFlags.NonPublic | BindingFlags.Instance);
            if (unknownFieldsField == null)
            {
                //Console.WriteLine("Field '_unknownFields' not found.");
                return;
            }
            var unknownFields = unknownFieldsField.GetValue(message) as UnknownFieldSet;
            if (unknownFields == null)
            {
                //Console.WriteLine("No unknown fields.");
                return;
            }
            /*Console.WriteLine("Unknown Fields:");
            foreach (var field in unknownFields)
            {
                Console.WriteLine($"Field Number: {field.Key}, Value: {field.Value}");
            }*/
        }
        public static IMessage ParseC2S(byte[] payload, byte[] xor = null)
        {
            var opcode = (CtoSPacketMessageIds)BitConverter.ToUInt16(payload, 4);
            if (xor != null && payload.Length > 10)
            {
                payload = Decrypt(payload, xor);
                //payload = payload.Take(10).Concat(payload.Skip(10).Reverse().ToArray()).ToArray();
                //payload = IronSnappy.Snappy.Decode(payload.Skip(10).ToArray());
            }
            return Parse(opcode.ToString(), payload);
        }
        public static IMessage ParseS2C(byte[] payload)
        {
            var opcode = (StoCPacketMessageIds)BitConverter.ToUInt16(payload, 4);
            return Parse(opcode.ToString(), payload);
        }
        public static IMessage Parse(string opcode, byte[] payload)
        {
            var protoType = Type.GetType("Protos." + opcode);
            var obj = (IMessage)Activator.CreateInstance(protoType);
            obj.MergeFrom(payload, 10, payload.Length - 10);
#if DEBUG
            DumpUnknownFields(obj);
#endif
            return obj;
        }
        public static byte[] ToArray(IMessage packet, int counter = 0)
        {
            var payload = packet.ToByteArray();
            var packetBytes = new byte[payload.Length + 10];
            var length = BitConverter.GetBytes(packetBytes.Length);
            Array.Copy(length, 0, packetBytes, 0, 4);
            var opcode = BitConverter.GetBytes((ushort)Enum.Parse<CtoSPacketMessageIds>(packet.GetType().Name));
            Array.Copy(opcode, 0, packetBytes, 4, 2);
            var count = BitConverter.GetBytes(counter);
            Array.Copy(count, 0, packetBytes, 6, 4);
            Array.Copy(payload, 0, packetBytes, 10, payload.Length);
            return packetBytes;
        }
    }
}
