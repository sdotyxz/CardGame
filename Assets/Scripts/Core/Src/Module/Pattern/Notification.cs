using Core.Src.Module.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Pattern
{
    public class Notification : INotification
    {
        public Notification(string name)
            : this(name, null, null)
        { }

        public Notification(string name, object body)
            : this(name, body, null)
        { }

        public Notification(string name, object body, string type)
        {
            _name = name;
            _body = body;
            _type = type;
        }

        public virtual string name
        {
            get { return _name; }
        }

        public virtual object body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
            }
        }

        public virtual string type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        private string _name;
        private string _type;
        private object _body;
    }
}
