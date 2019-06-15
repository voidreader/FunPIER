using UnityEngine;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;


namespace RPG.AntiVariable
{
    public class HPlayerPrefs
    {
        /// <summary>
        /// encrypt prefix key
        /// </summary>
        public static string Encrypt_Prefix_Key = "abc";
        /// <summary>
        /// encrypt key
        /// </summary>
        public static string Encrypt_Key = "HPlayerPrefs";

        public static string Encrypt(string toEncrypt)
        {
            if (toEncrypt.Length == 0)
                return "";

            string key = MD5(Encrypt_Prefix_Key + Encrypt_Key);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string toDecrypt)
        {
            if (toDecrypt.Length == 0)
                return "";

            string key = MD5(Encrypt_Prefix_Key + Encrypt_Key);
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt.Trim());
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        public static string MD5(string strToEncrypt)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
            return hashString.PadLeft(32, '0');
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static float GetFloat(string key)
        {
            //return PlayerPrefs.GetFloat(key);
            return float.Parse(GetString(key));
        }

        public static float GetFloat(string key, float defaultValue)
        {
            //return PlayerPrefs.GetFloat(key, defaultValue);
            return float.Parse(GetString(key, defaultValue.ToString()));
        }

        public static int GetInt(string key)
        {
            //return PlayerPrefs.GetInt(key);
            return int.Parse(GetString(key));
        }

        public static int GetInt(string key, int defaultValue)
        {
            //return PlayerPrefs.GetInt(key, defaultValue);
            return int.Parse(GetString(key, defaultValue.ToString()));
        }

        public static string GetString(string key)
        {
            //return PlayerPrefs.GetString(key);
            return Decrypt(PlayerPrefs.GetString(key));
        }

        public static string GetString(string key, string defaultValue)
        {
            //return PlayerPrefs.GetString(key, defaultValue);
            return Decrypt(PlayerPrefs.GetString(key, Encrypt(defaultValue)));
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public static void SetFloat(string key, float value)
        {
            //PlayerPrefs.SetFloat(key, value);
            SetString(key, value.ToString());
        }

        public static void SetInt(string key, int value)
        {
            //PlayerPrefs.SetInt(key, value);
            SetString(key, value.ToString());
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, Encrypt(value));
        }


    }
}
