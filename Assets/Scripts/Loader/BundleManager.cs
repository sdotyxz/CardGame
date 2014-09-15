using Helper.Config;
using Helper.Loader;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BundleManager:IBundleManager
{
    private static IBundleManager _instance = new BundleManager();
    public static IBundleManager instance { get { return _instance; } }

    private BundleManager()
    {
        if (_instance != null)
        {
            throw new UnityException("Error: Please use instance to get BundleManager.");
        }
    }

    private Dictionary<string, IBundleLoader> _als = new Dictionary<string, IBundleLoader>();
    public IBundleLoader Get(string id)
    {
        if (_als.ContainsKey(id))
        {
            return _als[id];
        }
        return null;
    }

    public IBundleLoader GetByContentName(string assetsBundleContentName)
    {
        foreach (IBundleLoader al in _als.Values)
        {
            if (al.bundle != null && al.bundle.Contains(assetsBundleContentName))
            {
                return al;
            }
        }
        return null;
    }

    public IDictionary<string, IBundleLoader> GetAllBundleLoader()
    {
        return _als;
    }

    public List<IBundleLoader> Loads(List<VersionBundle> items, Action loadComplete, Action loadError)
    {
        List<IBundleLoader> lal = new List<IBundleLoader>();
        int count = items.Count;
        bool existError = false;
        foreach (VersionBundle v in items)
        {
            IBundleLoader al = Load(v, () =>
            {
                count--;
                if (count <= 0)
                {
                    if (existError && loadError != null) loadError();
                    else if (!existError && loadComplete != null) loadComplete();
                }
            }, () =>
            {
                count--;
                existError = true;
                if (count <= 0 && loadError != null) loadError();
            });
            lal.Add(al);
        }
        return lal;
    }

    public IBundleLoader Load(VersionBundle item, Action loadComplete, Action loadError)
    {
        if (item == null)
        {
            Debug.LogError("Not found the item");
            return null;
        }

        if (_als.ContainsKey(item.id))
        {
            Debug.LogError("Don't load the same item - " + item.id);
            return _als[item.id];
        }

        _als[item.id] = new BundleLoader(item);
        _als[item.id].Load(loadComplete, loadError);

        return _als[item.id];
    }

    public void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
    }

    public void Unloads(List<string> ids, bool unloadAllLoadedObjects)
    {
        foreach (string id in ids)
        {
            Unload(id, unloadAllLoadedObjects);
        }
    }

    public void Unload(string id, bool unloadAllLoadedObjects)
    {
        if (_als.ContainsKey(id))
        {
            _als[id].Unload(unloadAllLoadedObjects);
            _als.Remove(id);
        }
        else
        {
            Debug.LogError("Error: Not exist IBundleLoader about " + id);
        }
    }
}
