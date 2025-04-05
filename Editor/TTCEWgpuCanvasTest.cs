#if UNITY_EDITOR && CONTAINS_TTCE_WGPU

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.rs64.TexTransCore;
using net.rs64.TexTransCoreEngineForWgpu;
using net.rs64.TexTransTool.MultiLayerImage;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    public class TTCEWgpuCanvasTest : MonoBehaviour
    {
        public MultiLayerImageCanvas Canvas;
        public Texture2D Result;


        [ContextMenu("Test!!!")]
        void test()
        {
            if (Result != null) { DestroyImmediate(Result); }
            Profiler.BeginSample("ctr TTCEWgpuDevice");

            using var ttceWgpuDevice = new TTCEWgpuDeviceWithTTT4Unity(format:TexTransCoreTextureFormat.Byte);
            using var ttceWgpu = ttceWgpuDevice.GetTTCEWgpuContext();

            var nwDomain = new NotWorkDomain(Array.Empty<Renderer>(), ttceWgpu);

            Profiler.EndSample();
            Profiler.BeginSample("EvaluateCanvas");

            var gloCtx = new GenerateLayerObjectContext(nwDomain, (Canvas?.tttImportedCanvasDescription?.Width ?? 1024, Canvas?.tttImportedCanvasDescription?.Height ?? 1024));
            var resultRt = Canvas.EvaluateCanvas(gloCtx);

            Profiler.EndSample();
            Profiler.BeginSample("Finalize");
            var tex = new Texture2D(resultRt.Width, resultRt.Hight, TextureFormat.RGBA32, false);
            var map = tex.GetRawTextureData<byte>();

            Profiler.BeginSample("DownloadTexture");
            ttceWgpu.DownloadTexture(map.AsSpan(), TexTransCoreTextureFormat.Byte, resultRt);
            Profiler.EndSample();
            tex.Apply();
            Result = tex;
            Profiler.EndSample();
        }
    }
}
#endif
