using Core.Src.Manager;
using Core.Src.Module.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Pattern
{
    public class Notifier : INotifier
    {
        public virtual void SendNotification(string notificationName)
        {
            ModuleManager.instance.SendNotification(notificationName);
        }

        public virtual void SendNotification(string notificationName, object body)
        {
            ModuleManager.instance.SendNotification(notificationName, body);
        }

        public virtual void SendNotification(string notificationName, object body, string type)
        {
            ModuleManager.instance.SendNotification(notificationName, body, type);
        }
    }
}
