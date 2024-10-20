#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using net.rs64.PSDParser;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal static class ExtractLowLevelPSDImageDataExtract
    {
        [MenuItem(ExtractLowLevelPSDData.AssetForBasePath + "/ImageDataExtract")]
        static void Execute()
        {
            var target = Selection.activeObject;

            var path = AssetDatabase.GetAssetPath(target);
            if (string.IsNullOrWhiteSpace(path) || (Path.GetExtension(path) != ".psd" && Path.GetExtension(path) != ".psb")) { Debug.Log("知らない形式"); }

            Debug.Log("PSDの ImageData を書き出してみる");

            var psdByte = File.ReadAllBytes(path);
            var lowLevelData = PSDLowLevelParser.Parse(psdByte);



            var format = TexTransTool.PSDImporter.PSDImportedRasterImage.BitDepthToTextureFormat(lowLevelData.BitDepth, lowLevelData.Channels);
            var buffer = new byte[ChannelImageDataParser.ChannelImageData.GetImageByteCount((int)lowLevelData.Width, (int)lowLevelData.Height, lowLevelData.BitDepth) * lowLevelData.Channels];

            PSDLowLevelParser.LoadImageData(psdByte, lowLevelData, buffer);

            var tex = new Texture2D((int)lowLevelData.Width, (int)lowLevelData.Height, format, false);

            tex.LoadRawTextureData(buffer);
            tex.Apply();

            File.WriteAllBytes(path + ".LowLevelData.png", tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);

            // AssetDatabase.CreateAsset(tex, path + ".LowLevelData.asset");
        }

    }
}
#endif
