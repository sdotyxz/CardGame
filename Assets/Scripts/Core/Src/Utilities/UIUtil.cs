using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Src.Utilities
{
    public class UIUtil
    {
        public static T GetChildByName<T>(GameObject go, string name) where T:Component
        {
            foreach (Transform t in go.transform)
            {
                if (t.gameObject.name == name)
                {
                    T c = t.GetComponent<T>();
                    if (c != null) return c;
                }
            }
            return null;
            T[] cs = go.GetComponentsInChildren<T>(true);
            if (cs != null)
            {
                foreach (T c in cs)
                {
                    if (c.gameObject.name == name) return c;
                }
            }
            return null;
        }

        public static T GetChildByName<T>(GameObject go, string name, bool deep) where T : Component
        {
            T[] cs = go.GetComponentsInChildren<T>(true);
            if (cs != null)
            {
                foreach (T c in cs)
                {
                    if (c.gameObject.name == name) return c;
                }
            }
            return null;
        }
    }
}
