using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Net.Sockets;
using Core.Net.Protocol;
using Core.Src.Manager;
using Core.Src.Interface;

namespace Core.Src.Manager
{
    public class ProtocolManager
    {
        private CmdHandler _sendHandler;
        private CmdHandler _receiveHandler;

        private IDictionary<int, ICommand> _commands = new Dictionary<int, ICommand>();
        private List<int> _lockList = new List<int>();

        private static ProtocolManager _instance = new ProtocolManager();
        public static ProtocolManager instance
        {
            get
            {
                return _instance;
            }
        }

        public ProtocolManager()
        {
            if (_instance != null)
            {
                throw new UnityException("Error: Please use instance to get ProtocalManager.");
            }
        }

        public void RegistHandler(CmdHandler sendHandler, CmdHandler receiveHandler)
        {
            _sendHandler = sendHandler;
            _receiveHandler = receiveHandler;
        }

        public void Init(System.Reflection.Assembly assembly)
        {
            _Inject(assembly);
        }

        private void _Inject(System.Reflection.Assembly assembly)
        {
            //Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                CmdAttribute attribute = Attribute.GetCustomAttribute(t, typeof(CmdAttribute)) as CmdAttribute;
                if (attribute != null)
                {
                    ICommand command = t.Assembly.CreateInstance(t.FullName) as ICommand;
                    if (command != null)
                    {
                        AddCommand(command);
                    }
                }
            }
        }

        public void AddCommand(ICommand command)
        {
            if (_commands.ContainsKey(command.code))
            {
                Log.Error("协议号:[" + command.code.ToString() + ", " + command.desc + "] 已存在 ");
                return;
            }
            _commands[command.code] = command;
        }

        public static void AddListener(int code, EventListener handler)
        {
            _instance.AddProtocolListent<EventListener>(code, handler);
        }

        public static void AddListener<T>(int code, T handler)
        {
            _instance.AddProtocolListent<T>(code, handler);
        }

        public void AddProtocolListent<T>(int code, T handler)
        {
            ICommand command = _GetCommand(code);

            if (command != null)
            {
                command.AddProtocolEventListent(handler as Delegate);
            }
        }

        public void AddProtocolListent(int code, EventListener handler)
        {
            AddProtocolListent<EventListener>(code, handler);
        }

        public static void RemoveListener(int code, EventListener handler)
        {
            _instance.RemoveProtocolListent<EventListener>(code, handler);
        }

        public static void RemoveListener<T>(int code, T handler)
        {
            _instance.RemoveProtocolListent<T>(code, handler);
        }

        public void RemoveProtocolListent<T>(int code, T handler)
        {
            ICommand command = _GetCommand(code);

            if (command != null)
            {
                command.RemoveProtocolEventListent(handler as Delegate);
            }
        }

        public void RemoveProtocolListent(int code, EventListener handler)
        {
            RemoveProtocolListent<EventListener>(code, handler);
        }

        public static void Send(int code, object data = null)
        {
            _instance.SendMessage(code, data);
        }

        /// <summary>
        /// 出包
        /// </summary>
        public void SendMessage(int code, object data = null)
        {
            //Socket连接上，并且无重连事宜处理
            ICommand command = _GetCommand(code);
            if (command != null && command.needWaitResponse)
            {
                if (_lockList.Exists(c => c == code))
                {
                    Log.Error("协议[" + code.ToString() + "," + command.desc + "]暂时锁定");
                    return;
                }
            }

            if (command != null)
            {
                Log.Error("Socket send: [" + code.ToString() + "," + command.desc + "]");
                command.WritePackage(data);

                if (_sendHandler != null) _sendHandler(code);
            }
        }

        /// <summary>
        /// 收包
        /// </summary>
        public void SendNotifyListener(int code, params object[] parameters)
        {
            ICommand command = _GetCommand(code);

            if (command != null)
            {
                command.Notify(parameters);

                if (_receiveHandler != null) _receiveHandler(code);
            }
        }

        public void ClearLock()
        {
            _lockList.Clear();
        }

        public bool ReadMessage(int code, PackageIn pkg)
        {
            ICommand command = _GetCommand(code);
            ClearLock();
            if (command != null)
            {
                Log.Error("Socket Receive: [0x" + code.ToString() + "," + command.desc + "," + BitConverter.ToString(pkg.GetByteArray(0, pkg.length)) + "]");
                command.ReadPackage(pkg);
                return true;
            }
            return false;
        }

        private ICommand _GetCommand(int code)
        {
            if (_commands.ContainsKey(code))
            {
                return _commands[code];
            }
            return null;
        }
    }
}