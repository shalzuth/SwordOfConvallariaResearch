using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnluacNET.IO;

namespace UnluacNET
{
    public class LStringType : BObjectType<LString>
    {
        internal static byte[] StringDecrypt(byte[] buffer)
        {
            for (var i = 0ul; i < (ulong)buffer.Length; i++)
            {
                buffer[i] = (byte)(((buffer[i] + i % 3) ^ 0xee) - i);
            }
            return buffer;
        }
        public override LString Parse(Stream stream, BHeader header)
        {
            var sizeT = header.SizeT.Parse(stream, header);

            var sb = new List<byte>();
            sizeT.m_big -= 10;
            sizeT.Iterate(() => {
                sb.Add((byte)stream.ReadByte());
            });
            var str = Encoding.UTF8.GetString(sb.ToArray());
            if (str.StartsWith("A"))
            {
                str = str.Trim('\0');
                var buffer = new Span<byte>(new byte[str.Length * 4]);
                var c = Convert.TryFromBase64String(str.Substring(1), buffer, out int numBytes);
                if (c)
                    str = Encoding.UTF8.GetString(StringDecrypt(buffer.Slice(0, numBytes).ToArray()));
            }
            if (header.Debug)
                Console.WriteLine("-- parsed <string> \"" + str + "\"");

            return new LString(sizeT, str);
        }
    }
}
