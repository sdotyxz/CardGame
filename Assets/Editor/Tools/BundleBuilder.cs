using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class BundleBuilder : EditorWindow 
{
    private BuildTarget _buildTarget = BuildTarget.Android;
    private string _bundleExt = ".bundle";

    private string _assetsRoot = Application.dataPath;
    private string _bundleRoot = "/StreamingAssets/";
    private string _bundleResRoot = "/Resources/";
    private string _bundleDll = "DLL";
    private string _bundleGUI = "GUI";
    private string _bundleCFG = "CONFIG";

    [MenuItem("Tools/BundleBuilder")]
    private static void OpenBundleBuilder()
    {
        BundleBuilder window = (BundleBuilder)EditorWindow.GetWindow(typeof(BundleBuilder));
        window.Show();
    }

    void OnGUI()
    {
        _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget: ", _buildTarget);

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("打包DLL资源", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _CreateAssetBundle(_assetsRoot + _bundleResRoot + _bundleDll + "/", _assetsRoot + _bundleRoot + _bundleDll + _bundleExt);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("打包GUI资源", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _CreateAssetBundle(_assetsRoot + _bundleResRoot + _bundleGUI + "/", _assetsRoot + _bundleRoot + _bundleGUI + _bundleExt);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("打包CONFIG资源", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _CreateAssetBundle(_assetsRoot + _bundleResRoot + _bundleCFG + "/", _assetsRoot + _bundleRoot + _bundleCFG + _bundleExt);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("打包所有资源", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _CreateAssetBundle(_assetsRoot + _bundleResRoot + _bundleDll + "/", _assetsRoot + _bundleRoot + _bundleDll + _bundleExt);
            _CreateAssetBundle(_assetsRoot + _bundleResRoot + _bundleGUI + "/", _assetsRoot + _bundleRoot + _bundleGUI + _bundleExt);
            _CreateAssetBundle(_assetsRoot + _bundleResRoot + _bundleCFG + "/", _assetsRoot + _bundleRoot + _bundleCFG + _bundleExt);
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void _CreateAssetBundle(string srcPath, string destPath)
    {
        _CreateAssetBundle(new string[] { srcPath }, destPath);
    }

    private static bool _IsDir(string path)
    {
        FileInfo fi = new FileInfo(path);
        if ((fi.Attributes & FileAttributes.Directory) != 0)
            return true;
        else
        {
            return false;
        }
    }

    public static void GetAllFiles(string dir, List<string> paths)
    {
        if (_IsDir(dir))
        {
            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                if (!file.Contains(".meta")) paths.Add(file);
            }
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string d in dirs) GetAllFiles(d, paths);
        }
        else
        {
            paths.Add(dir);
        }
    }

    private void _CreateAssetBundle(string[] srcPaths, string destPath)
    {
        List<string> paths = new List<string>();
        foreach (string srcPath in srcPaths)
        {
            GetAllFiles(srcPath, paths);
        }
        List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
        foreach (string path in paths) objects.Add(AssetDatabase.LoadAssetAtPath(path.Replace(_assetsRoot, "Assets"), typeof(UnityEngine.Object)));
        
        if (_buildTarget == BuildTarget.iPhone || _buildTarget == BuildTarget.Android || _buildTarget == BuildTarget.WP8Player)
        {
            BuildPipeline.BuildAssetBundle(objects[0], objects.ToArray(), destPath,
                BuildAssetBundleOptions.CollectDependencies |
                BuildAssetBundleOptions.CompleteAssets |
                BuildAssetBundleOptions.DeterministicAssetBundle, _buildTarget);
        }
        else
        {
            BuildPipeline.BuildAssetBundle(objects[0], objects.ToArray(), destPath,
                BuildAssetBundleOptions.CollectDependencies |
                BuildAssetBundleOptions.CompleteAssets |
                BuildAssetBundleOptions.DeterministicAssetBundle);
        }
    }
}