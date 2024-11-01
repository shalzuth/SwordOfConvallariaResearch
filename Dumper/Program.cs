using Dumper;

await Downloader.GetVersionInfo();
await Downloader.DownloadUnity3dFiles();
var rawFiles = Unity.GetFileNames();
foreach(var file in rawFiles)
{
    if (!file.Key.StartsWith("lua")) continue;
    var fileName ="temp\\unity3d\\" + file.Key.Replace("lua_", "") + ".unity3d";
    if (File.Exists(fileName)) continue;
    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
    File.WriteAllBytes(fileName, await Downloader.GetFile("WebAssets", file.Key + "_" + file.Value + ".unity3d"));
}

Unity.DumpLuacAndPbFromUnity();
Directory.GetFiles("temp\\luac\\", "*.luac", SearchOption.AllDirectories).ToList().ForEach(Lua.DecryptDecompile);
Protobuf.Process();
OpCodes.Parse();