using ProtobufDumper;
namespace Dumper
{
    internal class Protobuf
    {
        internal static void Process()
        {
            Parser.ProcessFile(Directory.GetFiles(@"temp\pb"));
            Directory.CreateDirectory("temp\\proto");
            Directory.GetFiles(@"temp", "*.proto").ToList().ForEach(f =>
            {
                if (f.EndsWith("PacketsMultiple.proto")) return;
                var protoLines = new string[] { "syntax = \"proto2\";" }.Concat(File.ReadAllLines(f)).ToArray();
                var parentType = "";
                var nestedParentType = "";
                // fix up proto to work with C# protobuf
                for (var i = 0; i < protoLines.Length; i++)
                {
                    protoLines[i] = protoLines[i].Replace("hasLogicError", "hasLogicErrorField");
                    if (protoLines[i].Contains(".Type "))
                    {
                        var parts = protoLines[i].Split(new char[] { '.', ' ' }).ToList();
                        var typeName = parts.IndexOf("Type");
                        if (parts[typeName - 1] == "SandboxRoundReport") continue;
                        protoLines[i] = protoLines[i].Replace(".Type", "." + parts[typeName - 1] + "Enum");
                    }
                    if (protoLines[i].StartsWith("message "))
                    {
                        var parts = protoLines[i].Split(new char[] { '.', ' ' }).ToList();
                        parentType = parts[1];
                    }
                    if (protoLines[i].StartsWith("\tmessage "))
                    {
                        var parts = protoLines[i].Split(new char[] { '.', ' ' }).ToList();
                        nestedParentType = parts[1];
                    }
                    if (protoLines[i].StartsWith("\tenum Type "))
                    {
                        protoLines[i] = protoLines[i].Replace("Type", parentType + "Enum");
                    }
                    if (protoLines[i].StartsWith("\t\tenum Type "))
                    {
                        protoLines[i] = protoLines[i].Replace("Type", nestedParentType + "Enum");
                    }
                }
                File.WriteAllLines(f.Replace("temp\\", "C:\\Users\\" + Environment.UserName + "\\Documents\\GitHub\\SwordOfConvallariaResearch\\Protos\\protos\\"), protoLines);
            });
            Directory.GetFiles(@"temp", "*.proto").ToList().ForEach(File.Delete);
            Directory.GetFiles(@"temp", "*.dump").ToList().ForEach(File.Delete);
        }
    }
}
