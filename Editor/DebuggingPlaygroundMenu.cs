#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using net.rs64.TexTransCoreEngineForUnity;
using net.rs64.TexTransTool.Build;
using net.rs64.TexTransTool.Migration;
using net.rs64.TexTransTool.MultiLayerImage.Importer;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

namespace net.rs64.TexTransTool
{
    internal sealed class DebuggingPlaygroundMenu : TTTMenu.ITTTMenuWindow
    {
        public string MenuName => "DebuggingPlayground";
        [InitializeOnLoadMethod]
        static void Registering()
        {
            TTTMenu.RegisterMenu(new DebuggingPlaygroundMenu());
        }
        static List<DebuggerState> _menus = new();
        static bool _dirty = false;

        public void OnGUI()
        {
            EditorGUILayout.HelpBox("This is a Debug Menu !!!, If you do not understand it, do not touch this menu !!!\nこれはデバッグメニューです!!! 理解できないのであればこのメニューは使用しないでください!!!", MessageType.Warning);
            if (_dirty) { _dirty = false; _menus = _menus.OrderBy(m => m.Debugger.MenuName).ToList(); }
            foreach (var debuggerState in _menus)
            {
                debuggerState.Foldout = EditorGUILayout.Foldout(debuggerState.Foldout, debuggerState.Debugger.MenuName);
                if (debuggerState.Foldout)
                    debuggerState.Debugger.OnGUI();
            }
        }
        internal static void RegisterMenu(TTTMenu.ITTTMenuWindow menuWindow)
        {
            _menus.Add(new(menuWindow, false));
            _dirty = true;
        }
    }

    internal class DebuggerState
    {
        public TTTMenu.ITTTMenuWindow Debugger;
        public bool Foldout;

        public DebuggerState(TTTMenu.ITTTMenuWindow debugger, bool foldout)
        {
            this.Debugger = debugger;
            this.Foldout = foldout;
        }
    }
}
