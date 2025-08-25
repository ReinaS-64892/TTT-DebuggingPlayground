using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using net.rs64.TexTransTool;
using System.IO;
using System.Linq;
using System;
using System.Reflection;
namespace net.rs64.TexTransTool.DebuggingPlayground
{
    internal class TTTSaveDataVersionUtility : TTTMenu.ITTTMenuWindow
    {
        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new TTTSaveDataVersionUtility());
        }
        public string MenuName => "TTTSaveDataVersionUtility";

        TexTransMonoBase target;
        Action<object, object> fieldSet;

        public void OnGUI()
        {
            target = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), target, typeof(TexTransMonoBase), true) as TexTransMonoBase;
            if (target != null)
            {
                fieldSet ??= typeof(TexTransMonoBase).GetField("_saveDataVersion", BindingFlags.Instance | BindingFlags.NonPublic).SetValue;
                Undo.RecordObject(target, "set SaveDataVersion");
                var bVal = (target as ITexTransToolTag).SaveDataVersion;
                var val = EditorGUILayout.IntField("SaveDataVersion", bVal);
                if (bVal != val) { fieldSet(target, val); }
            }
            if (GUILayout.Button("RevertPrefabOverrideTTTSaveDataVersion-ALL"))
                RevertPrefabOverrideTTTSaveDataVersion();
            if (GUILayout.Button("DecrementTTTSaveDataVersion-ALL"))
                DecrementTTTSaveDataVersion();
        }
        static void RevertPrefabOverrideTTTSaveDataVersion()
        {
            var result = EditorUtility.DisplayDialog("警告", "TTTSaveDataVersion の プレハブオーバーライドを全部消すわよ！！", "やって", "だめ");
            if (result is false) { return; }

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
        static void DecrementTTTSaveDataVersion()
        {
            var result = EditorUtility.DisplayDialog("警告", "TTTSaveDataVersion の を全部無理やり下げるよ！どうなってもしらないからね！", "やって", "だめ");
            if (result is false) { return; }

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
