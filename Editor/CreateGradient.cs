#if UNITY_EDITOR
using System.IO;
using Unity.Collections;
using UnityEngine;

namespace net.rs64.TexTransTool.DebuggingPlayground
{

    internal  class CreateGradient : MonoBehaviour
    {
        public bool IsX = true;
        public Color ColMin;
        public Color ColMax;
        [ContextMenu("CreateGradient")]
        public void TestAt()
        {
            var tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            var data = new Color[256 * 256];
            for (int y = 0; 256 > y; y += 1)
                for (int x = 0; 256 > x; x += 1)
                    data[x + y * 256] = Color.Lerp(ColMin, ColMax, (IsX ? x : y) / (float)256);


            tex.SetPixels(data);
            tex.Apply();
            File.WriteAllBytes("Assets/Grad.png", tex.EncodeToPNG());
        }


    }
}
#endif
