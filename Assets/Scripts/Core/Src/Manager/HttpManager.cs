using Core.Net.Protocol;
using Core.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Src.Manager
{
    public class HttpManager
    {
        public const string HTTP_ERROR = "HTTP_ERROR";

        private string _url = "http://192.168.1.22";
        private static HttpManager _instance = new HttpManager();
        public static HttpManager instance
        {
            get { return _instance; }
        }

        private HttpManager()
        {
            if (_instance != null)
            {
                throw new UnityEngine.UnityException("Error: Please use instance to get HttpManager.");
            }
        }

        public void Init(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.Error("HttpManager:Init - url is invalid");
            }

            _url = url;
            NetManager.sendFunction = SocketManager.instance.SendPackage;
        }

        private void _SocketData(PackageIn pkg)
        {
            ProtocolManager.instance.ReadMessage(pkg.code, pkg);
        }

        public void SendPackage(PackageOut pkg)
        {
            var r = new HTTP.Request("GET", _url, pkg.ToByteArray());
            r.Send(_OnResponse);
            Log.Debug("Socket SendPackage: [" + pkg.code.ToString() + "," + BitConverter.ToString(pkg.GetByteArray(0, pkg.length)) + "]");
        }

        private void _OnResponse(HTTP.Response response)
        {
            if (response != null && response.status == 200)
            {
                PackageIn pkg = new PackageIn();
                pkg.Load(response.Bytes, 0, response.Bytes.Length);
                ProtocolManager.instance.ReadMessage(pkg.code, pkg);
            }
            else
            {
                EventManager.Dispatch(this, HTTP_ERROR);
            }
        }
    }
}
