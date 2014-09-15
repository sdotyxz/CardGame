using Core.Src.Config;
using Core.Src.Module.Demo.View;
using Core.Src.Module.Pattern;
using Core.Src.Utilities;
using System;
using System.Collections.Generic;

namespace Core.Src.Module.Demo
{
    public class DemoMediator:Mediator
    {
        public override void OnRegister()
        {
            Pool.GetUI<DemoPanel>(ResConfig.GUI_DEMO, true);
        }

        public override void OnRemove()
        {
            
        }
    }
}
