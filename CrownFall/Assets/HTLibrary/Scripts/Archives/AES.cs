using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_WSA
#else // UNITY_WSA
using System.IO;
using System.Security.Cryptography;

namespace HT
{
	class AES
	{
		public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
		{
			if (bytesToBeEncrypted.Length == 0)
				return bytesToBeEncrypted;

			byte[] encryptedBytes = null;

			// Set your salt here, change it to meet your flavor:
			byte[] saltBytes = passwordBytes;
			// Example:
			//saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

			using (MemoryStream ms = new MemoryStream())
			{
				using (RijndaelManaged AES = new RijndaelManaged())
				{
					AES.KeySize = 256;
					AES.BlockSize = 128;

					var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
					AES.Key = key.GetBytes(AES.KeySize / 8);
					AES.IV = key.GetBytes(AES.BlockSize / 8);

					AES.Mode = CipherMode.CBC;

					using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
						cs.Close();
					}
					encryptedBytes = ms.ToArray();
				}
			}

			return encryptedBytes;
		}

		public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
		{
			if (bytesToBeDecrypted.Length == 0)
				return bytesToBeDecrypted;

			byte[] decryptedBytes = null;
			// Set your salt here to meet your flavor:
			byte[] saltBytes = passwordBytes;
			// Example:
			//saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

			using (MemoryStream ms = new MemoryStream())
			{
				using (RijndaelManaged AES = new RijndaelManaged())
				{
					AES.KeySize = 256;
					AES.BlockSize = 128;

					var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
					AES.Key = key.GetBytes(AES.KeySize / 8);
					AES.IV = key.GetBytes(AES.BlockSize / 8);

					AES.Mode = CipherMode.CBC;

					using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
						cs.Close();
					}
					decryptedBytes = ms.ToArray();
				}
			}

			return decryptedBytes;
		}

		public static string Encrypt(string text, string password)
		{
			return Encrypt(text, Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(password)));
		}

		public static string Encrypt(string text, byte[] passwordBytes)
		{
			return Encrypt(Encoding.UTF8.GetBytes(text), passwordBytes);
		}

		public static string Encrypt(byte[] originalBytes, string password)
		{
			return Encrypt(originalBytes, Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(password)));
		}

		public static string Encrypt(byte[] originalBytes, byte[] passwordBytes)
		{
			return Convert.ToBase64String(EncryptBytes(originalBytes, passwordBytes));
		}

		public static byte[] EncryptBytes(byte[] originalBytes, byte[] passwordBytes)
		{
			byte[] encryptedBytes = null;

			// Hash the password with SHA256
			passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

			// Getting the salt size
			int saltSize = GetSaltSize(passwordBytes);
			// Generating salt bytes
			byte[] saltBytes = GetRandomBytes(saltSize);

			// Appending salt bytes to original bytes
			byte[] bytesToBeEncrypted = new byte[saltBytes.Length + originalBytes.Length];
			for (int i = 0; i < saltBytes.Length; i++)
			{
				bytesToBeEncrypted[i] = saltBytes[i];
			}
			for (int i = 0; i < originalBytes.Length; i++)
			{
				bytesToBeEncrypted[i + saltBytes.Length] = originalBytes[i];
			}

			encryptedBytes = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
			return encryptedBytes;
		}
		public static string Decrypt(string text, string password)
		{
			return Decrypt(text, Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(password)));
		}
		public static string Decrypt(string decryptedText, byte[] passwordBytes)
		{
			byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);
			byte[] originalBytes = DecryptBytes(bytesToBeDecrypted, passwordBytes);
			return Encoding.UTF8.GetString(originalBytes);
		}

		public static byte[] DecryptBytes(string text, string password)
		{
			return DecryptBytes(text, Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(password)));
		}

		public static byte[] DecryptBytes(string decryptedText, byte[] passwordBytes)
		{
			byte[] bytesToBeDecrypted = Convert.FromBase64String(decryptedText);
			return DecryptBytes(bytesToBeDecrypted, passwordBytes);
		}

		public static byte[] DecryptBytes(byte[] bytesToBeDecrypted, byte[] passwordBytes)
		{
			// Hash the password with SHA256
			passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

			byte[] decryptedBytes = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

			// Getting the size of salt
			int saltSize = GetSaltSize(passwordBytes);

			// Removing salt bytes, retrieving original bytes
			byte[] originalBytes = new byte[decryptedBytes.Length - saltSize];
			for (int i = saltSize; i < decryptedBytes.Length; i++)
			{
				originalBytes[i - saltSize] = decryptedBytes[i];
			}

			return originalBytes;
		}

		public static int GetSaltSize(byte[] passwordBytes)
		{
			var key = new Rfc2898DeriveBytes(passwordBytes, passwordBytes, 1000);
			byte[] ba = key.GetBytes(2);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < ba.Length; i++)
			{
				sb.Append(Convert.ToInt32(ba[i]).ToString());
			}
			int saltSize = 0;
			string s = sb.ToString();
			foreach (char c in s)
			{
				int intc = Convert.ToInt32(c.ToString());
				saltSize = saltSize + intc;
			}

			return saltSize;
		}

		public static byte[] GetRandomBytes(int length)
		{
			byte[] ba = new byte[length];
			RNGCryptoServiceProvider.Create().GetBytes(ba);
			return ba;
		}
	}
}

#endif // UNITY_WSA
