#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
using net.rs64.TexTransTool;
using net.rs64.TexTransCore.Utils;
using net.rs64.TexTransTool.TextureAtlas.FineTuning;
using System.Linq;
using System;
namespace net.rs64.TTTDebuggingPlayground.TTT
{

    public class ShowTextureFineTuningsOrder : MonoBehaviour
    {

        [ContextMenu("Show!!!")]
        void Show()
        {
            var applicantList = InterfaceUtility.GetInterfaceInstance<ITuningApplicant>().ToList();
            applicantList.Sort((L, R) => L.Order - R.Order);

            Debug.Log(string.Join("\n", applicantList.Select(a => $" order-{a.Order} : {a.GetType().Name} ")));
        }
    }

}
#endif
