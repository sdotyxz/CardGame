using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Net.Sockets
{
    public class PackageIn:ByteBuffer
    {
        public const int HEADER_SIZE = 12;

        private int _code;
        private int _errCode;
		
		public PackageIn(short code = 0)
		{
			_code = code;
		}

        public void Load(byte[] src, int index, int len)
        {
            SetByteArray(src, index, 0, len);
            this.position = 0;
            _ReadHeader();
        }
		
		private void _ReadHeader()
		{
			PopInt();
            _code = PopInt();
            _errCode = PopInt();
		}
		
        public int code{
            get {return _code;}
        }

        public int errCode {
            get { return _errCode; }
        }

        public bool HasBody()
        {
            return _length > HEADER_SIZE;
        }
        /*
        public byte[] ReadBodyBytes()
        {
            return GetByteArray(HEADER_SIZE, _length - HEADER_SIZE);
        }

        public string ReadBody()
        { 
            byte[] bytes = ReadBodyBytes();
            if(bytes != null && bytes.Length > 0){
                return Encoding.UTF8.GetString(bytes);
            }
            return null;
        }
        */
    }
}
