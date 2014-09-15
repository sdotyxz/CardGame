using Helper.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static string HOST { get { return _instance.host; } }
    public static string CFG_PATH { get { return _instance.config; } }

    private static Main _instance;
    public static Main instance { get { return _instance; } }

    public bool isLocalVersion = false;
    public string host = "http://192.168.1.22/uproject/";
    public string config = "config.txt";

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);

            if (isLocalVersion)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        host = "jar:file://" + Application.dataPath + "!/assets/";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        host = "file://" + Application.dataPath + "/Raw/";
                        break;
                    case RuntimePlatform.WP8Player:
                        host = "file:///" + Application.streamingAssetsPath;
                        break;
                    case RuntimePlatform.OSXEditor:
                        host = "file://" + Application.dataPath + "/StreamingAssets/";
                        break;
                    case RuntimePlatform.WindowsEditor:
                        host = "file:///" + Application.dataPath + "/StreamingAssets/";
                        break;
                    default:
                        host = "file:///" + Application.dataPath + "/StreamingAssets/";
                        break;
                }
            }
            host = host.Replace(" ", "%20");
        }
    }

    void Start()
    { 
        //Game init
        VersionManager.instance.Init(_OnVersionComplete, _OnVersionError);
    }

    private void _OnVersionError()
    {
        //Alert and Quit
        Application.Quit();
    }

    private void _OnVersionComplete()
    {
        BundleManager.instance.Loads(new List<VersionBundle>(VersionManager.instance.config.bundles), _OnBundleComplete, _OnBundleError);
    }

    private void _OnBundleError()
    {
        //Alert and Quit
        Application.Quit();
    }

    private void _OnBundleComplete()
    {
#if UNITY_EDITOR
        Core.Src.Engine engine = gameObject.AddComponent<Core.Src.Engine>();
        engine.Init(BundleManager.instance);
#else
        if (Debug.isDebugBuild)
        {
            Core.Src.Engine engine = gameObject.AddComponent<Core.Src.Engine>();
            engine.Init(BundleManager.instance);
        }
        else
        {
            Type type = BundleManager.instance.Get("DLL").GetClass("Core.Src.Engine", "Core");
            Component engine = gameObject.AddComponent(type);
            System.Reflection.MethodInfo method = type.GetMethod("Init");
            method.Invoke(engine, new object[] { BundleManager.instance });
        }
#endif
    }
}
