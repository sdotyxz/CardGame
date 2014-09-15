using Core.Src.Module.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Pattern
{
    public class SimpleCommand : Notifier, ICommand, INotifier
    {
        public virtual void Execute(INotification notification)
        {
        }
    }
}
