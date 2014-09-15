using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Interface
{
    public interface IProxy
    {
        object data { get; set; }
        void OnRegister();
        void OnRemove();
    }
}
