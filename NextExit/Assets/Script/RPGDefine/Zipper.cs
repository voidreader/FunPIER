using UnityEngine;
using System;
using System.IO;
using System.Text;
/// <summary>
/// Assets/Plugins/ICSharpCode.SharpZibLib
/// </summary>
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.GZip;

public struct Zipper
{
    /// <summary>
    /// zip 압축을 하고 결과를 리턴합니다.
    /// </summary>
    /// <returns>The string.</returns>
    /// <param name="sBuffer">S buffer.</param>
    public static string ZipString(string sBuffer)
    {
        //return ZipString(Encoding.UTF8.GetBytes(sBuffer));
        return ZipString(Encoding.UTF8.GetBytes(sBuffer));
    }

    public static string ZipString(byte[] bBuffer)
    {
        //Debug.Log("ZipString original byte count = " + bBuffer.Length);
        
        MemoryStream m_msBZip2 = null;
        BZip2OutputStream m_osBZip2 = null;
        string result;
        try
        {
            m_msBZip2 = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(m_msBZip2, System.Text.Encoding.UTF8))
            {
                byte[] bytes = bBuffer;
                writer.Write(bytes.Length);

                m_osBZip2 = new BZip2OutputStream(m_msBZip2);                
                m_osBZip2.Write(bytes, 0, bytes.Length);
                
                m_osBZip2.Close();
                //Debug.Log("ZipString compress byte count = " + m_msBZip2.ToArray().Length);

                result = Convert.ToBase64String(m_msBZip2.ToArray());
                //result = Encoding.UTF8.GetString(m_msBZip2.ToArray());
                m_msBZip2.Close();

                writer.Close();
            }
        }
        finally
        {
            if (m_osBZip2 != null)
            {
                m_osBZip2.Dispose();
            }
            if (m_msBZip2 != null)
            {
                m_msBZip2.Dispose();
            }
        }
        return result;
    }

    /// <summary>
    /// zip 압축을 해제하고 결과를 리턴합니다.
    /// </summary>
    /// <returns>The string.</returns>
    /// <param name="compbytes">Compbytes.</param>
    public static string UnzipString(string str)
    {
        return UnzipString(Convert.FromBase64String(str));
        //return UnzipString(Encoding.UTF8.GetBytes(str));
    }

    public static string UnzipString(byte[] compbytes)
    {
        return Encoding.UTF8.GetString(UnzipBytes(compbytes));
    }

    public static byte[] UnzipBytes(string str)
    {
        return UnzipBytes(Convert.FromBase64String(str));
        //return UnzipBytes(Encoding.UTF8.GetBytes(str));
    }

    public static byte[] UnzipBytes(byte[] compbytes)
    {
        byte[] result = null;

        using (MemoryStream ms = new MemoryStream(compbytes))
        using (BinaryReader reader = new BinaryReader(ms, System.Text.Encoding.UTF8))
        {
            Int32 size = reader.ReadInt32();

            byte[] bytesUncompressed = new byte[size];
            using (BZip2InputStream isBZip2 = new BZip2InputStream(ms))
            {
                isBZip2.Read(bytesUncompressed, 0, bytesUncompressed.Length);
                result = bytesUncompressed;
            }
        }
        return result;
    }

    public static string UnGzipString(byte[] compbytes)
    {
        return Encoding.UTF8.GetString(UnGzipBytes(compbytes));
    }

    public static byte[] UnGzipBytes(byte[] compbytes)
    {
        byte[] result = null;
        byte[] bytesUncompressed = new byte[4096];
        using (MemoryStream read_stream = new MemoryStream(compbytes))
        using (MemoryStream write_stream = new MemoryStream())
        {
            using (GZipInputStream uncompress_stream = new GZipInputStream(read_stream))
            {
                while (true)
                {
                    int size = uncompress_stream.Read(bytesUncompressed, 0, bytesUncompressed.Length);
                    if (size == 0)
                        break;
                    write_stream.Write(bytesUncompressed, 0, size);
                }
            }
            result = write_stream.ToArray();
        }
        return result;
    }



}
