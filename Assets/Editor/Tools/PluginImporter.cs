using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PluginImporter : EditorWindow
{
    private string[] _pluginSrc = new string[] { 
        //"MsgPack/bin/Debug/MsgPack.dll",
        "NGUI/bin/Debug/NGUI.dll",
        "Helper/bin/Debug/Helper.dll",
        "UniWeb/bin/Debug/UniWeb.dll",
        "TK2D/bin/Debug/TK2D.dll"
    };

    private static string _pluginSrcRoot;
    private string _pluginDst = Application.dataPath + "/Scripts/Plugins/";

    private string _coreSrcPath = "Core/bin/Debug/Core.dll";
    private string _coreDstPath = "/Resources/DLL/Core.bytes";

    private bool _isDebug = true;
    private bool _isPC = false;

    [MenuItem("Tools/PluginImporter")]
    private static void OpenPluginImporter()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
        _pluginSrcRoot = dir.Parent.Parent.ToString() + "/Plugins/";

        PluginImporter window = (PluginImporter)EditorWindow.GetWindow(typeof(PluginImporter));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _isDebug = GUILayout.Toggle(_isDebug, "是否为Debug库");
        _isPC = GUILayout.Toggle(_isPC, "是否为PC库");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("重新导入DLL", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _ImportDLL();
            _ImportCore();
            AssetDatabase.Refresh();
        }
    }

    private void _ImportDLL()
    {
        foreach (string file in _pluginSrc)
        {
            string srcPath = _pluginSrcRoot + file;
            if (!_isDebug) srcPath = srcPath.Replace("Debug", "Release");
            if (_isPC && srcPath.Contains("NGUI.dll")) srcPath = srcPath.Replace("NGUI.dll", "NGUI_PC.dll");
            string dstPath = _pluginDst + new FileInfo(file).Name;
            if (File.Exists(srcPath))
            {
                File.Copy(srcPath, dstPath, true);
            }
            else
            {
                Debug.LogError("Not exist dll: " + srcPath);
            }
        }
    }

    private void _ImportCore()
    {
        string srcPath = _pluginSrcRoot + _coreSrcPath;
        if (!_isDebug) srcPath = srcPath.Replace("Debug", "Release");
        string dstPath = Application.dataPath + _coreDstPath;
        if (File.Exists(srcPath))
        {
            File.Copy(srcPath, dstPath, true);
        }
        else
        {
            Debug.LogError("Not exist dll: " + srcPath);
        }
    }
}