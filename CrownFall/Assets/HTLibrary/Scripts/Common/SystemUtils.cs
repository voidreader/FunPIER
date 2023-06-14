using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib.GZip;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class SystemUtils
	{
		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// Mouse Cursor의 표시 여부를 설정합니다.
		/// </summary>
		public static void CursorShow(bool bShow)
		{
			Cursor.visible = bShow;
		}

		//---------------------------------------
		/// <summary>
		/// Mouse Cursor 잠금 상태를 설정합니다.
		/// </summary>
		public static void CursorLock(eCursorLockState.e eState)
		{
			Cursor.lockState = eCursorLockState.ToLockMode(eState);
		}

		public static bool GetCursorState(eCursorLockState.e eState)
		{
			return (Cursor.lockState == eCursorLockState.ToLockMode(eState)) ? true : false;
		}

		public static eCursorLockState.e GetCursorState()
		{
			return eCursorLockState.ToLockState(Cursor.lockState);
		}


		/////////////////////////////////////////
		//---------------------------------------
		public static byte[] Compress(byte[] data)
		{
			MemoryStream output = new MemoryStream();
			using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
				dstream.Write(data, 0, data.Length);

			return output.ToArray();
		}

		public static byte[] Decompress(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			MemoryStream output = new MemoryStream();
			using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
				dstream.CopyTo(output);

			return output.ToArray();
		}

		//---------------------------------------
		public static byte[] BinaryToGZip(byte[] bin)
		{
			MemoryStream outputMemStream = new MemoryStream();
			GZipOutputStream zipStream = new GZipOutputStream(outputMemStream);
			
			zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
			zipStream.Write(bin, 0, bin.Length);
			zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
			zipStream.Close();          // Must finish the ZipOutputStream before using outputMemStream.

			outputMemStream.Position = 0;
			return outputMemStream.ToArray();
		}

		public static byte[] GZipToBinary(byte[] gzip)
		{
			try
			{
				GZipInputStream gis = new GZipInputStream(new MemoryStream(gzip));
				MemoryStream outputMemStream = new MemoryStream();

				byte[] buffer = new byte[1024];
				int len;
				while ((len = gis.Read(buffer, 0, 1024)) > 0)
					outputMemStream.Write(buffer, 0, len);

				byte[] val = outputMemStream.ToArray();

				//close resources
				outputMemStream.Close();
				gis.Close();

				return val;
			}
			catch (IOException)
			{
				return null;
			}
		}
		
		public static byte[] StringToGZip(string str)
		{
			if (str == null)
				return null;

			return BinaryToGZip(System.Text.Encoding.Default.GetBytes(str));
		}

		public static string GZipToString(byte[] gzip)
		{
			byte[] b = GZipToBinary(gzip);
			if (b == null)
				return null;

			return System.Text.Encoding.Default.GetString(b);
		}

		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
