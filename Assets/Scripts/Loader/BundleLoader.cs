using Helper.Config;
using Helper.Loader;
using System;
using System.Collections;
using UnityEngine;

public class BundleLoader:IBundleLoader
{
    public const string ENCRYPTED_DATA = "encryptedData";

    private WWW _loader;
    private VersionBundle _versionBundle;
    public VersionBundle versionBundle { get { return _versionBundle; } }

    private AssetBundle _bundle;
    public AssetBundle bundle { get { return _bundle; } }

    private bool _isLoadComplete = false;
    public bool isLoadComplete { get { return _isLoadComplete; } }

    public BundleLoader(VersionBundle item)
    {
        _versionBundle = item;
    }

    public void Unload(bool unloadAllLoadedObjects)
    {
        if (_bundle != null)
        {
            _bundle.Unload(unloadAllLoadedObjects);
            _bundle = null;
        }
        _loader = null;
    }

    public void Load(Action loadComplete, Action loadError)
    {
        Main.instance.StartCoroutine(_LoadAssets(loadComplete, loadError));
    }

    IEnumerator _LoadAssets(Action loadComplete, Action loadError)
    {
        if (Debug.isDebugBuild)
            _loader = new WWW(Main.HOST + _versionBundle.url);
        else
            _loader = WWW.LoadFromCacheOrDownload(Main.HOST + _versionBundle.url, _versionBundle.versionValue);
        yield return _loader;

        if (string.IsNullOrEmpty(_loader.error))
        {
            if (_versionBundle.isEncrypted)
            {
                TextAsset encryped = _loader.assetBundle.Load(BundleLoader.ENCRYPTED_DATA, typeof(TextAsset)) as TextAsset;
                byte[] encryptedData = encryped.bytes;
                byte[] decryptedData = _Decrypt(encryptedData);
                _bundle = AssetBundle.CreateFromMemory(decryptedData).assetBundle;
            }
            else
            {
                _bundle = _loader.assetBundle;
            }

            _isLoadComplete = true;

            if (loadComplete != null) loadComplete();
        }
        else
        {
            Debug.LogError("Error: Load asset error with url: " + Main.HOST + _versionBundle.url + " - " + _loader.error);
            if (loadError != null) loadError();
        }
    }

    public float GetProgress()
    {
        if (_loader != null)
        {
            return _loader.progress;
        }
        return 0f;
    }

    public int GetSize()
    {
        if (_loader != null)
        {
            return _loader.size;
        }
        return 0;
    }

    public int GetBytesDownloaded()
    {
        if (_loader != null)
        {
            return _loader.bytesDownloaded;
        }
        return 0;
    }

    public byte[] GetBytes()
    {
        if (_loader != null)
        {
            return _loader.bytes;
        }
        return null;
    }

    public string GetText()
    {
        if (_loader != null)
        {
            return _loader.text;
        }
        return null;
    }

    public UnityEngine.Object GetAsset(string name, Type type)
    {
        if (_bundle != null && !string.IsNullOrEmpty(name))
        {
            return _bundle.Load(name, type);
        }
        return null;
    }

    public GameObject GetPrefab(string name)
    {
        if (_bundle != null && !string.IsNullOrEmpty(name))
        {
            return _bundle.Load(name, typeof(GameObject)) as GameObject;
        }
        return null;
    }

    public GameObject GetGameObject(string name)
    {
        GameObject prefab = GetPrefab(name);
        if (prefab != null)
        {
            return GameObject.Instantiate(prefab) as GameObject;
        }
        return null;
    }

    public void GetPrefabAsync(string name, Action<GameObject> onComplete)
    {
        Main.instance.StartCoroutine(_GetPrefabAsync(name, onComplete));
    }

    IEnumerator _GetPrefabAsync(string name, Action<GameObject> onComplete)
    {
        AssetBundleRequest request = _bundle.LoadAsync(name, typeof(GameObject));
        yield return request;
        if (onComplete != null)
        {
            onComplete(request.asset as GameObject);
            onComplete = null;
        }
    }

    public byte[] GetBytes(string name)
    {
        if (_bundle != null && !string.IsNullOrEmpty(name))
        {
            TextAsset asset = _bundle.Load(name, typeof(TextAsset)) as TextAsset;
            if (asset != null) return asset.bytes;
        }
        return null;
    }

    public string GetText(string name)
    {
        if (_bundle != null && !string.IsNullOrEmpty(name))
        {
            TextAsset asset = _bundle.Load(name, typeof(TextAsset)) as TextAsset;
            return asset.text;
        }
        return null;
    }

    public Type GetClass(string name, string assetName)
    {
        byte[] bytes = GetBytes(assetName);
        if (bytes != null)
        {
            try
            {
                var assembly = System.Reflection.Assembly.Load(bytes);
                Type type = assembly.GetType(name);
                return type;
            }
            catch (Exception error)
            {
                Debug.LogError("Error: " + error.Message);
            }
        }
        return null;
    }

    private byte[] _Decrypt(byte[] bytes)
    {
        return bytes;
    }
}