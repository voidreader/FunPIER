using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class ResourceUtils
	{
		//---------------------------------------
		/// <summary>
		/// Resource를 가져옵니다.
		/// </summary>
		static public UnityEngine.Object Load(string szAddress)
		{
			return Resources.Load(szAddress);
		}

		/// <summary>
		/// Resource를 가져옵니다.
		/// </summary>
		static public T Load<T>(string szAddress) where T : UnityEngine.Object
		{
			return Resources.Load(szAddress, typeof(T)) as T;
		}

		static public void SafeUnload<T>(ref T pObject) where T : UnityEngine.Object
		{
			if (pObject != null)
				Resources.UnloadAsset(pObject);

			pObject = null;
		}

		//---------------------------------------
		/// <summary>
		/// 해당 Address 내에 있는 모든 Resource를 가져옵니다.
		/// </summary>
		static public List<UnityEngine.Object> LoadAll(string szAddress)
		{
			List<UnityEngine.Object> vList = new List<UnityEngine.Object>();

			UnityEngine.Object[] vObjs = Resources.LoadAll(szAddress);
			if (vObjs != null && vObjs.Length > 0)
				for (int nInd = 0; nInd < vObjs.Length; ++nInd)
					vList.Add(vObjs[nInd]);

			return vList;
		}

		//---------------------------------------
		/// <summary>
		/// Resource에서 XML Document를 Parsing합니다.
		/// </summary>
		public static System.Xml.XmlDocument LoadXMLFromResource(string szResourceAddr)
		{
			UnityEngine.Object pObj = Load(szResourceAddr);
			TextAsset pAsset = pObj as TextAsset;
			if (pAsset == null)
			{
				HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTUtils] {0} is not exist!", szResourceAddr));
				return null;
			}

			return ConvertToXML(pAsset);
		}

		public static System.Xml.XmlDocument ConvertToXML(TextAsset pAsset)
		{
			System.Xml.XmlDocument pXML = new System.Xml.XmlDocument();
			pXML.LoadXml(pAsset.text);

			return pXML;
		}

		//---------------------------------------
		/// <summary>
		/// Resource에서 XML Document를 Parsing합니다.
		/// </summary>
		public static JSONNode LoadJsonFromResource(string szResourceAddr)
		{
			UnityEngine.Object pObj = Load(szResourceAddr);
			TextAsset pAsset = pObj as TextAsset;
			if (pAsset == null)
			{
				HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTUtils] {0} is not exist!", szResourceAddr));
				return null;
			}

			return ConvertToJson(pAsset);
		}

		public static JSONNode ConvertToJson(TextAsset pAsset)
		{
			byte[] bytes = CryptUtils.BlowFish.Decrypt_CBC(pAsset.bytes);
			return JSON.Parse(Encoding.UTF8.GetString(bytes));
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 현재 화면을 Texture로 추출합니다.
		/// </summary>
		/// <param name="onComplete">Texture 생성이 완료되면 호출되는 Callback입니다</param>
		/// <param name="fWaitTime">Texture 생성 지연 시간</param>
		/// <param name="pRect">화면 범위</param>
		static public void CreateScreenShot(Action<Texture2D> onComplete, float fWaitTime, Rect pRect)
		{
			HTFramework.Instance.StartCoroutine(CreateScreenShot_Internal(onComplete, fWaitTime, pRect));
		}

		static private IEnumerator CreateScreenShot_Internal(Action<Texture2D> onComplete, float fWaitTime, Rect pRect)
		{
			if (fWaitTime > 0.0f)
				yield return new WaitForSecondsRealtime(fWaitTime);

			yield return new WaitForEndOfFrame();

			Texture2D pTexture = new Texture2D((int)pRect.width, (int)pRect.height, TextureFormat.RGB24, false);
			pTexture.ReadPixels(pRect, 0, 0);
			pTexture.Apply();

			Utils.SafeInvoke(onComplete, pTexture);
		}


		/////////////////////////////////////////
		//---------------------------------------
		static public void RefreshSkinnedMeshBones(SkinnedMeshRenderer pOriginal, SkinnedMeshRenderer pTarget)
		{
			Transform[] vNewBones = new Transform[pTarget.bones.Length];
			for (int nNewBone = 0; nNewBone < pOriginal.bones.Length; nNewBone++)
			{
				Transform originalBone = pOriginal.bones[nNewBone];
				for (int nTargetBone = 0; nTargetBone < pTarget.bones.Length; ++nTargetBone)
				{
					Transform newBone = pTarget.bones[nTargetBone];
					if (newBone != null && newBone.name == originalBone.name)
					{
						vNewBones[nNewBone] = pTarget.bones[nTargetBone];
						continue;
					}
				}
			}

			//-----
			pTarget.sharedMesh = pOriginal.sharedMesh;
			pTarget.bones = vNewBones;
		}


		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
