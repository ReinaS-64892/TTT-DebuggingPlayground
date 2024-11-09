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
            using var ttceWgpuDevice = new TTCEWgpuDevice();
            var sd = ShaderFinder.RegisterShaders(ttceWgpuDevice);

            using var ttceWgpu = ttceWgpuDevice.GetContext<TTCEWgpuWithTTT4U>();
            ttceWgpu.ShaderDictionary = sd;


            var resultRt = Canvas.EvaluateCanvas(ttceWgpu, 1024, 1024);
            var tex = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
            var map = tex.GetRawTextureData<byte>();

            ttceWgpu.DownloadTexture(map.AsSpan(), TexTransCoreTextureFormat.Byte, resultRt);
            tex.Apply();
            Result = tex;
        }
    }


    class TTCEWgpuWithTTT4U : TTCEWgpu, ITexTransToolForUnity
    {
        public ShaderFinder.ShaderDictionary ShaderDictionary = null!;//気を付けるようにね！
        public ITexTransStandardComputeKey StandardComputeKey => ShaderDictionary;

        public ITexTransComputeKeyDictionary<string> GrabBlend => ShaderDictionary;

        public ITexTransComputeKeyDictionary<ITTBlendKey> BlendKey => ShaderDictionary;
        public ITTBlendKey QueryBlendKey(string blendKeyName) => ShaderDictionary.QueryBlendKey(blendKeyName);


        public void UploadTexture<T>(ITTRenderTexture uploadTarget, ReadOnlySpan<T> bytes, TexTransCoreTextureFormat format) where T : unmanaged
        {
            base.UploadTexture((TTRenderTexture)uploadTarget, bytes, format);
        }
        public void DownloadTexture<T>(Span<T> dataDist, TexTransCoreTextureFormat format, ITTRenderTexture renderTexture) where T : unmanaged
        {
            base.DownloadTexture(dataDist, format, (TTRenderTexture)renderTexture);
        }

        public void LoadTexture(ITTRenderTexture writeTarget, ITTDiskTexture diskTexture)
        {
            if (writeTarget.Width != diskTexture.Width || writeTarget.Hight != diskTexture.Hight) { throw new ArgumentException(); }
            var rtDisk = (RenderTextureAsDiskTexture)diskTexture;
            CopyRenderTexture(writeTarget, rtDisk.TTRenderTexture);
        }
    }


    public static class ShaderFinder
    {
        public static ShaderDictionary RegisterShaders(this TTCEWgpuDevice device)
        {
            var shaderDicts = new Dictionary<TTComputeType, Dictionary<string, TTComputeShaderID>>();
            var ttcompPaths = GetAllShaderPathWithCurrentDirectory().ToArray();
            foreach (var path in ttcompPaths)
            {
                var computeName = Path.GetFileNameWithoutExtension(path);

                var srcText = File.ReadAllText(path);
                if (srcText.Contains("UnityCG.cginc")) { throw new InvalidDataException(" UnityCG.cginc は使用してはいけません！"); }

                var descriptions = TTComputeShaderUtility.Parse(srcText);
                if (descriptions is null) { continue; }

                if (shaderDicts.ContainsKey(descriptions.ComputeType) is false) { shaderDicts[descriptions.ComputeType] = new(); }

                switch (descriptions.ComputeType)
                {
                    case TTComputeType.General:
                        {
                            shaderDicts[descriptions.ComputeType][computeName] = device.RegisterComputeShaderFromHLSL(path);
                            break;
                        }
                    case TTComputeType.GrabBlend:
                        {
                            shaderDicts[descriptions.ComputeType][computeName] = device.RegisterComputeShaderFromHLSL(path);
                            break;
                        }
                    case TTComputeType.Blending:
                        {
                            var blendKey = descriptions["Key"];
                            var csCode = TexTransCoreEngineForUnity.TTBlendingComputeShader.KernelDefine + srcText + TTComputeShaderUtility.BlendingShaderTemplate;
                            shaderDicts[descriptions.ComputeType][blendKey] = device.RegisterComputeShaderFromHLSL(path, csCode);

                            break;
                        }
                }
            }

            return new(shaderDicts);
        }

        public static IEnumerable<string> GetAllShaderPathWithCurrentDirectory()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ttcomp", SearchOption.AllDirectories).Concat(Directory.GetFiles(Directory.GetCurrentDirectory(), "*.ttblend", SearchOption.AllDirectories));
        }

        public class ShaderDictionary : ITexTransStandardComputeKey, ITexTransComputeKeyDictionary<string>, ITexTransComputeKeyDictionary<ITTBlendKey>
        {
            private Dictionary<TTComputeType, Dictionary<string, TTComputeShaderID>> _shaderDict;


            public ITTComputeKey this[string key] => _shaderDict[TTComputeType.GrabBlend][key];

            public ITTComputeKey this[ITTBlendKey key] => ((BlendKey)key).ComputeKey;

            public ITTComputeKey AlphaFill { get; private set; }
            public ITTComputeKey AlphaCopy { get; private set; }
            public ITTComputeKey AlphaMultiply { get; private set; }
            public ITTComputeKey AlphaMultiplyWithTexture { get; private set; }
            public ITTComputeKey ColorFill { get; private set; }
            public ITTComputeKey ColorMultiply { get; private set; }
            public ITTComputeKey BilinearReScaling { get; private set; }
            public ITTComputeKey GammaToLinear { get; private set; }
            public ITTComputeKey LinearToGamma { get; private set; }
            public ITTBlendKey QueryBlendKey(string blendKeyName)
            {
                return new BlendKey(_shaderDict[TTComputeType.Blending][blendKeyName]);
            }

            public ShaderDictionary(Dictionary<TTComputeType, Dictionary<string, TTComputeShaderID>> dict)
            {
                _shaderDict = dict;
                AlphaFill = _shaderDict[TTComputeType.General][nameof(AlphaFill)];
                AlphaCopy = _shaderDict[TTComputeType.General][nameof(AlphaCopy)];
                AlphaMultiply = _shaderDict[TTComputeType.General][nameof(AlphaMultiply)];
                AlphaMultiplyWithTexture = _shaderDict[TTComputeType.General][nameof(AlphaMultiplyWithTexture)];
                ColorFill = _shaderDict[TTComputeType.General][nameof(ColorFill)];
                ColorMultiply = _shaderDict[TTComputeType.General][nameof(ColorMultiply)];
                BilinearReScaling = _shaderDict[TTComputeType.General][nameof(BilinearReScaling)];
                GammaToLinear = _shaderDict[TTComputeType.General][nameof(GammaToLinear)];
                LinearToGamma = _shaderDict[TTComputeType.General][nameof(LinearToGamma)];
            }

        }

        class BlendKey : ITTBlendKey
        {
            public ITTComputeKey ComputeKey;

            public BlendKey(ITTComputeKey computeKey)
            {
                ComputeKey = computeKey;
            }
        }
    }
}
#endif
