#if UNITY_EDITOR && CONTAINS_TTCE_WGPU

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using net.rs64.TexTransCore;
using net.rs64.TexTransCore.TransTexture;
using net.rs64.TexTransCoreEngineForUnity;
using net.rs64.TexTransCoreEngineForWgpu;
using net.rs64.TexTransTool.Decal;
using net.rs64.TexTransTool.MultiLayerImage;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    public class TTCEWgpuDecalTest : MonoBehaviour
    {
        public SimpleDecal SimpleDecal;
        public List<Texture2D> Results;


        [ContextMenu("Test!!!")]
        void test()
        {
            if (Results.Any()) { foreach (var t in Results) { if (t != null) { DestroyImmediate(t); } } }
            using var ttceWgpuDevice = new TTCEWgpuDevice();
            ttceWgpuDevice.SetDefaultTextureFormat(TexTransCoreTextureFormat.Byte);
            var sd = ShaderFinder.RegisterShaders(ttceWgpuDevice, ShaderFinder.GetAllShaderPathWithCurrentDirectory(), ShaderFinder.CurrentDirectoryFind);

            using var ttceWgpu = ttceWgpuDevice.GetContext<TTCEWgpuWithTTT4Unity>();
            ttceWgpu.ShaderDictionary = sd;

            var decalTexture = ttceWgpu.LoadTextureWidthFullScale((ttceWgpu as ITexTransToolForUnity).Wrapping(SimpleDecal.DecalTexture));

            var decalCtx = new DecalContext<ParallelProjectionSpace, ITrianglesFilter<ParallelProjectionSpace>>(ttceWgpu, SimpleDecal.GetSpaceConverter(), SimpleDecal.GetTriangleFilter());
            decalCtx.DecalPadding = SimpleDecal.Padding;
            decalCtx.TargetPropertyName = SimpleDecal.TargetPropertyName;


            var decalCompiledRenderTextures = new Dictionary<Material, TTRenderTexWithDistance>();
            foreach (var renderer in SimpleDecal.TargetRenderers)
            {
                decalCtx.WriteDecalTexture(decalCompiledRenderTextures, renderer, decalTexture);
            }

            Results = decalCompiledRenderTextures.Select(i => i.Value).Select(i => ttceWgpu.DownloadToTexture2D(i.Texture)).ToList();
            foreach (var t in decalCompiledRenderTextures) t.Value.Dispose();
        }
    }
}

#endif
