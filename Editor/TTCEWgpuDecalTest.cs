#if CONTAINS_TTCE_WGPU

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
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal class TTCEWgpuDecalTest : TTTMenu.ITTTMenuWindow
    {
        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new TTCEWgpuDecalTest());
        }
        public string MenuName => "TTCEWgpuDecalTest";

        public SimpleDecal SimpleDecal;
        public List<Texture2D> Results;

        public void OnGUI()
        {
            SimpleDecal = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), SimpleDecal, typeof(SimpleDecal), true) as SimpleDecal;
            if (GUILayout.Button("Do!"))
            {
                test();
            }
            EditorGUILayout.LabelField("Results");
            if (Results is not null)
                for (var i = 0; Results.Count > i; i += 1)
                    Results[i] = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), Results[i], typeof(Texture2D), true) as Texture2D;
        }
        void test()
        {
            if (Results.Any()) { foreach (var t in Results) { if (t != null) { UnityEngine.Object.DestroyImmediate(t); } } }
            var domainRoot = DomainMarkerFinder.FindMarker(SimpleDecal.gameObject);
            UnityObjectEqualityComparison originEqual = (l, r) => l == r;

            using var ttceWgpuDevice = new TTCEWgpuDeviceWithTTT4Unity(format: TexTransCoreTextureFormat.Byte);
            using var ttceWgpu = ttceWgpuDevice.GetTTCEWgpuContext();

            using var tempAssets = new TempAssetHolder();
            var domaineRenderers = new AvatarDomain(domainRoot, tempAssets, null, ttceWgpu);


            using var mulDecalTexture = SimpleDecal.GetDecalSourceTexture(domaineRenderers, ttceWgpu);

            var targetRenderers = SimpleDecal.TargetRenderers(domaineRenderers);
            var decalContext = SimpleDecal.GenerateDecalCtx(domaineRenderers, ttceWgpu);
            decalContext.DrawMaskMaterials = SimpleDecal.RendererSelector.GetOrNullAutoMaterialHashSet(domaineRenderers);

            var decalCompiledRenderTextures = decalContext.WriteDecalTexture<Texture>(domaineRenderers, targetRenderers, mulDecalTexture, SimpleDecal.TargetPropertyName) ?? new();

            Results = decalCompiledRenderTextures.Select(i => i.Value).Select(i => ttceWgpu.DownloadToTexture2D(i.Texture, false)).ToList();
            foreach (var t in decalCompiledRenderTextures) t.Value.Dispose();
        }
    }
}

#endif
