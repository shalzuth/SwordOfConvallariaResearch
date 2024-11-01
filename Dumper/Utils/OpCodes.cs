using System.Text;

namespace Dumper
{
    internal class OpCodes
    {
        internal static void Parse()
        {
            var modes = new List<string> { "CtoS", "StoC" };
            var opcodes = new Dictionary<string, Dictionary<int, string>> { };
            foreach (var mode in modes)
            {
                opcodes[mode] = new Dictionary<int, string> { };
                foreach (var c2s in Directory.GetFiles("temp\\lua\\pb\\", "*proto.lua", SearchOption.AllDirectories).Where(e => e.Contains(mode)))
                {
                    var luaLines = File.ReadAllLines(c2s);
                    foreach (var l in luaLines)
                    {
                        if (l.Contains(".id = "))
                        {
                            var hasOpcode = int.TryParse(l.Split(' ').Last(), out int opcode);
                            if (!hasOpcode || opcode == 0) continue;
                            var name = l.Split(' ')[0].Split('.')[1];
                            opcodes[mode][opcode] = name;
                        }
                    }
                }
            }
            var enumFile = new StringBuilder();
            enumFile.AppendLine("namespace Protos\r\n{");
            foreach (var mode in opcodes)
            {
                enumFile.AppendLine("    internal enum " + mode.Key + "PacketMessageIds");
                enumFile.AppendLine("    {");
                foreach (var opcode in mode.Value.OrderBy(e => e.Key))
                    enumFile.AppendLine("        " + opcode.Value + " = " + opcode.Key + ",");
                enumFile.AppendLine("    }");
            }
            enumFile.AppendLine("}");
            File.WriteAllText(@"C:\Users\" + Environment.UserName + @"\Documents\GitHub\Convallaria\Protos\PacketMessageIds.cs", enumFile.ToString());
        }
    }
}
