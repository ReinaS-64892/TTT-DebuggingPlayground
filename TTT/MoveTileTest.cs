#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using net.rs64.TexTransTool;
namespace net.rs64.TTTDebuggingPlayground.TTT
{

    public class MoveTileTest : MonoBehaviour
    {
        public Mesh Target;

        [ContextMenu("Move!!!")]
        void move()
        {
            var newMesh = Instantiate(Target);

            var uv = newMesh.uv;
            for (var i = 0; uv.Length > i; i += 1)
            {
                uv[i] += Vector2.one;
            }
            newMesh.uv = uv;

            AssetDatabase.CreateAsset(newMesh, AssetDatabase.GenerateUniqueAssetPath("Assets/" + newMesh.name + ".mesh"));
        }
    }

}
#endif
