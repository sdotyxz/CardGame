using Core.Src.Common;
using Core.Src.Config;
using Helper.Loader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Src.Utilities
{
    public class Pool
    {
        private static string[] PREFAB_PATH = new string[] { "GUI", "SCENE", "HERO" };
        private static string[] ASSET_PATH = new string[] { "CONFIG", "TPL" };
        
        public static T GetAsset<T>(string name) where T:UnityEngine.Object
        {
            T t = default(T);
            IBundleLoader loader = Engine.bundleManager.GetByContentName(name);
            if (loader != null)
            {
                t = (T)(object)loader.GetAsset(name, typeof(T));
            }
            if(t == null) 
            {
                foreach (string path in ASSET_PATH)
                {
                    t = Resources.Load<T>(path + "/" + name);
                    if (t != null) break;
                }
            }
            if (t == null) Log.Error("Pool - Not found asset : " + name);
            return t;
        }

        public static GameObject GetPrefab(string name)
        {
            GameObject prefab = null;
            IBundleLoader loader = Engine.bundleManager.GetByContentName(name);
            if (loader != null)
            {
                prefab = loader.GetPrefab(name);
            }
            if (prefab == null)
            {
                foreach (string path in PREFAB_PATH)
                {
                    prefab = Resources.Load(path + "/" + name) as GameObject;
                    if (prefab != null) break;
                }
            }
            if (prefab == null) Log.Error("Pool - Not found prefab : " + name);
            return prefab;
        }

        public static GameObject Get(string name, Transform parent)
        {
            return Get(GetPrefab(name), parent);
        }

        public static GameObject Get(string name, Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            return Get(GetPrefab(name), parent, localPosition, localRotation);
        }

        public static GameObject Get(string name)
        {
            return Get(GetPrefab(name), Vector3.zero, Quaternion.identity);
        }

        public static GameObject Get(string name, Vector3 position, Quaternion rotation)
        {
            return Get(GetPrefab(name), position, rotation);
        }

        public static GameObject Get(GameObject prefab, Transform parent)
        {
            return Get(prefab, parent, Vector3.zero, Quaternion.identity);
        }

        public static GameObject Get(GameObject prefab)
        {
            return Get(prefab, Vector3.zero, Quaternion.identity);
        }

        public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab != null)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.name = go.name.Replace("(Clone)", "");
                if (go != null)
                {
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.SetActive(true);
                }
                return go;
            }
            return null;
        }

        public static GameObject Get(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            if (prefab != null)
            {
                GameObject go = GameObject.Instantiate(prefab) as GameObject;
                go.name = go.name.Replace("(Clone)", "");
                if (go != null)
                {
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.transform.parent = parent;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.SetActive(true);
                }
                return go;
            }
            return null;
        }

        public static void Recycle(GameObject go)
        {
            GameObject.Destroy(go);
        }

        public static T GetComponent<T>(GameObject go) where T : Component
        {
            object t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent(typeof(T));
            }
            return (T)t;
        }

        public static T GetComponent<T>(Component component) where T : Component
        {
            return GetComponent<T>(component.gameObject);
        }

        //======================= 分割线 ===================
        public static GameObject GetUI(string name)
        {
            return Get(name, Scene.instance.uiLayer2D);
        }

        public static T GetUI<T>(string name) where T : Component
        {
            return GetUI<T>(name, false);
        }

        public static T GetUI<T>(string name, bool autoAdd) where T : Component
        {
            GameObject go = GetUI(name);
            if (go != null)
            {
                if (autoAdd) return GetComponent<T>(go);
                else return go.GetComponent<T>();
            }
            return null;
        }
    }
}
