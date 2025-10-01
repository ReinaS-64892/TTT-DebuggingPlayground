#nullable enable
#if CONTAINS_TTT_CLIP_IMPORTER
using System.IO;
using net.rs64.TexTransTool.ClipImporter;
using net.rs64.TexTransTool.ClipParser;
using UnityEditor;
using UnityEngine;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal class ClipImporterDebug : TTTMenu.ITTTMenuWindow
    {
        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new ClipImporterDebug());
        }
        public string MenuName => "ClipImporterDebug";

        public UnityEngine.Object? target;
        private (SerializedObject, ImporterResult)? result;

        public void OnGUI()
        {
            target = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), target, typeof(UnityEngine.Object), false);

            if (target != null)
            {
                if (GUILayout.Button("DO DebugExtracting "))
                {
                    var path = AssetDatabase.GetAssetPath(target);
                    if (Path.GetExtension(path) is not ".clip") { Debug.Log("not .clip file"); return; }

                    var bytes = File.ReadAllBytes(path);
                    var data = ClipLowLevelParser.Parse(bytes);
                    var resultObject = ScriptableObject.CreateInstance<ImporterResult>();
                    resultObject.ClipLowLevelData = data;
                    result = (new SerializedObject(resultObject), resultObject);

                    foreach (var extraData in data.CHNKExtaList)
                    {
                        var extraDataID = extraData.ExternalID;
                        foreach (var b in extraData.DataArray)
                        {
                            var tex = new Texture2D((int)b.BlockWidth, (int)b.BlockHeight, TextureFormat.RGBA32, false);

                            if (b.NotEmpty is not 0) tex.SetPixelData(b.LoadColorData(bytes), 0);
                            else tex.GetRawTextureData<ClipParser.Color32>().AsSpan().Fill(new ClipParser.Color32(0, 0, 0, 0));

                            tex.name = "ClipExtractedImage-" + extraDataID + "-" + b.BlockDataIndex;
                            AssetSaveHelper.SavePNG(tex);
                            UnityEngine.Object.DestroyImmediate(tex);
                        }
                    }
                    AssetSaveHelper.SaveBinary("ClipSQLiteData.sqlite3", new ParserUtility.BinarySectionStream(bytes, data.SQLiteDataAdders).ReadToArray());
                }
                if (GUILayout.Button("DO SQLite3Test "))
                {
                    var path = AssetDatabase.GetAssetPath(target);
                    if (Path.GetExtension(path) is not ".clip") { Debug.Log("not .clip file"); return; }

                    var bytes = File.ReadAllBytes(path);
                    var data = ClipLowLevelParser.Parse(bytes);
                    ClipHighLevelParser.Parse(bytes, data, new TexTransToolClipImporter.UnitySQLiteWrapper());
                }
            }

            if (result.HasValue && result.Value.Item1 != null && result.Value.Item2 != null)
            {
                EditorGUILayout.PropertyField(result.Value.Item1.FindProperty(nameof(ImporterResult.ClipLowLevelData)));
            }

        }
        class ImporterResult : ScriptableObject
        {
            public ClipLowLevelData? ClipLowLevelData = null;
        }
    }
}
#endif
