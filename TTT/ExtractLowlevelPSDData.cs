#if UNITY_EDITOR
using System.IO;
using System.Linq;
using net.rs64.MultiLayerImage.Parser.PSD;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace net.rs64.TTTDebuggingPlayground.TTT
{

    internal static class ExtractLowlevelPSDData
    {
        internal const string AssetforBasePath = "Assets/TexTransTool/TTTDebuggingPlayground";
        internal const string ToolsforBasePath = "Tools/TexTransTool/TTTDebuggingPlayground";

        [MenuItem(AssetforBasePath + "/ExtractLowlevelPSDData")]
        static void Execute()
        {
            var target = Selection.activeObject;

            var path = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrWhiteSpace(path) || Path.GetExtension(path) != ".psd") { return; }

            Debug.Log("こにゃ～ん...PSDのLowLevelDataのしゅつりょくをするよ～");

            var lowlevelData = PSDLowLevelParser.Parse(path);
            var jsonStr = JsonUtility.ToJson(lowlevelData, true);

            File.WriteAllText(path + ".LowLevelData.json", jsonStr);
        }

    }
}
#endif
