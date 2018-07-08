using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityObject;

//字节流协议模型
public class ProtocolBytes : ProtocolBase
{
    //传输的字节流
    public byte[] bytes;

    //解码器
    public override ProtocolBase Decode(byte[] readbuff, int start, int length)
    {

        ProtocolBytes protocol = new ProtocolBytes();
        protocol.bytes = new byte[length];
        Array.Copy(readbuff, start, protocol.bytes, 0, length);
        return protocol;
    }

    //编码器
    public override byte[] Encode()
    {
        return bytes;
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
        if (bytes == null) return str;
        for (int i = 0; i < bytes.Length; i++)
        {
            int b = (int)bytes[i];
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
        if (bytes == null)
            bytes = lenBytes.Concat(strBytes).ToArray();
        else
            bytes = bytes.Concat(lenBytes).Concat(strBytes).ToArray();
    }

    //从字节数组的start处开始读取字符串
    public string GetString(int start, ref int end)
    {
        if (bytes == null)
            return "";
        if (bytes.Length < start + sizeof(Int32))
            return "";
        Int32 strLen = BitConverter.ToInt32(bytes, start);
        if (bytes.Length < start + sizeof(Int32) + strLen)
            return "";
        string str = System.Text.Encoding.UTF8.GetString(bytes, start + sizeof(Int32), strLen);
        end = start + sizeof(Int32) + strLen;
        return str;
    }

    public string GetString(int start)
    {
        int end = 0;
        return GetString(start, ref end);
    }



    public void AddInt(int num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }

    public int GetInt(int start, ref int end)
    {
        if (bytes == null)
            return 0;
        if (bytes.Length < start + sizeof(Int32))
            return 0;
        end = start + sizeof(Int32);
        return BitConverter.ToInt32(bytes, start);
    }

    public int GetInt(int start)
    {
        int end = 0;
        return GetInt(start, ref end);
    }


	public void AddLong(long num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }

	public long GetLong(int start, ref int end)
    {
        if (bytes == null)
            return 0;
        if (bytes.Length < start + sizeof(Int32))
            return 0;
		end = start + sizeof(long);
		return BitConverter.ToInt64(bytes, start);
    }

    public long GetLong(int start)
    {
        int end = 0;
        return GetLong(start, ref end);
    }
    
    public void AddFloat(float num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        if (bytes == null)
            bytes = numBytes;
        else
            bytes = bytes.Concat(numBytes).ToArray();
    }

    public float GetFloat(int start, ref int end)
    {
        if (bytes == null)
            return 0;
        if (bytes.Length < start + sizeof(float))
            return 0;
        end = start + sizeof(float);
        return BitConverter.ToSingle(bytes, start);
    }

    public float GetFloat(int start)
    {
        int end = 0;
        return GetFloat(start, ref end);
    }

    public void AddSyncData(SyncData data)
    {
        if (data.Bytes.Length == 0) return;
        bytes = bytes.Concat(data.Bytes).ToArray();
    }

    public SyncData GetSyncData(int start, ref int end)
    {
        SyncData data = new SyncData();
        data.Bytes = new byte[bytes.Length - start];
        if (data.Bytes.Length == 0)
            return data;
        Array.Copy(bytes, start, data.Bytes, 0, bytes.Length - start);

        return data;
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


    public void AddObjects(object[] objs)
    {
        AddInt(objs.Length);
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] buff = null;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is Vector3)// 用is会出问题
                {
                    Vector3 vec = (Vector3)objs[i];
                    MyVector3 myvec = new MyVector3 { x = vec.x, y = vec.y, z = vec.z };
                    objs[i] = myvec;
                }
                else if (objs[i] is Vector2)
                {
                    Vector2 vec = (Vector2)objs[i];
                    MyVector2 myvec = new MyVector2 { x = vec.x, y = vec.y };
                    objs[i] = myvec;
                }
                bf.Serialize(ms, objs[i]);
            }
            buff = ms.ToArray();
            bytes = bytes.Concat(buff).ToArray();
        }
    }

    public object[] GetObjects(int start)
    {
        if (bytes == null)
            return null;
        if (bytes.Length < start)
            return null;
        int length = GetInt(start, ref start);
        object[] objs = new object[length];

        using (MemoryStream ms = new MemoryStream(bytes, start, bytes.Length - start))
        {
            BinaryFormatter bf = new BinaryFormatter();
            for (int i = 0; i < length; i++)
            {
                objs[i] = bf.Deserialize(ms);
                if (objs[i] is MyVector3)
                {
                    MyVector3 myvec = (MyVector3)objs[i];
                    Vector3 vec = new Vector3 { x = myvec.x, y = myvec.y, z = myvec.z };
                    objs[i] = vec;
                }
                else if (objs[i] is MyVector2)
                {
                    MyVector2 myvec = (MyVector2)objs[i];
                    Vector3 vec = new Vector3 { x = myvec.x, y = myvec.y };
                    objs[i] = vec;
                }
            }
        }
        return objs;
    }
}