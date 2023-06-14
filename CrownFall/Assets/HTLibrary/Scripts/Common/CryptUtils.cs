using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;

#if UNITY_WSA
#else // UNITY_WSA
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
#endif // UNITY_WSA

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// 암호화 기능을 가진 Utility입니다.
	/// </summary>
	public class CryptUtils
	{
#if UNITY_WSA
		//---------------------------------------
		private static string Encrypt(string value)
		{
			return value;
		}

		private static string Decrypt(string value)
		{
			return value;
		}
		
		//---------------------------------------
		private static byte[] EncryptToBytes(string value)
		{
			return Encoding.UTF8.GetBytes(value);
		}

		private static byte[] DecryptToByte(string value)
		{
			return Convert.FromBase64String(value);
		}
		
		//---------------------------------------
		public static string EncryptString(string val)
		{
			return Encrypt(val);
		}

		public static string DecryptString(string val)
		{
			return Decrypt(val);
		}

		//---------------------------------------
		public static string EncryptInt(int val)
		{
			return Encrypt(val.ToString());
		}

		public static int DecryptInt(string val)
		{
			try
			{
				return int.Parse(Decrypt(val), CultureInfo.InvariantCulture);
			}
			catch(Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}

			return 0;
		}

		//---------------------------------------
		public static string EncryptFloat(float val)
		{
			return Encrypt(val.ToString());
		}

		public static float DecryptFloat(string val)
		{
			try
			{
				return float.Parse(Decrypt(val), CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}

			return 0.0f;
		}

		//---------------------------------------
		public static string EncryptEnum<T>(T val) where T : struct
		{
			return Encrypt(val.ToString());
		}

		public static T DecryptEnum<T>(string val) where T : struct
		{
			try
			{
				return (T)Enum.Parse(typeof(T));
			}
			catch(Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}

			return default(T);
		}

#else // UNITY_WSA
		//---------------------------------------
		private const string uniqueID = "B40725BD5EC34332AFFC92A5609C1A3A";
		private const string cryptKey = "d*2eZE#i";

		static private DESCryptoServiceProvider _cryptProvd = null;
		public static DESCryptoServiceProvider CryptProvd
		{
			get
			{
				if (_cryptProvd == null)
				{
					_cryptProvd = new DESCryptoServiceProvider();
					_cryptProvd.Key = System.Text.ASCIIEncoding.ASCII.GetBytes(cryptKey);
					_cryptProvd.IV = System.Text.ASCIIEncoding.ASCII.GetBytes(cryptKey);
				}
				return _cryptProvd;
			}
		}

		private static BlowFish blowFish = new BlowFish(uniqueID, BitConverter.GetBytes(0x0148d7b544a8af41));
		public static BlowFish BlowFish { get { return blowFish; } }

		private static MD5 md5instance = null;

		private static byte[] _password = null;

		/// <summary>
		/// 암호화에 사용되는 Key 값입니다.
		/// </summary>
		public static byte[] Password
		{
			get
			{
				if (_password == null)
					_password = blowFish.Encrypt_CBC(Encoding.UTF8.GetBytes(uniqueID));

				return _password;
			}
		}

		//---------------------------------------
		private static string Encrypt(string value)
		{
			byte[] encrypt = EncryptToBytes(value);
			return Convert.ToBase64String(encrypt);
		}

		private static string Decrypt(string value)
		{
			byte[] decrypt = DecryptToByte(value);
			if (decrypt.Length == 0)
				return string.Empty;

			return Encoding.UTF8.GetString(decrypt);
		}

		//---------------------------------------
		private static byte[] EncryptToBytes(string value)
		{
			return AES.AES_Encrypt(Encoding.UTF8.GetBytes(value), Password);
		}

		private static byte[] DecryptToByte(string value)
		{
			return AES.AES_Decrypt(Convert.FromBase64String(value), Password);
		}

		//---------------------------------------
		public static int DecryptToBlowFishInt(string value)
		{
			if (value.Length == 0)
				return 0;

			byte[] decryptCBC = blowFish.Decrypt_CBC(Convert.FromBase64String(value));
			return BitConverter.ToInt32(decryptCBC, 0);
		}

		public static float DecryptToBlowFishFloat(string value)
		{
			if (value.Length == 0)
				return 0f;

			byte[] decryptCBC = blowFish.Decrypt_CBC(Convert.FromBase64String(value));
			return BitConverter.ToSingle(decryptCBC, 0);
		}

		//---------------------------------------
		public static string EncryptString(string val)
		{
			return Encrypt(val);
		}
		
		public static string DecryptString(string val)
		{
			return Decrypt(val);
		}

		//---------------------------------------
		public static string EncryptInt(int val)
		{
			return Encrypt(val.ToString());
		}
		
		public static int DecryptInt(string val)
		{
			try
			{
				return int.Parse(Decrypt(val), CultureInfo.InvariantCulture);
			}
			catch(Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}

			return 0;
		}

		//---------------------------------------
		public static string EncryptFloat(float val)
		{
			return Encrypt(val.ToString());
		}
		
		public static float DecryptFloat(string val)
		{
			try
			{
				return float.Parse(Decrypt(val), CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}

			return 0.0f;
		}

		//---------------------------------------
		public static string EncryptEnum<T>(T val) where T : struct
		{
			return Encrypt(val.ToString());
		}
		
		public static T DecryptEnum<T>(string val) where T : struct
		{
			try
			{
				return (T)Enum.Parse(typeof(T), Decrypt(val));
			}
			catch(Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}

			return default(T);
		}

		//---------------------------------------
		private static void CreateMD5Instance()
		{
			if (md5instance != null)
				return;

			System.Reflection.MethodInfo pInfo = typeof(MD5).GetMethod("Create", new Type[] { });
			md5instance = (MD5)pInfo.Invoke(null, null);
		}

		/// <summary>
		/// File의 MD5 Hash 값을 가져옵니다.
		/// </summary>
		/// <param name="szFilePath">File의 경로. Unity Client 내부 파일은 사용 할 수 없습니다..</param>
		/// <returns>MD5 Hash값</returns>
		public static string GetFileHashMD5(string szFilePath)
		{
			FileStream pStream = File.OpenRead(szFilePath);
			if (pStream == null)
				return null;

			CreateMD5Instance();
			byte[] vHashes = md5instance.ComputeHash(pStream);

			StringBuilder pBuilder = new StringBuilder();
			foreach (byte bt in vHashes)
				pBuilder.Append(bt.ToString("x2"));

			return pBuilder.ToString();
		}

		/// <summary>
		/// Resource File의 MD5 Hash 값을 가져옵니다.
		/// </summary>
		/// <param name="szFilePath">Resource File의 경로. Unity Client 외부 파일은 사용 할 수 없습니다..</param>
		/// <returns>MD5 Hash값</returns>
		public static string GetResourceHashMD5(string szResourcePath)
		{
			TextAsset pAsset = ResourceUtils.Load(szResourcePath) as TextAsset;
			if (pAsset == null)
				return null;

			CreateMD5Instance();
			byte[] vHashes = md5instance.ComputeHash(pAsset.bytes);

			StringBuilder pBuilder = new StringBuilder();
			foreach (byte bt in vHashes)
				pBuilder.Append(bt.ToString("x2"));

			return pBuilder.ToString();
		}

		//---------------------------------------
		public static bool CheckCodeSignatureCertificatePurity(string szFilePath)
		{
			return true;

			//X509Certificate2 pCertificate;
			//try
			//{
			//	X509Certificate pSigner = X509Certificate.CreateFromSignedFile(szFilePath);
			//	pCertificate = new X509Certificate2(pSigner);
			//}
			//catch (Exception ex)
			//{
			//	HTDebug.PrintLog(eMessageType.Error, string.Format("[CryptUtils] No digital signature found from [{0}] : {1}", szFilePath, ex.Message));
			//	return false;
			//}
			//
			////-----
			//X509Chain pCertificateChain = new X509Chain();
			//pCertificateChain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
			//pCertificateChain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
			//pCertificateChain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
			//pCertificateChain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
			//
			//return pCertificateChain.Build(pCertificate);
		}

		//---------------------------------------
#endif // UNITY_WSA
	}
}
