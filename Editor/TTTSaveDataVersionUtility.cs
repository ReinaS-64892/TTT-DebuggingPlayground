#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using net.rs64.TexTransTool;
using System.IO;
using System.Linq;
namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal static class TTTSaveDataVersionUtility
    {
        [MenuItem(ExtractLowLevelPSDData.ToolsForBasePath + "/RevertPrefabOverrideTTTSaveDataVersion")]
        static void RevertPrefabOverrideTTTSaveDataVersion()
        {
            Debug.Log("TTTSaveDataVersion の プレハブオーバーライドを全部消すわよ！！");

            foreach (var prefab in EnumerateModifiablePrefabs())
            {
                foreach (var c in prefab.GetComponentsInChildren<ITexTransToolTag>(true).OfType<Component>())
                {
                    var sc = new SerializedObject(c);
                    var sdvp = sc.FindProperty("_saveDataVersion");
                    if (sdvp == null) { continue; }
                    sdvp.prefabOverride = false;
                    sc.ApplyModifiedProperties();
                }
            }

        }

        private static IEnumerable<GameObject> EnumerateModifiablePrefabs()
        {
            bool CheckPrefabType(PrefabAssetType prefabAssetType)
            {
                switch (prefabAssetType)
                {
                    default:
                    case PrefabAssetType.MissingAsset:
                    case PrefabAssetType.Model:
                    case PrefabAssetType.NotAPrefab:
                        return false;

                    case PrefabAssetType.Regular:
                    case PrefabAssetType.Variant:
                        return true;
                }
            }


            var targets = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                .Where(p => CheckPrefabType(PrefabUtility.GetPrefabAssetType(p)))
                .Where(p => p.GetComponentsInChildren<ITexTransToolTag>(true).Length != 0);

            foreach (var p in targets)
            {
                var prefabPath = AssetDatabase.GetAssetPath(p);
                var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
                AssetDatabase.OpenAsset(prefab);

                yield return p;

                PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }

        [MenuItem(ExtractLowLevelPSDData.ToolsForBasePath + "/DecrementTTTSaveDataVersion")]
        static void DecrementTTTSaveDataVersion()
        {
            Debug.Log("TTTSaveDataVersion の を全部無理やり下げるよ！どうなってもしらないからね！");
            foreach (var prefab in EnumerateModifiablePrefabs())
            {
                foreach (var c in prefab.GetComponentsInChildren<ITexTransToolTag>(true).OfType<Component>())
                {
                    var sc = new SerializedObject(c);
                    var sdvp = sc.FindProperty("_saveDataVersion");
                    if (sdvp == null) { continue; }
                    sdvp.intValue -= 1;
                    sc.ApplyModifiedProperties();
                }
            }
        }
    }


}
#endif
