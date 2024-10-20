#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering;
namespace net.rs64.TexTransTool.DebuggingPlayground
{

    public class MaterialView : MonoBehaviour
    {
        public Material Material;
    }


    [CustomEditor(typeof(MaterialView))]
    public class MaterialViewEditor : UnityEditor.Editor
    {
        bool Tex;
        bool ShowNullTex;
        bool Float;
        bool Color;
        bool Vector;
        bool Range;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var material = (target as MaterialView)?.Material;
            if (material == null) { return; }
            var shader = material.shader;

            Tex = EditorGUILayout.Foldout(Tex, "Tex");
            if (Tex)
            {
                ShowNullTex = EditorGUILayout.Toggle("ShowNullTex", ShowNullTex);
                foreach (var pi in GetProperty(ShaderPropertyType.Texture))
                {
                    var tex = material.GetTexture(pi);
                    if ((tex == null) != ShowNullTex) { continue; }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(pi);
                    EditorGUILayout.ObjectField(tex, typeof(Texture), false);
                    EditorGUILayout.EndHorizontal();
                }
            }

            Float = EditorGUILayout.Foldout(Float, "Float");
            if (Float)
            {
                foreach (var pi in GetProperty(ShaderPropertyType.Float))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(pi);
                    EditorGUILayout.FloatField(material.GetFloat(pi));
                    EditorGUILayout.EndHorizontal();
                }
            }

            Color = EditorGUILayout.Foldout(Color, "Color");
            if (Color)
            {
                foreach (var pi in GetProperty(ShaderPropertyType.Color))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(pi);
                    EditorGUILayout.ColorField(material.GetColor(pi));
                    EditorGUILayout.EndHorizontal();
                }
            }

            Vector = EditorGUILayout.Foldout(Vector, "Vector");
            if (Vector)
            {
                foreach (var pi in GetProperty(ShaderPropertyType.Vector))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(pi);
                    EditorGUILayout.Vector4Field(GUIContent.none, material.GetVector(pi));
                    EditorGUILayout.EndHorizontal();
                }
            }

            Range = EditorGUILayout.Foldout(Range, "Range");
            if (Range)
            {
                foreach (var pi in GetPropertyRanges())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(pi.prop);
                    EditorGUILayout.Slider(material.GetFloat(pi.prop), pi.range.min, pi.range.max);
                    EditorGUILayout.EndHorizontal();
                }
            }

            IEnumerable<string> GetProperty(ShaderPropertyType shaderPropertyType)
            {
                var count = shader.GetPropertyCount();
                for (var i = 0; count > i; i += 1)
                {
                    if (shader.GetPropertyType(i) == shaderPropertyType)
                    {
                        yield return shader.GetPropertyName(i);
                    }
                }
            }

            IEnumerable<(string prop, (float min, float max) range)> GetPropertyRanges()
            {
                var count = shader.GetPropertyCount();
                for (var i = 0; count > i; i += 1)
                {
                    if (shader.GetPropertyType(i) == ShaderPropertyType.Range)
                    {
                        var range = shader.GetPropertyRangeLimits(i);
                        yield return (shader.GetPropertyName(i), (range.x, range.y));
                    }
                }
            }

        }
    }


}
#endif
