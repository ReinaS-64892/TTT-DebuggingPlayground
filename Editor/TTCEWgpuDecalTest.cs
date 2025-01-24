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
            var domainRoot = DomainMarkerFinder.FindMarker(SimpleDecal.gameObject);
            var domaineRenderers = new AvatarDomain(domainRoot, false, false, null);
            OriginEqual originEqual = (l, r) => l == r;

            using var ttceWgpuDevice = new TTCEWgpuDevice();
            ttceWgpuDevice.SetDefaultTextureFormat(TexTransCoreTextureFormat.Byte);
            var sd = ShaderFinder.RegisterShaders(ttceWgpuDevice, ShaderFinder.GetAllShaderPathWithCurrentDirectory(), ShaderFinder.CurrentDirectoryFind);

            using var ttceWgpu = ttceWgpuDevice.GetContext<TTCEWgpuWithTTT4Unity>();
            ttceWgpu.ShaderDictionary = sd;

            var decalTexture = ttceWgpu.LoadTextureWidthFullScale((ttceWgpu as ITexTransToolForUnity).Wrapping(SimpleDecal.DecalTexture));

            var decalCtx = new DecalContext
            <ParallelProjectionSpaceConvertor, ParallelProjectionSpace, ITrianglesFilter<ParallelProjectionSpace, IFilteredTriangleHolder>, IFilteredTriangleHolder>
            (ttceWgpu, SimpleDecal.GetSpaceConverter(), SimpleDecal.GetTriangleFilter(originEqual));
            decalCtx.TargetPropertyName = SimpleDecal.TargetPropertyName;
            decalCtx.IsTextureStretch = false;
            decalCtx.DecalPadding = SimpleDecal.Padding;
            decalCtx.HighQualityPadding = false;
            decalCtx.UseDepthOrInvert = SimpleDecal.GetUseDepthOrInvert;
            decalCtx.DrawMaskMaterials = SimpleDecal.RendererSelector.GetOrNullAutoMaterialHashSet(domaineRenderers);

            var targetRenderers = SimpleDecal.RendererSelector.GetSelectedOrIncludingAll(domaineRenderers, out _);
            var decalCompiledRenderTextures = decalCtx.WriteDecalTexture<Texture2D>(targetRenderers, decalTexture);

            Results = decalCompiledRenderTextures.Select(i => i.Value).Select(i => ttceWgpu.DownloadToTexture2D(i.Texture)).ToList();
            foreach (var t in decalCompiledRenderTextures) t.Value.Dispose();
        }
    }
}

#endif
