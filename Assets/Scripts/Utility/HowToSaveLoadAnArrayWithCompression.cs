using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq;
using UnityEngine;

public class HowToSaveLoadAnArrayWithCompression : MonoBehaviour
{
    public static HowToSaveLoadAnArrayWithCompression instance;

    private void Awake()
    {
        instance = this;
    }

    public static void CopyStream(Stream input, Stream output)
    // Helper funtion to copy from one stream to another
    {
        // Magic number is 2^16
        byte[] buffer = new byte[32768];
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, read);
        }
    }


    public static string Compress(string s)
    {
        var bytes = Encoding.UTF8.GetBytes(s);
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {

                CopyStream(msi, gs);
            }
            return Convert.ToBase64String(mso.ToArray());
        }
    }

    public static string Decompress(string s)
    {
        var bytes = Convert.FromBase64String(s);
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                CopyStream(gs, mso);
            }
            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }



    public string GenerateAStringFromAnArray(bool[,] myArray)
    {
        string s = "";

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, myArray);

        s = Convert.ToBase64String(ms.ToArray());
        s = Compress(s);
        return s;
    }
    public string GenerateAStringFromSect(BasicSect[,,] myArray)
    {
        string s = "";

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, myArray);

        s = Convert.ToBase64String(ms.ToArray());
        s = Compress(s);
        return s;
    }

    public string GenerateStringFromList(List<bool[,,]> myList)
    {
        string s = "";

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        List<bool[,,]> arrayList = new List<bool[,,]>();
        foreach (bool[,,] item in myList)
        {
            arrayList.Add(item);
        }
        bool[][,,] myArray = arrayList.ToArray();
        bf.Serialize(ms, myArray);

        s = Convert.ToBase64String(ms.ToArray());
        s = Compress(s);
        return s;
    }
    public bool[][,,] LoadListFromString(string s)
    {
        s = Decompress(s);

        BinaryFormatter bf = new BinaryFormatter();
        Byte[] by = Convert.FromBase64String(s);
        MemoryStream sr = new MemoryStream(by);

        return (bool[][,,])bf.Deserialize(sr);
    }

    public BasicSect[,,] LoadAnArrayFromString(string s)
    {
        s = Decompress(s);

        BinaryFormatter bf = new BinaryFormatter();
        Byte[] by = Convert.FromBase64String(s);
        MemoryStream sr = new MemoryStream(by);

        return (BasicSect[,,])bf.Deserialize(sr);

    }
}
