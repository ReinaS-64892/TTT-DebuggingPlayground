#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace net.rs64.TexTransTool.DebuggingPlayground
{
    internal class TransformCopyByPath : TTTMenu.ITTTMenuWindow
    {
        private Transform? source;
        private Transform? destination;

        [InitializeOnLoadMethod]
        static void Registering()
        {
            DebuggingPlaygroundMenu.RegisterMenu(new TransformCopyByPath());
        }
        public string MenuName => "TransformCopyByPath";

        public void OnGUI()
        {
            source = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), "source", source, typeof(Transform), true) as Transform;
            destination = EditorGUI.ObjectField(EditorGUILayout.GetControlRect(), "destination", destination, typeof(Transform), true) as Transform;

            if (source == null || destination == null) { return; }
            if (GUILayout.Button("Copy!"))
            {
                CopyReclusive(destination, source);
            }

            void CopyReclusive(Transform d, Transform s)
            {
                Undo.RecordObject(d, "copy transform values");
                d.localPosition = s.localPosition;
                d.localRotation = s.localRotation;
                d.localScale = s.localScale;

                var intersectedTransformers = EnumerateChild(d).Select(i => i.gameObject.name).Intersect(EnumerateChild(s).Select(i => i.gameObject.name));

                foreach (var name in intersectedTransformers)
                {
                    var dc = d.Find(name);
                    var sc = s.Find(name);

                    CopyReclusive(dc, sc);
                }
            }
            static IEnumerable<Transform> EnumerateChild(Transform d)
            {
                for (var i = 0; d.childCount > i; i += 1)
                {
                    yield return d.GetChild(i);
                }
            }
        }
    }
}
