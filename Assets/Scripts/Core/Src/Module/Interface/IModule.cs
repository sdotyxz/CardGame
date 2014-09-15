using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Interface
{
    public interface IModule
    {
        void Init();
        void Init(object data);
        void Init(object data, Action initComplete);
        void Destroy();

        void Notify(INotification notification);

        IProxy proxy { get; }
        IMediator mediator { get; }
        string moduleName { get; }
    }
}
