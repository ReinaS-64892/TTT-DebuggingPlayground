#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using net.rs64.TexTransTool;
using net.rs64.TexTransCore;
using net.rs64.TexTransCoreEngineForUnity;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    public class ColorSpaceTest : MonoBehaviour
    {
        public Texture2D Source;
        public RenderTexture OutPut;
        public TexTransCoreTextureFormat Format;

        [ContextMenu("Test")]
        void Test()
        {
            OutPut = new RenderTexture(Source.width, Source.height, 0, Format.ToUnityGraphicsFormat());
            Graphics.Blit(Source, OutPut);
        }


    }

}
#endif
