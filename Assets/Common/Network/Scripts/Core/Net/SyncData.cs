using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.InteropServices;

//字节流协议模型
public class SyncData : ProtocolBase
{
    //传输的字节流
    private byte[] bytes;

    public byte[] Bytes
    {
        get
        {
            return bytes;
        }

        set
        {
            bytes = value;
        }
    }

    private int start = 0;
    // public byte[] GetBytes()
    // {
    //     return Bytes;
    // }
    //解码器
    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {

        SyncData protocol = new SyncData();
        protocol.Bytes = new byte[length];
        Array.Copy(readbuff, start, protocol.Bytes, 0, length);
        return protocol;
    }

    //编码器
    public override byte[] Encode()
    {
        return Bytes;
    }

    //协议名称
    public override string GetName()
    {
        return GetString(0);
    }

    //描述
    public override string GetDesc()
    {
        string str = "";
        if (Bytes == null) return str;
        for (int i = 0; i < Bytes.Length; i++)
        {
            int b = (int)Bytes[i];
            str += b.ToString() + " ";
        }
        return str;
    }


    //添加字符串
    public void AddString(string str)
    {
        Int32 len = str.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(str);
        if (Bytes == null)
            Bytes = lenBytes.Concat(strBytes).ToArray();
        else
            Bytes = Bytes.Concat(lenBytes).Concat(strBytes).ToArray();
    }

    //从字节数组的start处开始读取字符串
    public string GetString(int start, ref int end)
    {
        if (Bytes == null)
            return "";
        if (Bytes.Length < start + sizeof(Int32))
            return "";
        Int32 strLen = BitConverter.ToInt32(Bytes, start);
        if (Bytes.Length < start + sizeof(Int32) + strLen)
            return "";
        string str = System.Text.Encoding.UTF8.GetString(Bytes, start + sizeof(Int32), strLen);
        end = start + sizeof(Int32) + strLen;
        return str;
    }

    public string GetString(int start)
    {
        int end = 0;
        return GetString(start, ref end);
    }

    byte[] Object2Bytes(object obj)
    {
        byte[] buff = new byte[Marshal.SizeOf(obj)];
        IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);
        Marshal.StructureToPtr(obj, ptr, true);
        return buff;
    }

    object Bytes2Object(byte[] buff, Type type)
    {
        IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);
        return Marshal.PtrToStructure(ptr, type);
    }


    public void Add(object obj)
    {
        byte[] numBytes = Object2Bytes(obj);
        if (Bytes == null)
            Bytes = numBytes;
        else
            Bytes = Bytes.Concat(numBytes).ToArray();
    }

    public object Get(Type type)
    {
        if (Bytes == null)
            return null;
        if (Bytes.Length < start + Marshal.SizeOf(type))
            return 0;

        byte[] temp = new byte[Marshal.SizeOf(type)];
        Array.Copy(bytes, start, temp, 0, Marshal.SizeOf(type));
        start += Marshal.SizeOf(type);
        return Bytes2Object(temp, type);

    }
}