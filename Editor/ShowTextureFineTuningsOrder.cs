using UnityEngine;
using UnityEditor;
using net.rs64.TexTransTool.TextureAtlas.FineTuning;
using System.Linq;
using net.rs64.TexTransTool.Utils;
using System.Collections.Generic;
namespace net.rs64.TexTransTool.DebuggingPlayground
{
    internal class ShowTextureFineTuningsOrder : TTTMenu.ITTTMenuWindow
    {
        private List<ITuningProcessor> applicantList;

        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new ShowTextureFineTuningsOrder());
        }
        public string MenuName => "ShowTextureFineTuningsOrder";


        public void OnGUI()
        {
            if (applicantList is null)
            {
                applicantList = InterfaceUtility.CreateConcreteAssignableTypeInstances<ITuningProcessor>().ToList();
                applicantList.Sort((L, R) => L.Order - R.Order);
            }
            foreach (var a in applicantList)
                GUILayout.Label($"order - {a.Order} : {a.GetType().Name} ");
        }

    }

}
