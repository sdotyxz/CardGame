using Core.Src.Config;
using Core.Src.Manager;
using Core.Src.Module.Demo;
using Core.Src.Module.Pattern;
using Helper.Config;
using Helper.Loader;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Src.Module.Initialize
{
    public class InitializeModule : BaseModule
    {
        new public static string NAME = "InitializeModule";
        public InitializeModule() : base(NAME) { }

        protected override void _Start()
        {
            Application.targetFrameRate = UIConfig.instance.frameRate;
            Input.multiTouchEnabled = false;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
            EventManager.instance.Init();
            SocketManager.instance.Init();
            //HttpManager.instance.Init(UIConfig.instance.ptUrl);
            ProtocolManager.instance.Init(System.Reflection.Assembly.GetExecutingAssembly());
            
            _InitData();
        }

        private void _InitData()
        {
            IBundleLoader loader = Engine.bundleManager.Get(ResConfig.CONFIG);
            if(loader != null)
            {
                string text = loader.GetText(ResConfig.OT);
                if (!string.IsNullOrEmpty(text)) OTManager.instance.AddOT(text);
            }
            
            _InitManagers();
            
            ModuleManager.instance.GotoModule(new DemoModule());
        }

        private void _InitManagers()
        {
            
        }

        protected override void _Dispose()
        {
            
        }
    }
}
