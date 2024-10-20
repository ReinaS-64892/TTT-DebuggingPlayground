#if UNITY_EDITOR
using System.IO;
using System.Linq;
using net.rs64.PSDParser;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal static class ExtractLowLevelPSDData
    {
        internal const string AssetForBasePath = "Assets/TexTransTool/TTTDebuggingPlayground";
        internal const string ToolsForBasePath = "Tools/TexTransTool/TTTDebuggingPlayground";

        [MenuItem(AssetForBasePath + "/ExtractLowLevelPSDData")]
        static void Execute()
        {
            var target = Selection.activeObject;

            var path = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrWhiteSpace(path) || (Path.GetExtension(path) != ".psd" && Path.GetExtension(path) != ".psb")) { Debug.Log("知らない形式だぞ！！！"); }

            Debug.Log("PSDのLowLevelDataのしゅつりょくをするよ！");

            var lowlevelData = PSDLowLevelParser.Parse(path);
            var jsonStr = JsonUtility.ToJson(lowlevelData, true);

            File.WriteAllText(path + ".LowLevelData.json", jsonStr);
        }

    }
}
#endif
