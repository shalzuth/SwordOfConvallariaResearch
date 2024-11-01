using System.Text;
using UnluacNET;

namespace Dumper
{
    internal class Lua
    {
        internal static byte[] Decrypt(byte[] data)
        {
            data[0] = (byte)(data[0] ^ 0x35);
            var key = new byte[] { 0x17, 0xf1, 0xc3, 0x55, 0x78, 0x64, 0x39, 0x40, 0x42, 0x77, 0x59, 0x12, 0x33, 0xcb, 0x7b, 0xb9, 0x35 };
            for (var i = 1; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ key[(i - 1) % key.Length]);
            return data;
        }
        internal static byte[] LuaZReadDecrypt(byte[] buffer)
        {
            for (var i = 2ul; i < (ulong)buffer.Length; i++)
            {
                var key = 0x20210507 * i;
                var idx = i % 3;
                if (idx == 1)
                    buffer[i] = (byte)(((byte)((key >> 16) & 0xFF) - i) ^ buffer[i]);
                else if (idx == 2)
                    buffer[i] = (byte)(((key >> 21) | i) ^ buffer[i]);
                else
                    buffer[i] = (byte)(((key >> 28) + (key & 1) + i) ^ buffer[i]);
            }
            buffer[0] = 0x1b;
            buffer[1] = (byte)'L';
            buffer[2] = (byte)'u';
            buffer[3] = (byte)'a';
            buffer[4] = 0x51;
            return buffer;
        }
        internal static void DecryptDecompile(string luac)
        {
            if (File.Exists(luac.Replace("luac", "lua"))) return;
            var luacBytes = LuaZReadDecrypt(Decrypt(File.ReadAllBytes(luac)));
            using (var ms = new MemoryStream(luacBytes))
            {
                var header = new BHeader(ms);
                var function = header.Function.Parse(ms, header);
                var d = new Decompiler(function);
                d.Decompile();
                Directory.CreateDirectory(Path.GetDirectoryName(luac.Replace("luac\\", "lua\\")));
                using (var writer = new StreamWriter(luac.Replace("luac","lua"), false, new UTF8Encoding(false)))
                {
                    d.Print(new Output(writer));
                    writer.Flush();
                }
            }
        }
    }
}
