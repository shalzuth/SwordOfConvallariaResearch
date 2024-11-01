using AssetsTools.NET.Extra;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Dumper
{
    internal class Downloader
    {
        static HttpClient client = new HttpClient();
        static string Host = "example.com";
        static JsonNode versionInfo;
        internal static async Task GetVersionInfo()
        {
            versionInfo = await client.GetFromJsonAsync<JsonNode>($"https://{Host}/version/us-prod/1.12.2.17531");
        }
        static JsonNode webAssets;
        internal static async Task<byte[]> GetWebAssets()
        {
            return await client.GetByteArrayAsync(versionInfo["url_asset"] + "WebAssets/asset_md5_" + versionInfo["pc_md5"] + ".unity3d");
        }
        static JsonNode fileInfo;
        internal static async Task GetFileInfo()
        {
            fileInfo = await client.GetFromJsonAsync<JsonNode>(versionInfo["url_asset"] + "pc/GameFileInfo_" + versionInfo["win_md5"] + ".json");
        }
        static JsonNode xdConfig;
        internal static async Task GetConfig()
        {
            xdConfig = await client.GetFromJsonAsync<JsonNode>(versionInfo["url_asset"] + "pc/SoC_Data/StreamingAssets/XDConfig.json");
        }
        internal static async Task<byte[]> GetFile(string name)
        {
            var pieces = name.Split('.');
            return await client.GetByteArrayAsync(versionInfo["url_asset"] + "pc/" + pieces[0] + "_" + fileInfo["FileInfos"].AsArray().FirstOrDefault(a => (string)a["FileName"] == name)["Md5Hash"] + "." + pieces[1]);
        }
        internal static async Task<byte[]> GetFile(string dir, string name)
        {
            return await client.GetByteArrayAsync(versionInfo["url_asset"] + dir + "/" + name);
        }
        internal static async Task DownloadUnity3dFiles()
        {
            var temp = "temp\\";
            Directory.CreateDirectory(temp);
            await GetFileInfo();
            if (!File.Exists(temp + "asset_md5.unity3d"))
            {
                File.WriteAllBytes(temp + "asset_md5.unity3d", await GetWebAssets());
            }
            if (!File.Exists(temp + "data.unity3d"))
            {
                File.WriteAllBytes(temp + "data.unity3d", await GetFile("SoC_Data/data.unity3d"));
            }
        }
    }
}
