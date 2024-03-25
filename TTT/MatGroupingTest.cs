#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using net.rs64.TexTransCore.TransTextureCore.Utils;
using net.rs64.TexTransTool.Utils;
using UnityEditor;
using UnityEngine;

public class MatGroupingTest : MonoBehaviour
{
    public GameObject SearchRoot;
}

[CustomEditor(typeof(MatGroupingTest))]
public class MatGroupingTestEditor : Editor
{
    List<HashSet<(Material Mat, List<(string, Texture2D)> TexProps)>> MaterialGroupList;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var mgt = target as MatGroupingTest;
        if (GUILayout.Button("Refresh"))
        {
            var materialGroupList = new List<HashSet<(Material Mat, List<(string, Texture2D)> TexProps)>>();
            var materials = RendererUtility.GetMaterials(mgt.SearchRoot.GetComponentsInChildren<Renderer>())
            .Where(i => i != null)
            .Distinct()
            .Select(m => (m, new List<(string, Texture2D)>(MaterialUtility.GetAllTexture2DProperty(m))))
            .ToList();

            foreach (var mat in materials)
            {
                var index = materialGroupList.FindIndex(matGroup => matGroup.All(m2 => PropEqual(m2.TexProps, mat.Item2)));

                if (index == -1) { materialGroupList.Add(new HashSet<(Material Mat, List<(string, Texture2D)> TexProps)>() { mat }); }
                else { materialGroupList[index].Add(mat); }
            }

            MaterialGroupList = materialGroupList;


            // RootTree = new List<PropNode>();

            // foreach (var mat in materials)
            // {
            //     AddProp(RootTree, new Queue<(string, Texture2D)>(mat.Item2));
            // }

            // void AddProp(List<PropNode> nodes, Queue<(string, Texture2D)> queue)
            // {
            //     if (queue.TryDequeue(out var result))
            //     {
            //         var node = nodes.Find(i => i.Texture == result.Item2);

            //         if (node is null)
            //         {
            //             node = new(result.Item1, result.Item2);
            //             nodes.Add(node);
            //         }

            //         AddProp(node.Chiles, queue);
            //     }
            // }
        }
        if (MaterialGroupList is null) { return; }

        foreach (var matGroup in MaterialGroupList)
        {
            EditorGUILayout.LabelField("---");
            foreach (var mat in matGroup) { EditorGUILayout.ObjectField(mat.Mat, typeof(Material), false); }
        }



        // DrawTree(RootTree);

        // void DrawTree(List<PropNode> tree)
        // {
        //     foreach (var node in tree)
        //     {
        //         var rect = EditorGUILayout.GetControlRect();
        //         rect.width -= 18f;
        //         EditorGUI.LabelField(rect, node.PropName);
        //         rect.x += rect.width;
        //         rect.width = 18f;
        //         if (node.Texture != null) { EditorGUI.DrawTextureTransparent(rect, AssetPreview.GetAssetPreview(node.Texture)); }
        //         EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //         DrawTree(node.Chiles);
        //         EditorGUILayout.EndVertical();
        //     }
        // }

        bool PropEqual(List<(string, Texture2D)> propL, List<(string, Texture2D)> propR)
        {
            var count = propL.Count;
            if (count != propR.Count) { return false; }

            for (var i = 0; count > i; i += 1)
            {
                var l = propL[i];
                var r = propR[i];

                if (l.Item2 == null || r.Item2 == null) { continue; }
                if (l.Item2 != r.Item2) { Debug.Log("NYA"); return false; }
            }
            return true;
        }

    }


    public class PropNode
    {
        public string PropName;
        public Texture2D Texture;
        public List<PropNode> Chiles = new();

        public PropNode(string propName, Texture2D texture)
        {
            PropName = propName;
            Texture = texture;
        }
    }
}
#endif
