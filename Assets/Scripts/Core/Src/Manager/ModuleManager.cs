using Core.Src.Module.Interface;
using Core.Src.Module.Pattern;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Src.Manager
{
    public class ModuleManager:INotifier
    {
        public bool canGotoSameModule = false;

        private IModule _currentModule;
        public IModule currentModule { get { return _currentModule; } }
        private List<IModule> _additionalModules = new List<IModule>(); 
        
        private static ModuleManager _instance;
        public static ModuleManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModuleManager();
                }
                return _instance;
            }
        }

        private ModuleManager()
        {
            if (_instance != null)
            {
                throw new UnityException("Error: Please use instance to get ModuleManager!");
            }
        }

        public void GotoModule(IModule module)
        {
            GotoModule(module, null, null);
        }

        public void GotoModule(IModule module, object data)
        {
            GotoModule(module, data, null);
        }

        public void GotoModule(IModule module, object data, Action initComplete)
        {
            if (_currentModule != null)
            {
                if (!canGotoSameModule && _currentModule.moduleName == module.moduleName)
                {
                    Log.Error("Don't goto the same module twice.");
                    return;
                }
            }

            if (_currentModule != null) _currentModule.Destroy();

            _currentModule = module;
            _currentModule.Init(data, initComplete);
        }

        public bool ExistAdditionalModule(string moduleName)
        {
            return _additionalModules.Exists(m => m.moduleName == moduleName);
        }

        public void AddAdditionalModule(IModule module, object data = null)
        {
            if (_additionalModules.Exists(m => m.moduleName == module.moduleName))
            {
                Log.Error("Don't add the same addtional module twice.");
                return;
            }
            _additionalModules.Add(module);
            module.Init(data, null);
        }

        public void RemoveAdditionalModule(string moduleName)
        {
            IModule module = _additionalModules.Find(m => m.moduleName == moduleName);
            if (module == null)
            {
                Log.Error("Don't remove the not exist addtional module.");
                return;
            }
            module.Destroy();
            _additionalModules.Remove(module);
        }

        public void SendNotification(string notificationName)
        {
            _Notify(new Notification(notificationName));
        }
        
        public void SendNotification(string notificationName, object body)
        {
            _Notify(new Notification(notificationName, body));
        }

        public void SendNotification(string notificationName, object body, string type)
        {
            _Notify(new Notification(notificationName, body, type));
        }

        private void _Notify(INotification notification)
        {
            if (currentModule != null) currentModule.Notify(notification);
            _additionalModules.ForEach(m => m.Notify(notification));
        }

        public IModule RetrieveModule(string moduleName)
        {
            IModule module = null;
            if (currentModule != null && currentModule.moduleName == moduleName)
            {
                module = currentModule;
            }
            else
            {
                module = _additionalModules.Find(m => m.moduleName == moduleName);
            }
            return module;
        }

        public IProxy RetrieveProxy(string moduleName)
        {
            IModule module = RetrieveModule(moduleName);
            if (module != null) return module.proxy;
            return null;
        }

        public IMediator RetrieveMediator(string moduleName)
        {
            IModule module = RetrieveModule(moduleName);
            if (module != null) return module.mediator;
            return null;
        }
    }
}
