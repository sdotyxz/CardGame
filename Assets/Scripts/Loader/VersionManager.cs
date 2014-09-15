using Helper.Config;
using System;
using System.Collections;
using UnityEngine;

public class VersionManager
{
    private VersionConfig _config;
    public VersionConfig config { get { return _config; } }

    private static VersionManager _instance = new VersionManager();
    public static VersionManager instance { get { return _instance; } }
    private VersionManager()
    {
        if (_instance != null)
        {
            throw new UnityException("Error: Please use instance to get VersionManager.");
        }
    }

    public void Init(Action onComplete, Action onError)
    {
        Main.instance.StartCoroutine(_LoadConfig(onComplete, onError));
    }

    IEnumerator _LoadConfig(Action onComplete, Action onError)
    {
        WWW loader = new WWW(Main.HOST + Main.CFG_PATH);
        yield return loader;

        if (string.IsNullOrEmpty(loader.error))
        {
            _config = JsonFx.Json.JsonReader.Deserialize<VersionConfig>(loader.text);
            if (onComplete != null) onComplete();
        }
        else
        {
            if (onError != null) onError();
        }
    }

    public VersionBundle GetVersion(string id)
    {
        if (_config == null)
        {
            Debug.LogError("VersionManager:GetVersion - Not exist VersionConfig");
            return null;
        }
        foreach (VersionBundle vb in _config.bundles)
        {
            if (vb.id == id) return vb;
        }
        return null;
    }
}