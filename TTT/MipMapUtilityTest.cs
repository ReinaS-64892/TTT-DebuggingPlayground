#if UNITY_EDITOR
// using net.rs64.TexTransCore.MipMap;
using net.rs64.TexTransCore.TransTextureCore.Utils;
using UnityEngine;
namespace net.rs64.TTTDebuggingPlayground.TTT
{

    public class MipMapUtilityTest : MonoBehaviour
    {
        public Texture2D STex;
        public Texture2D CTex;

        [ContextMenu("Average")]
        public void Average()
        {
            if (CTex != null) { DestroyImmediate(CTex); }
            var renderTexture = new RenderTexture(2048, 2048, 0, RenderTextureFormat.ARGB32);
            renderTexture.enableRandomWrite = true;
            renderTexture.useMipMap = true;
            renderTexture.autoGenerateMips = false;

            Graphics.Blit(STex, renderTexture);
            // MipMapUtility.GenerateMips(renderTexture, DownScalingAlgorism.Average);

            CTex = renderTexture.CopyTexture2D();
            CTex.alphaIsTransparency = true;
        }
    }
}
#endif
