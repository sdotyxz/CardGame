using Core.Src.Interface;
using Core.Src.Manager;
using Core.Src.Module.Initialize;
using Helper.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Src
{
    /// <summary>
    /// Engine. Contains all the shared components.
    /// </summary>
    public class Engine : MonoBehaviour
    {
        public static bool isQuitting = false;

        private static Engine _instance;
        public static Engine instance { get { return _instance; } }

        private static IBundleManager _bundleManager;
        public static IBundleManager bundleManager { get { return _bundleManager; } }

        #region TickObjectQueue
        private List<ITickObject> _tickObjects = new List<ITickObject>();
        public static void AddTickObject(ITickObject tickObject)
        {
            if (tickObject == null)
            {
                Log.Warning("Warning: Don't add the null object to tick object list");
                return;
            }
            if (_instance._tickObjects.Exists(t => t == tickObject))
            {
                Log.Warning("Warning: Don't add the same tick object twice.");
                return;
            }
            _instance._tickObjects.Add(tickObject);
        }

        public static void RemoveTickObject(ITickObject tickObject)
        {
            if (tickObject == null)
            {
                Log.Warning("Warning: Can not remove the null object from the tick object list");
                return;
            }
            if (!_instance._tickObjects.Remove(tickObject))
            {
                Log.Warning("Warning: Remove tick object error. May be the tick object is not in list.");
            }
        }
        #endregion

        public void Init(IBundleManager bundleManager)
        {
            _bundleManager = bundleManager;
        }

        /// <summary>
        /// Save the first and only instance
        /// </summary>
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
            }
        }

        void Start()
        {
            ModuleManager.instance.GotoModule(new InitializeModule());
        }

        /// <summary>
        /// Update Game (None MonoBehaviour Partition.)
        /// </summary>
        protected void Update()
        {
            _tickObjects.ForEach(tickObject => tickObject.Update());
        }

        void OnApplicationQuit()
        {
            isQuitting = true;
        }

        public void DoAfter(int frame, Action action)
        {
            StartCoroutine(_DoAfter(frame, action));
        }

        public void DoAfter(float time, Action action)
        {
            StartCoroutine(_DoAfter(time, action));
        }

        public IEnumerator _DoAfter(float time, Action onComplete)
        {
            yield return new WaitForSeconds(time);
            if (onComplete != null) onComplete();
        }

        public IEnumerator _DoAfter(int frame, Action onComplete)
        {
            for (int i = 0; i < frame; i++) yield return null;
            if (onComplete != null) onComplete();
        }
    }
}
