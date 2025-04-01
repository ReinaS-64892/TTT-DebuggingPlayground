#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using net.rs64.TexTransTool.TextureAtlas.FineTuning;
using System.Linq;
using net.rs64.TexTransTool.Utils;
namespace net.rs64.TexTransTool.DebuggingPlayground
{

    public class ShowTextureFineTuningsOrder : MonoBehaviour
    {

        [ContextMenu("Show!!!")]
        void Show()
        {
            var applicantList = InterfaceUtility.GetInterfaceInstance<ITuningProcessor>().ToList();
            applicantList.Sort((L, R) => L.Order - R.Order);

            Debug.Log(string.Join("\n", applicantList.Select(a => $" order-{a.Order} : {a.GetType().Name} ")));
        }
    }

}
#endif
