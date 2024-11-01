using AssetsTools.NET.Extra;
using System.Text.Json.Nodes;

namespace Dumper
{
    internal class Unity
    {
        internal static Dictionary<string,string> GetFileNames()
        {
            var temp = "temp\\";
            var manager = new AssetsManager();
            var bunInst = manager.LoadBundleFile(new MemoryStream(File.ReadAllBytes(temp + "asset_md5.unity3d")), "fakeassets.assets");
            var fileInstance = manager.LoadAssetsFileFromBundle(bunInst, 0, false);
            var assetFile = fileInstance.file;
            var textBase = manager.GetBaseField(fileInstance, assetFile.GetAssetsOfType(AssetClassID.TextAsset).First());
            var m_Script = JsonNode.Parse(textBase["m_Script"].AsString).AsObject();
            var files = new Dictionary<string,string>();
            foreach (var o in m_Script) files[o.Key] = o.Value["md5"].ToString();
            return files;
        }
        internal static void DumpLuacAndPbFromUnity()
        {
            var temp = "temp\\";
            foreach (var luabase in Directory.GetFiles(temp + "unity3d\\lua"))
            {
                var manager = new AssetsManager();
                var bunInst = manager.LoadBundleFile(new MemoryStream(File.ReadAllBytes(luabase)), "fakeassets.assets");
                var fileInstance = manager.LoadAssetsFileFromBundle(bunInst, 0, false);
                var assetFile = fileInstance.file;
                foreach (var asset in assetFile.GetAssetsOfType(AssetClassID.TextAsset))
                {
                    var textBase = manager.GetBaseField(fileInstance, asset);
                    var m_Name = textBase["m_Name"].AsString;
                    var m_Script = textBase["m_Script"].AsByteArray;
                    var fileName = temp + @"luac\" + Path.GetFileNameWithoutExtension(luabase) + @"\" + m_Name.Replace("_", @"\\") + ".luac";
                    if (m_Name.EndsWith(".proto")) fileName = temp + Path.GetFileNameWithoutExtension(luabase) + @"\" + m_Name;
                    if (File.Exists(fileName)) continue;
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    File.WriteAllBytes(fileName, m_Script);
                }
            }
        }
    }
}
