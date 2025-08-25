#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using net.rs64.TexTransTool;
using net.rs64.TexTransTool.MultiLayerImage.Importer;
using net.rs64.TexTransTool.MultiLayerImage;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace net.rs64.TexTransTool.DebuggingPlayground
{
    internal class  ImportedImageAllGenerate : TTTMenu.ITTTMenuWindow
    {
        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new ImportedImageAllGenerate());
        }
        public string MenuName => "ImportedImageAllGenerate";

        public void OnGUI()
        {
            if (GUILayout.Button("Do ImportedImageAllGenerate"))
            {
                Test();
            }
        }
        public async void Test()
        {
            CanvasImportedImagePreviewManager.InvalidatesCacheAll();
            foreach (var canvasGroup in Resources.FindObjectsOfTypeAll(typeof(TTTImportedImage)).Cast<TTTImportedImage>().GroupBy(i => i.CanvasDescription))
            {
                GC.Collect();
                await Task.Delay(250);

                var cs = canvasGroup.Key.LoadCanvasSource(AssetDatabase.GetAssetPath(canvasGroup.Key));
                foreach (var i in canvasGroup)
                {
                    CanvasImportedImagePreviewManager.CreatePreviewImageWithCache(i, cs);
                }

            }
        }
    }

}
#endif
