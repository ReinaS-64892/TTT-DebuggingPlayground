using UnityEditor;
using System.IO;
using UnityEngine;
using System;

namespace net.rs64.TexTransTool.DebuggingPlayground
{
    internal static class AssetSaveHelper
    {
        public const string SaveDirectory = "Assets/TexTransToolGenerates";

        internal static string SavePNG(UnityEngine.Texture2D tex2d)
        {
            if (!Directory.Exists(SaveDirectory)) { AssetDatabase.CreateFolder("Assets", "TexTransToolGenerates"); }
            var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(SaveDirectory, FilteringInvalidChars(tex2d.name) + ".png"));
            File.WriteAllBytes(path, tex2d.EncodeToPNG());
            return path;
        }

        internal static string FilteringInvalidChars(string str)
        {
            return string.Join("_", str.Split(Path.GetInvalidFileNameChars()));
        }

        internal static string SaveBinary(string name, byte[] sQLiteData)
        {
            if (!Directory.Exists(SaveDirectory)) { AssetDatabase.CreateFolder("Assets", "TexTransToolGenerates"); }
            var path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(SaveDirectory, FilteringInvalidChars(name)));
            File.WriteAllBytes(path, sQLiteData);
            return path;
        }
    }
}
