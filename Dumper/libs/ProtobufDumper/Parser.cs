using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufDumper
{
    public static class Parser
    {
        public static void ProcessFile(string[] targets)
        {
            var collector = new ProtobufCollector();
            foreach (var target in targets)
            {
                Console.WriteLine("Loading binary '{0}'...", target);

                ExecutableScanner.ScanFile(target, (name, buffer) =>
                {
                    if (collector.Candidates.Find(c => c.name == name) != null) return true;

                    //if (!name.Contains("game_account_handle")) return true;
                    Console.Write("{0}... ", name);

                    var complete = collector.TryParseCandidate(name, buffer, out var result, out var error);

                    switch (result)
                    {
                        case ProtobufCollector.CandidateResult.OK:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("OK!");
                            Console.ResetColor();
                            break;

                        case ProtobufCollector.CandidateResult.Rescan:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("needs rescan: {0}", error.Message);
                            Console.ResetColor();
                            break;

                        default:
                        case ProtobufCollector.CandidateResult.Invalid:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("is invalid: {0}", error.Message);
                            Console.ResetColor();
                            break;
                    }

                    if (complete)
                    {
                        var fileName = Path.Combine("temp", $"{name}.dump");
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                        Console.WriteLine("  ! Dumping to '{0}'!", fileName);

                        try
                        {
                            using (var file = File.OpenWrite(fileName))
                            {
                                buffer.Seek(0, SeekOrigin.Begin);
                                file.SetLength(buffer.Length);
                                buffer.CopyTo(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Unable to dump: {0}", ex.Message);
                        }
                    }

                    return complete;
                });
            }

            var dumper = new ProtobufDumper(collector.Candidates);

            if (dumper.Analyze())
            {
                dumper.DumpFiles((name, buffer) =>
                {
                    var outputFile = Path.Combine("temp", name);

                    Console.WriteLine("  ! Outputting proto to '{0}'", outputFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                    File.WriteAllText(outputFile, buffer.ToString());
                });
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Dump failed. Not all dependencies and types were found.");
                Console.ResetColor();

                Environment.ExitCode = -1;
            }
        }
    }
}
