using System;
namespace Core.Net.Sockets
{
    public class ByteBuffer
    {
        //数组的最大长度
        public const int MAX_LENGTH = 32768;
        //固定长度的中间数组
        protected byte[] _tempBytes = new byte[MAX_LENGTH];
        //当前数组长度
        protected int _length = 0;
        //当前Pop指针位置
        protected int _position = 0;

        public ByteBuffer()
        {
            _tempBytes.Initialize();
            _length = 0;
            _position = 0;
        }

        public int length
        {
            get
            {
                return _length;
            }
        }

        public int position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public byte[] ToByteArray()
        {
            //分配大小
            byte[] bytes = new byte[_length];
            //调整指针
            Array.Copy(_tempBytes, 0, bytes, 0, _length);
            return bytes;
        }

        public void PushByte(byte by)
        {
            _tempBytes[_length++] = by;
        }

        public void PushSByte(sbyte by)
        {
            PushByte((byte)by);
        }

        public void PushByteArray(byte[] sourceBytes)
        {
            SetByteArray(sourceBytes, 0, _length, sourceBytes.Length);
        }

        public void PushUShort(ushort Num)
        {
            _tempBytes[_length++] = (byte)(((Num & 0xff00) >> 0x08) & 0xff);
            _tempBytes[_length++] = (byte)((Num & 0x00ff) & 0xff);
        }

        public void PushShort(short Num)
        {
            PushUShort((ushort)Num);
        }

        public void PushUInt(uint Num)
        {
            _tempBytes[_length++] = (byte)(((Num & 0xff000000) >> 0x18) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x00ff0000) >> 0x10) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x0000ff00) >> 0x08) & 0xff);
            _tempBytes[_length++] = (byte)((Num & 0x000000ff) & 0xff);
        }

        public void PushInt(int Num)
        {
            PushUInt((uint)Num);
        }

        public void PushULong(ulong Num)
        {
            _tempBytes[_length++] = (byte)(((Num & 0xff00000000000000) >> 0x38) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x00ff000000000000) >> 0x30) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x0000ff0000000000) >> 0x28) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x000000ff00000000) >> 0x20) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x00000000ff000000) >> 0x18) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x0000000000ff0000) >> 0x10) & 0xff);
            _tempBytes[_length++] = (byte)(((Num & 0x000000000000ff00) >> 0x08) & 0xff);
            _tempBytes[_length++] = (byte)((Num & 0x00000000000000ff) & 0xff);
        }

        public void PushLong(long Num)
        {
            PushULong((ulong)Num);
        }

        public void PushUTF(string value)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            PushShort((short)bytes.Length);
            PushByteArray(bytes);
        }

        public byte PopByte()
        {
            byte ret = _tempBytes[_position++];
            return ret;
        }

        public sbyte PopSByte()
        {
            return (sbyte)PopByte();
        }

        public ushort PopUShort()
        {
            //溢出
            if (_position + 2 > _length)
            {
                return 0;
            }
            ushort ret = (ushort)(_tempBytes[_position] << 0x08 | _tempBytes[_position + 1]);
            _position += 2;
            return ret;
        }

        public short PopShort()
        {
            return (short)PopUShort();
        }

        public uint PopUInt()
        {            
            if (_position + 4 > _length)
                return 0;
            uint ret = (uint)(_tempBytes[_position] << 0x18 | _tempBytes[_position + 1] << 0x10 | _tempBytes[_position + 2] << 0x08 | _tempBytes[_position + 3]);
            _position += 4;
            return ret;
        }

        public int PopInt()
        {
            return (int)PopUInt();
        }

        public ulong PopULong()
        {
            if (_position + 8 > _length)
                return 0;
            ulong ret = (ulong)((uint)(_tempBytes[_position] << 0x18 | _tempBytes[_position + 1] << 0x10 | _tempBytes[_position + 2] << 0x08 | _tempBytes[_position + 3]) * 0x100000000) +
                (uint)(_tempBytes[_position + 4] << 0x18 | _tempBytes[_position + 5] << 0x10 | _tempBytes[_position + 6] << 0x08 | _tempBytes[_position + 7]);
            _position += 8;
            return ret;
        }

        public long PopLong()
        {
            return (long)PopULong();
        }

        public string PopUTF()
        {
            short len = PopShort();
            return System.Text.Encoding.UTF8.GetString(PopByteArray(len));
        }

        public byte[] PopByteArray(int Length)
        {
            //溢出
            if (_position + Length > _length)
            {
                return new byte[0];
            }
            byte[] ret = new byte[Length];
            Array.Copy(_tempBytes, _position, ret, 0, Length);
            //提升位置
            _position += Length;
            return ret;
        }
  
        public byte Get(int index)
        {
            return _tempBytes[index];
        }

        public void Set(int index, byte value)
        {
            _tempBytes[index] = value;
        }

        public byte[] GetByteArray(int index, int Length)
        {
            //溢出
            if (index + Length > _length)
            {
                return new byte[0];
            }
            byte[] ret = new byte[Length];
            Array.Copy(_tempBytes, index, ret, 0, Length);
            return ret;
        }

        public void SetByteArray(byte[] sourceBytes, int sourceIndex, int offset, int len)
        {
            Array.Copy(sourceBytes, sourceIndex, _tempBytes, offset, len);
            if (_length < offset + len) {
                _length = offset + len;
            }
        }
    }
}