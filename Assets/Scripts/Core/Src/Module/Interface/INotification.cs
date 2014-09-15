using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Interface
{
    public interface INotification
    {
        string name { get; }
        object body { get; set; }
        string type { get; set; }
    }
}
