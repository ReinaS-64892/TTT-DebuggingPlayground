using System.IO;
using net.rs64.PSDParser;
using UnityEditor;
using UnityEngine;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal class ExtractLowLevelPSDData : TTTMenu.ITTTMenuWindow
    {
        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new ExtractLowLevelPSDData());
        }
        public string MenuName => "ExtractLowLevelPSDData";

        public UnityEngine.Object target;

        public void OnGUI()
        {
            target = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), target, typeof(UnityEngine.Object), false);

            if (GUILayout.Button("extract!"))
            {
                var path = AssetDatabase.GetAssetPath(target);
                if (string.IsNullOrWhiteSpace(path) || (Path.GetExtension(path) != ".psd" && Path.GetExtension(path) != ".psb")) { Debug.Log("知らない形式だぞ！！！"); }

                Debug.Log("PSDのLowLevelDataのしゅつりょくをするよ！");

                var lowlevelData = PSDLowLevelParser.Parse(path);
                var jsonStr = JsonUtility.ToJson(lowlevelData, true);

                File.WriteAllText(path + ".LowLevelData.json", jsonStr);
            }
        }
    }
}
