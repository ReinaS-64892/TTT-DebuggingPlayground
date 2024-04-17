#if UNITY_EDITOR
using UnityEngine;

namespace net.rs64.TTTDebuggingPlayground.TTT
{

    internal abstract class BlendingReversEngrailingBase : MonoBehaviour
    {
        public Texture2D Texture2D;
        public Texture2D BlendingResult;

        [ContextMenu("BlendingTest")]
        public void TestAt()
        {
            if (!Texture2D.isReadable) { Debug.Log("Textureが読めません!!!"); return; }
            if (Texture2D.width != 256 || Texture2D.height != 256) { Debug.Log("既定のサイズにして!!!"); return; }
            if (BlendingResult == null) { BlendingResult = new Texture2D(256, 256, TextureFormat.RGBA32, false); }

            var diff = 0f;

            var data = Texture2D.GetPixels();
            for (int y = 0; 256 > y; y += 1)
                for (int x = 0; 256 > x; x += 1)
                {
                    var i = x + (y * 256);
                    var val = data[i].r;
                    var revVal = Render(x / (float)255, y / (float)255);

                    diff += Mathf.Abs(val - revVal);
                    data[i].r = data[i].g = data[i].b = revVal;
                }

            BlendingResult.SetPixels(data);
            BlendingResult.Apply();

            var diffAv = diff / data.Length;
            Debug.Log("Diff average" + diffAv);
        }

        abstract public float Render(float x, float y);

    }
}
#endif
