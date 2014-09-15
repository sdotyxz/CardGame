using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Net.Sockets
{
    public class PackageOut:ByteBuffer
    {
        public const int HEADER_SIZE = 12;
		private int _code;

        public PackageOut(int code)
		{
            PushInt(0x00);	//长度
            PushInt(code);	//协议号
            PushInt(0x00);
			_code = code;
		}

        public int code
        {
            get { return _code; }
        }
		
		public void Pack()
		{
            byte[] len;
            if(BitConverter.IsLittleEndian)
            {
                len = EdianConvert.GetConvertEdian(BitConverter.GetBytes(_length));
            }
            else
            {
                len = BitConverter.GetBytes(_length);
            }
            Array.Copy(len, 0, _tempBytes, 0, len.Length);
		}
		/*
        public void WriteBodyBytes(byte[] value)
        {
            if (value != null && value.Length > 0) {
                SetByteArray(value, 0, HEADER_SIZE, value.Length);
            } 
        }

        public void WriteBody(string value)
        {
            if (!string.IsNullOrEmpty(value)) {
                WriteBodyBytes(System.Text.Encoding.UTF8.GetBytes(value));
            }
        }
        */
    }
}
