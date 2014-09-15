using Core.Src.Module.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Module.Pattern
{
    public class Mediator : Notifier, IMediator, INotifier
    {
        private List<string> _interestNotifications;

        public Mediator()
        {
            _interestNotifications = _ListNotificationInterests();
        }

        protected virtual List<string> _ListNotificationInterests()
        {
            return new List<string>();
        }

        public void HandleNotification(INotification notification)
        {
            if (_interestNotifications.Contains(notification.name))
            {
                _HandleNotification(notification);
            }
        }

        protected virtual void _HandleNotification(INotification notification)
        { 
            
        }

        public virtual void OnRegister()
        {
        }

        public virtual void OnRemove()
        {
        }
    }
}
