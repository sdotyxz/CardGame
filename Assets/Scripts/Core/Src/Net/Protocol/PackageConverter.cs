using Core.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core.Net.Protocol
{
    public class PackageConverter
    {
        public static object PackageToObject(ByteBuffer pkg, Type type)
        {
            return _PackageToObject(pkg, type);
        }

        private static object _PackageToObject(ByteBuffer pkg, Type type)
        {
            object value = null;
            if (type == typeof(bool))
            {
                value = pkg.PopByte() > 0 ? true : false;
            }
            else if (type == typeof(sbyte))
            {
                value = pkg.PopSByte();
            }
            else if (type == typeof(byte))
            {
                value = pkg.PopByte();
            }
            else if (type == typeof(short))
            {
                value = pkg.PopShort();
            }
            else if (type == typeof(ushort))
            {
                value = pkg.PopUShort();
            }
            else if (type == typeof(int))
            {
                value = pkg.PopInt();
            }
            else if (type == typeof(uint))
            {
                value = pkg.PopUInt();
            }
            else if (type == typeof(long))
            {
                value = pkg.PopLong();
            }
            else if (type == typeof(ulong))
            {
                value = pkg.PopULong();
            }
            else if (type == typeof(string))
            {
                value = pkg.PopUTF();
            }
            else
            {
                Type subType = type.GetElementType();
                if (subType == null)
                {
                    value = System.Activator.CreateInstance(type);
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        field.SetValue(value, _PackageToObject(pkg, field.FieldType));
                    }
                }
                else
                {
                    short count = pkg.PopShort();
                    object[] array = Array.CreateInstance(subType, count) as object[];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = _PackageToObject(pkg, subType);
                    }
                    value = array;
                }
            }
            return value;
        }

        public static ByteBuffer ObjectToPackage(object value, ByteBuffer pkg)
        {
            _ObjectToPackage(value, pkg);
            return pkg;
        }

        private static void _ObjectToPackage(object value, ByteBuffer pkg)
        {
            Type type = value.GetType();
            if (type == typeof(bool))
            {
                pkg.PushByte((bool)value == true ? (byte)1 : (byte)0);
            }
            else if (type == typeof(sbyte))
            {
                pkg.PushSByte((sbyte)value);
            }
            else if (type == typeof(byte))
            {
                pkg.PushByte((byte)value);
            }
            else if (type == typeof(short))
            {
                pkg.PushShort((short)value);
            }
            else if (type == typeof(ushort))
            {
                pkg.PushUShort((ushort)value);
            }
            else if (type == typeof(int))
            {
                pkg.PushInt((int)value);
            }
            else if (type == typeof(uint))
            {
                pkg.PushUInt((uint)value);
            }
            else if (type == typeof(long))
            {
                pkg.PushLong((long)value);
            }
            else if (type == typeof(ulong))
            {
                pkg.PushULong((ulong)value);
            }
            else if (type == typeof(string))
            {
                pkg.PushUTF((string)value);
            }
            else
            {
                Type subType = type.GetElementType();
                if (subType == null)
                {
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        _ObjectToPackage(field.GetValue(value), pkg);
                    }
                }
                else
                {
                    Array array = value as Array;
                    short count = (short)array.Length;
                    pkg.PushShort(count);
                    for (int i = 0; i < count; i++)
                    {
                        _ObjectToPackage(array.GetValue(i), pkg);
                    }
                }
            }
        }
    }
}
