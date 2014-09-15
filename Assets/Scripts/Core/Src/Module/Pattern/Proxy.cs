using Core.Src.Module.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Pattern
{
    public class Proxy : Notifier, IProxy, INotifier
    {
        public Proxy()
            : this(null)
        {
        }

        public Proxy(object data)
        {
            if (data != null) _data = data;
        }

        public virtual void OnRegister()
        {
        }

        public virtual void OnRemove()
        {
        }

        public virtual object data
        {
            get { return _data; }
            set { _data = value; }
        }

        protected object _data;
    }
}
