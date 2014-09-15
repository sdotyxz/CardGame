using Core.Src.Module.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Demo
{
    public class DemoModule:BaseModule
    {
        new public static string NAME = "DemoModule";
        public DemoModule():base(NAME){}

        protected override void _Start()
        {
            _RegisterMediator(new DemoMediator());
        }

        protected override void _Dispose()
        {
            _RemoveMediator();
        }
    }
}
