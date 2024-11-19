#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.rs64.TexTransCore;
using net.rs64.TexTransCoreEngineForWgpu;
using net.rs64.TexTransTool.MultiLayerImage;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    public class TTCEWgpuTest : MonoBehaviour
    {
        public MultiLayerImageCanvas Canvas;
        public Texture2D Result;


        [ContextMenu("Test!!!")]
        void test()
        {
            if (Result != null) { DestroyImmediate(Result); }
            using var ttceWgpuDevice = new TTCEWgpuDevice();
            ttceWgpuDevice.SetDefaultTextureFormat(TexTransCoreTextureFormat.Byte);
            var sd = ShaderFinder.RegisterShaders(ttceWgpuDevice, ShaderFinder.GetAllShaderPathWithCurrentDirectory());

            using var ttceWgpu = ttceWgpuDevice.GetContext<TTCEWgpuWithTTT4Unity>();
            ttceWgpu.ShaderDictionary = sd;

            var resultRt = Canvas.EvaluateCanvas(ttceWgpu, Canvas?.tttImportedCanvasDescription?.Width ?? 1024, Canvas?.tttImportedCanvasDescription?.Height ?? 1024);
            var tex = new Texture2D(resultRt.Width, resultRt.Hight, TextureFormat.RGBA32, false);
            var map = tex.GetRawTextureData<byte>();

            ttceWgpu.DownloadTexture(map.AsSpan(), TexTransCoreTextureFormat.Byte, resultRt);
            tex.Apply();
            Result = tex;
        }
    }
}
#endif
