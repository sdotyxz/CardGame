using Core.Src.Manager;
using Core.Net.Protocol;
using Core.Net.Sockets;
using System;

namespace Core.Src.Manager
{
    public class SocketManager
    {
        private ByteSocket _socket;	//底层socket	
        public ByteSocket socket { get { return _socket; } }
        private string _ip;		//ip地址
        private int _port;		//端口号		

        private static SocketManager _instance = new SocketManager();
        public static SocketManager instance
        {
            get { return _instance; }
        }

        private SocketManager()
        {
            if (_instance != null)
            {
                throw new UnityEngine.UnityException("Error: Please use instance to get SocketManager.");
            }
        }

        public void Init()
        {
            NetManager.sendFunction = SocketManager.instance.SendPackage;

            _socket = new ByteSocket(false);
            EventManager.AddListener<Action>(_socket, SocketEvent.SOCKET_CONNECT, _SocketConnected);
            EventManager.AddListener<Action>(_socket, SocketEvent.SOCKET_CLOSE, _SocketClose);
            EventManager.AddListener<Action<PackageIn>>(_socket, SocketEvent.SOCKET_DATA, _SocketData);
            EventManager.AddListener<Action<string>>(_socket, SocketEvent.SOCKET_ERROR, _SocketError);
        }

        public void Connect(string ip, int port)
        {
            Log.Debug("connecting server:" + ip + " port:" + port.ToString());
            _socket.Connect(ip, port);
            _ip = ip;
            _port = port;
        }

        public void Close()
        {
            _socket.Close();
        }

        public void Reconnect()
        {
            _socket.ResetKey();
            _socket.Connect(_ip, _port);
        }

        private void _SocketClose()
        {
            EventManager.Dispatch(SocketManager.instance, SocketEvent.SOCKET_CLOSE);
        }

        private void _SocketConnected()
        {
            EventManager.Dispatch(SocketManager.instance, SocketEvent.SOCKET_CONNECT);
        }

        private void _SocketError(string reason)
        {
            EventManager.Dispatch(SocketManager.instance, SocketEvent.SOCKET_ERROR);
        }

        private void _SocketData(PackageIn pkg)
        {
            ProtocolManager.instance.ReadMessage(pkg.code, pkg);
        }

        public void SendPackage(PackageOut pkg)
        {
            _socket.Send(pkg);

            Log.Debug("Socket SendPackage: [" + pkg.code.ToString() + "," + BitConverter.ToString(pkg.GetByteArray(0, pkg.length)) + "]");
        }

        public int port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string ip
        {
            get { return _ip; }
            set { _ip = value; }
        }
    }
}