using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WSA
#else // UNITY_WSA
using System.Security.Cryptography;
#endif // UNITY_WSA

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// File을 읽거나 쓸 때 사용하는 Stream 입니다.
	/// 하나의 Stream에서는 읽기 쓰기 중 하나만 가능합니다.
	/// </summary>
	public class HTFileStream
	{
		//---------------------------------------
		private FileStream _fs = null;
		private CryptoStream _cs = null;
		private StreamWriter _sw = null;
		private StreamReader _sr = null;

		//---------------------------------------
		private HTFileStream() { }
		public HTFileStream(string szFileAddr, bool bWrite, bool bUseCrypt)
		{
			FileMode eMode = (bWrite) ? FileMode.Create : FileMode.Open;
			FileAccess eAccess = (bWrite) ? FileAccess.Write : FileAccess.Read;

			_fs = File.Open(szFileAddr, eMode, eAccess, FileShare.None);
			Stream pTargetStream = _fs;

#if UNITY_WSA
#else // UNITY_WSA
			if (bUseCrypt)
			{
				ICryptoTransform pTransform = (bWrite) ? CryptUtils.CryptProvd.CreateEncryptor() : CryptUtils.CryptProvd.CreateDecryptor();
				CryptoStreamMode eCryptMode = (bWrite) ? CryptoStreamMode.Write : CryptoStreamMode.Read;
				_cs = new CryptoStream(_fs, pTransform, eCryptMode);

				pTargetStream = _cs;
			}
#endif // UNITY_WSA

			if (bWrite)
				_sw = new StreamWriter(pTargetStream, System.Text.Encoding.UTF8);
			else
				_sr = new StreamReader(pTargetStream, System.Text.Encoding.UTF8);
		}

		~HTFileStream()
		{
			Release();
		}

		//---------------------------------------
		public void WriteLine(bool value) { _sw.WriteLine(value); }
		public void WriteLine(byte value) { _sw.WriteLine(value); }
		public void WriteLine(int value) { _sw.WriteLine(value); }
		public void WriteLine(short value) { _sw.WriteLine(value); }
		public void WriteLine(long value) { _sw.WriteLine(value); }
		public void WriteLine(float value) { _sw.WriteLine(value); }
		public void WriteLine(double value) { _sw.WriteLine(value); }
		public void WriteLine(string value) { _sw.WriteLine(value); }

		public bool ReadLine_Bool() { return System.Convert.ToBoolean(_sr.ReadLine()); }
		public byte ReadLine_Byte() { return System.Convert.ToByte(_sr.ReadLine()); }
		public int ReadLine_Int() { return System.Convert.ToInt32(_sr.ReadLine()); }
		public short ReadLine_Short() { return System.Convert.ToInt16(_sr.ReadLine()); }
		public long ReadLine_Long() { return System.Convert.ToInt64(_sr.ReadLine()); }
		public float ReadLine_Float() { return System.Convert.ToSingle(_sr.ReadLine()); }
		public double ReadLine_Double() { return System.Convert.ToDouble(_sr.ReadLine()); }
		public string ReadLine() { return _sr.ReadLine(); }

		//---------------------------------------
		public void Release()
		{
			if (_sw != null)
			{
				_sw.Flush();
				_sw.Close();
				_sw = null;
			}

			if (_sr != null)
			{
				_sr.Close();
				_sr = null;
			}

			if (_cs != null)
			{
				_cs.Close();
				_cs = null;
			}

			if (_fs != null)
			{
				_fs.Close();
				_fs = null;
			}
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// File을 관리하는 기능을 가진 Utility입니다.
	/// </summary>
	public class FileUtils
	{
		//---------------------------------------
		public static void CreateDirectory(string filename)
		{
			Directory.CreateDirectory((new FileInfo(filename)).Directory.FullName);
		}

		public static string PersistentPath { get { return Application.persistentDataPath; } }
		public static string CombinePersistentPath(string filename)
		{
			return string.Format("{0}/{1}", PersistentPath, filename);
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// JSON Node를 File로 추출합니다.
		/// </summary>
		/// <param name="szFileAddress">추출 될 File 경로 및 이름</param>
		/// <param name="pJSON">대상 JSON Node</param>
		static public void CreateFileFromJSON(string szFileAddress, JSONNode pJSON)
		{
			if (pJSON == null)
			{
				HTDebug.PrintLog(eMessageType.Warning, "[HTUtils] JSON is nullptr!");
				return;
			}

			CreateFileFromBuffer(szFileAddress, pJSON.ToString());
		}

		/// <summary>
		/// String을 File로 추출합니다.
		/// </summary>
		/// <param name="szFileAddress">추출 될 File 경로 및 이름</param>
		/// <param name="szBuffer">대상 String</param>
		static public void CreateFileFromBuffer(string szFileAddress, string szBuffer, bool bUseEncrypt = true)
		{
			CreateDirectory(szFileAddress);
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(szBuffer);

#if UNITY_WSA
			File.WriteAllBytes(szFileAddress, bytes);
#else // UNITY_WSA
			if (bUseEncrypt)
				bytes = CryptUtils.BlowFish.Encrypt_CBC(bytes);

			File.WriteAllBytes(szFileAddress, bytes);
#endif // UNITY_WSA
		}

		/// <summary>
		/// Texture를 Image File로 추출합니다.
		/// </summary>
		/// <param name="szFileAddress">추출 될 File 경로 및 이름</param>
		/// <param name="pTex">대상 Texture</param>
		/// <param name="eExist">추출 될 Image File 확장자</param>
		static public void CreateFileFromTexture(string szFileAddress, Texture2D pTex, eImageExist eExist)
		{
			CreateDirectory(szFileAddress);
			switch (eExist)
			{
				case eImageExist.PNG:
					File.WriteAllBytes(szFileAddress, pTex.EncodeToPNG());
					break;

				case eImageExist.JPG:
					File.WriteAllBytes(szFileAddress, pTex.EncodeToJPG());
					break;
			}
		}

		//---------------------------------------
		/// <summary>
		/// 대상 File을 JSON Node로 Parsing합니다.
		/// </summary>
		/// <param name="szFileAddress">대상 File 경로 및 이름</param>
		/// <param name="pJSON">Parsing 된 JSON Node</param>
		static public void LoadJSONFromFile(string szFileAddress, out JSONNode pJSON)
		{
			if (File.Exists(szFileAddress) == false)
			{
				HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTUtils] {0} is not exist!", szFileAddress));
				pJSON = null;
				return;
			}

			string szLoadBuffer;
			LoadBufferFromFile(szFileAddress, out szLoadBuffer);
			pJSON = JSON.Parse(szLoadBuffer);
		}

		/// <summary>
		/// 대상 File을 String으로 Parsing합니다.
		/// </summary>
		/// <param name="szFileAddress">대상 File 경로 및 이름</param>
		/// <param name="szBuffer">Parsing 된 String</param>
		static public void LoadBufferFromFile(string szFileAddress, out string szBuffer, bool bUseEncrypt = true)
		{
			szBuffer = null;

			byte[] vBytes = LoadBufferFromFile(szFileAddress, bUseEncrypt);
			if (vBytes.IsNullOrEmpty() == false)
				szBuffer = Encoding.UTF8.GetString(vBytes);
		}

		/// <summary>
		/// 대상 File을 byte[]으로 Parsing합니다.
		/// </summary>
		/// <param name="szFileAddress">대상 File 경로 및 이름</param>
		/// <param name="szBuffer">Parsing 된 String</param>
		static public byte[] LoadBufferFromFile(string szFileAddress, bool bUseEncrypt = true)
		{
			if (File.Exists(szFileAddress) == false)
			{
				HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTUtils] {0} is not exist!", szFileAddress));
				return null;
			}

			byte[] bytes = File.ReadAllBytes(szFileAddress);

#if UNITY_WSA
			szBuffer = Encoding.UTF8.GetString(bytes);
#else // UNITY_WSA
			if (bUseEncrypt)
				bytes = CryptUtils.BlowFish.Decrypt_CBC(bytes);

			return bytes;
#endif // UNITY_WSA
		}

		/// <summary>
		/// 대상 File을 Texture로 Parsing합니다.
		/// </summary>
		/// <param name="szFileAddress">대상 File 경로 및 이름</param>
		/// <param name="pTex">Parsing 된 Texture</param>
		static public void LoadTextureFromFile(string szFileAddress, out Texture2D pTex)
		{
			byte[] bytes = File.ReadAllBytes(szFileAddress);

			pTex = new Texture2D(2, 2);
			pTex.LoadImage(bytes);
		}

		/// <summary>
		/// 해당 File을 XML Document로 Parsing합니다.
		/// </summary>
		/// <param name="szFileAddress">대상 File 경로 및 이름</param>
		/// <returns>Parsing 된 XML Document</returns>
		public static System.Xml.XmlDocument LoadXMLFromFile(string szFileAddr)
		{
			string szBuffer = null;
			LoadBufferFromFile(szFileAddr, out szBuffer);
			if (szBuffer == null)
				return null;

			System.Xml.XmlDocument pXML = new System.Xml.XmlDocument();
			pXML.LoadXml(szBuffer);
			return pXML;
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 대상 경로 안의 모든 File 목록을 가져옵니다.
		/// </summary>
		/// <param name="szAddress">대상 경로</param>
		/// <returns>File 목록</returns>
		static public string[] GetFile(string szAddress)
		{
			return Directory.GetFiles(szAddress, "*.*", SearchOption.AllDirectories);
		}

		/// <summary>
		/// 대상 경로 안의 File 목록을 WildCard를 사용해서 가져옵니다.
		/// </summary>
		/// <param name="szAddress">대상 경로</param>
		/// <param name="szSearchPattern">Wild Card</param>
		/// <returns>File 목록</returns>
		static public string[] GetFile(string szAddress, string szSearchPattern)
		{
			return Directory.GetFiles(szAddress, szSearchPattern, SearchOption.AllDirectories);
		}

		/// <summary>
		/// 대상 File을 삭제합니다.
		/// </summary>
		static public void DeleteFile(string szFileAddress)
		{
			File.Delete(szFileAddress);
		}

		/// <summary>
		/// 대상 File의 Last Write Time을 가져옵니다.
		/// </summary>
		static public DateTime GetFileLastWriteTime(string szFileAddress)
		{
			return File.GetLastWriteTime(szFileAddress);
		}

		/// <summary>
		/// 해당 File이 존재하는지 확인합니다.
		/// </summary>
		static public bool CheckFileExist(string szFileAddress)
		{
			return File.Exists(szFileAddress);
		}

		//---------------------------------------
		/// <summary>
		/// 문자열에서 FileName과 Exist를 추출합니다.
		/// </summary>
		/// <param name="szFullAddress">File 문자열</param>
		/// <returns>추출 된 FileName과 Exist (File.exist)</returns>
		static public string GetFileNameAndExist(string szFullAddress)
		{
			int nIndex = szFullAddress.LastIndexOf("\\");
			if (nIndex <= 0)
				nIndex = szFullAddress.LastIndexOf("/");

			return szFullAddress.Substring(nIndex + 1);
		}

		/// <summary>
		/// 문자열 배열에서 FileName과 Exist를 추출합니다.
		/// </summary>
		/// <param name="szFullAddress">File 문자열 배열</param>
		/// <returns>추출 된 FileName과 Exist 배열 (File.exist)</returns>
		static public string[] GetFileNameAndExist(string[] szFullAddress)
		{
			string[] vRetVal = new string[szFullAddress.Length];
			for (int nInd = 0; nInd < vRetVal.Length; ++nInd)
				vRetVal[nInd] = GetFileNameAndExist(szFullAddress[nInd]);

			return vRetVal;
		}

		/// <summary>
		/// 문자열에서 Exist를 제외 한 FileName을 추출합니다.
		/// </summary>
		/// <param name="szFullAddress">File 문자열</param>
		/// <returns>추출 된 FileName</returns>
		static public string GetFileName(string szFullAddress)
		{
			szFullAddress = GetFileNameAndExist(szFullAddress);

			int nIndex = szFullAddress.LastIndexOf(".");
			if (nIndex > 0)
				szFullAddress = szFullAddress.Substring(0, nIndex);

			return szFullAddress;
		}

		/// <summary>
		/// 문자열 배열에서 Exist를 제외 한 FileName을 추출합니다.
		/// </summary>
		/// <param name="szFullAddress">File 문자열 배열</param>
		/// <returns>추출 된 FileName 배열</returns>
		static public string[] GetFileName(string[] szFullAddress)
		{
			string[] vRetVal = new string[szFullAddress.Length];
			for (int nInd = 0; nInd < vRetVal.Length; ++nInd)
				vRetVal[nInd] = GetFileName(szFullAddress[nInd]);

			return vRetVal;
		}


		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
