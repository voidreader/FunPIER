using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Globalization;
using HT;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_Array
	{
		//---------------------------------------
		/// <summary>
		/// Array를 defVal 값으로 채웁니다.
		/// </summary>
		/// <param name="defVal">초기화 값</param>
		public static void Memset<T>(this T[] array, T defVal = default(T))
		{
			if (array == null)
				return;

			for (int nInd = 0; nInd < array.Length; ++nInd)
				array[nInd] = defVal;
		}

		//---------------------------------------
		public static bool IsNullOrEmpty<T>(this T[] array)
		{
			if (array == null || array.Length == 0)
				return true;

			return false;
		}

		public static bool IsNullOrEmpty<T>(this List<T> list)
		{
			if (list == null || list.Count == 0)
				return true;

			return false;
		}

		//---------------------------------------
		public static bool Contain<T>(this T[] list, T value)
		{
			if (list.IsNullOrEmpty())
				return false;

			for (int nInd = 0; nInd < list.Length; ++nInd)
				if (list[nInd].Equals(value))
					return true;

			return false;
		}

		public static int IndexOf<T>(this T[] list, T value)
		{
			if (list.IsNullOrEmpty())
				return -1;

			for (int nInd = 0; nInd < list.Length; ++nInd)
				if (list[nInd].Equals(value))
					return nInd;

			return -1;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_Vector2
	{
		//---------------------------------------
		public static Vector2 yx(this Vector2 vec) { return new Vector2(vec.y, vec.x); }
		public static Vector2 xx(this Vector2 vec) { return new Vector2(vec.x, vec.x); }
		public static Vector2 yy(this Vector2 vec) { return new Vector2(vec.y, vec.y); }

		//---------------------------------------
		public static Vector3 xy0(this Vector2 vec) { return new Vector3(vec.x, vec.y, 0.0f); }
		public static Vector3 x0y(this Vector2 vec) { return new Vector3(vec.x, 0.0f, vec.y); }
		public static Vector3 yx0(this Vector2 vec) { return new Vector3(vec.y, vec.x, 0.0f); }
		public static Vector3 y0x(this Vector2 vec) { return new Vector3(vec.y, 0.0f, vec.x); }

		//---------------------------------------
		public static bool IsNaN(this Vector2 vec) { return (float.IsNaN(vec.x) || float.IsNaN(vec.y)) ? true : false; }

		public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max)
		{
			return new Vector2()
			{
				x = Mathf.Clamp(vec.x, min.x, max.x),
				y = Mathf.Clamp(vec.y, min.y, max.y),
			};
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_Vector3
	{
		//---------------------------------------
		public static Vector2 xy(this Vector3 vec) { return new Vector2(vec.x, vec.y); }
		public static Vector2 yx(this Vector3 vec) { return new Vector2(vec.y, vec.x); }
		public static Vector2 xz(this Vector3 vec) { return new Vector2(vec.x, vec.z); }
		public static Vector2 zx(this Vector3 vec) { return new Vector2(vec.z, vec.x); }
		public static Vector2 yz(this Vector3 vec) { return new Vector2(vec.y, vec.z); }
		public static Vector2 zy(this Vector3 vec) { return new Vector2(vec.z, vec.y); }

		public static Vector2 xx(this Vector3 vec) { return new Vector2(vec.x, vec.x); }
		public static Vector2 yy(this Vector3 vec) { return new Vector2(vec.y, vec.y); }
		public static Vector2 zz(this Vector3 vec) { return new Vector2(vec.z, vec.z); }

		public static Vector3 xzy(this Vector3 vec) { return new Vector3(vec.x, vec.z, vec.y); }
		public static Vector3 yxz(this Vector3 vec) { return new Vector3(vec.y, vec.x, vec.z); }
		public static Vector3 zxy(this Vector3 vec) { return new Vector3(vec.z, vec.x, vec.y); }
		public static Vector3 zyx(this Vector3 vec) { return new Vector3(vec.z, vec.y, vec.x); }

		public static Vector3 xxx(this Vector3 vec) { return new Vector3(vec.x, vec.x, vec.x); }
		public static Vector3 yyy(this Vector3 vec) { return new Vector3(vec.y, vec.y, vec.y); }
		public static Vector3 zzz(this Vector3 vec) { return new Vector3(vec.z, vec.z, vec.z); }

		public static Vector3 x0z(this Vector3 vec) { return new Vector3(vec.x, 0.0f, vec.z); }
		public static Vector3 xy0(this Vector3 vec) { return new Vector3(vec.x, vec.y, 0.0f); }

		//---------------------------------------
		public static bool IsNaN(this Vector3 vec) { return (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z)) ? true : false; }

		public static Vector3 Clamp(this Vector3 vec, Vector3 min, Vector3 max)
		{
			return new Vector3()
			{
				x = Mathf.Clamp(vec.x, min.x, max.x),
				y = Mathf.Clamp(vec.y, min.y, max.y),
				z = Mathf.Clamp(vec.z, min.z, max.z),
			};
		}

		//---------------------------------------
	}

	//---------------------------------------
	static public class Extension_Float
	{
		public static float CutoutInt(this float value)
		{
			return value - Mathf.Floor(value);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_ParticleSystem
	{
		//---------------------------------------
		/// <summary>
		/// ParticleSystem의 Running Time을 구해옵니다.
		/// Repeat 되는 경우를 고려하지 않습니다.
		/// </summary>
		public static float TotalSimulationTime(this ParticleSystem pParticle)
		{
			float fRetval = pParticle.main.startDelayMultiplier;
			fRetval += pParticle.main.startLifetimeMultiplier;
			fRetval += pParticle.main.duration;

			return fRetval;
		}

		//---------------------------------------
		/// <summary>
		/// Play가 완료 된 이후 자동으로 해제되는 복제 ParticleSystem을 생성합니다.
		/// Object Pool을 사용하지 않습니다.
		/// </summary>
		/// <param name="pTransform">참조 대상 Transform</param>
		/// <param name="bInheritPos">대상 Transform의 위치를 참조합니다</param>
		/// <param name="bInheritRot">대상 Transform의 회전을 참조합니다</param>
		/// <param name="bInheritScale">대상 Transform의 크기를 참조합니다</param>
		/// <param name="bInheritParent">대상 Transform의 Hierarchy 하위에 위치시킵니다</param>
		/// <returns>생성 된 ParticleSystem</returns>
		public static ParticleSystem CreateInstance(this ParticleSystem pParticle, Transform pTransform = null, bool bInheritPos = false, bool bInheritRot = false, bool bInheritScale = false, bool bInheritParent = false)
		{
			ParticleSystem pInstance = HT.Utils.Instantiate(pParticle);
			Utils.InheritTransform(pInstance.gameObject, pTransform, bInheritPos, bInheritRot, bInheritScale, bInheritParent);

			Utils.SafeDestroy(pInstance.gameObject, pInstance.TotalSimulationTime());

			return pInstance;
		}

		/// <summary>
		/// Play가 완료 된 이후 자동으로 해제되는 복제 ParticleSystem을 생성합니다.
		/// Object Pool을 사용해서 생성 / 해제 됩니다.
		/// </summary>
		/// <param name="pTransform">참조 대상 Transform</param>
		/// <param name="bInheritPos">대상 Transform의 위치를 참조합니다</param>
		/// <param name="bInheritRot">대상 Transform의 회전을 참조합니다</param>
		/// <param name="bInheritScale">대상 Transform의 크기를 참조합니다</param>
		/// <param name="bInheritParent">대상 Transform의 Hierarchy 하위에 위치시킵니다</param>
		/// <returns>생성 된 ParticleSystem</returns>
		public static ParticleSystem CreateInstanceFromPool(this ParticleSystem pParticle, Transform pTransform = null, bool bInheritPos = false, bool bInheritRot = false, bool bInheritScale = false, bool bInheritParent = false)
		{
			if (pParticle == null)
				return null;

			ParticleSystem pInstance = HT.Utils.InstantiateFromPool(pParticle);
			if (pInstance == null)
				return null;

			Utils.InheritTransform(pInstance.gameObject, pTransform, bInheritPos, bInheritRot, bInheritScale, bInheritParent);

			Utils.SafeDestroy(pInstance.gameObject, pInstance.TotalSimulationTime());

			return pInstance;
		}

		public static ParticleSystem CreateInstanceFromPool(this ParticleSystem pParticle, Vector3 vPos, Vector3 vRot)
		{
			if (pParticle == null)
				return null;

			ParticleSystem pInstance = HT.Utils.InstantiateFromPool(pParticle);
			pInstance.transform.position = vPos;
			pInstance.transform.rotation = Quaternion.Euler(vRot);

			Utils.SafeDestroy(pInstance.gameObject, pInstance.TotalSimulationTime());

			return pInstance;
		}


		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_Camera
	{
		//---------------------------------------
		/// <summary>
		/// Camera를 흔듭니다.
		/// </summary>
		/// <param name="fRatio">Camera의 흔들림 세기</param>
		public static void SetShake(this Camera pCamera, float fRatio)
		{
			if (pCamera == null)
				return;
			
			HTCamera pGameCamera = pCamera.GetComponent<HTCamera>();
			if (pGameCamera == null)
				pGameCamera = pCamera.gameObject.AddComponent<HTCamera>();

			pGameCamera.SetShake(fRatio);
		}

		/// <summary>
		/// Camera를 중앙 기준 방사형으로 흔듭니다.
		/// </summary>
		/// <param name="fRatio">Camera의 흔들림 세기</param>
		public static void SetShakeRadial(this Camera pCamera, float fRatio)
		{
			if (pCamera == null)
				return;

			HTCamera pGameCamera = pCamera.GetComponent<HTCamera>();
			if (pGameCamera == null)
				pGameCamera = pCamera.gameObject.AddComponent<HTCamera>();

			pGameCamera.SetShakeRadial(fRatio);
		}

		/// <summary>
		/// Camera를 특정 방향으로 흔듭니다.
		/// </summary>
		/// <param name="fRatio">Camera의 흔들림 세기</param>
		public static void SetShakeDirection(this Camera pCamera, float fRatio, Vector3 vDir)
		{
			if (pCamera == null)
				return;

			HTCamera pGameCamera = pCamera.GetComponent<HTCamera>();
			if (pGameCamera == null)
				pGameCamera = pCamera.gameObject.AddComponent<HTCamera>();

			pGameCamera.SetShakeDirection(fRatio, vDir.x0z());
		}

		//---------------------------------------
		/// <summary>
		/// Camera를 이동시킵니다.
		/// </summary>
		/// <param name="vTargetPos">이동 위치</param>
		/// <param name="fTime">이동에 소요되는 시간</param>
		public static void MoveTo(this Camera pCamera, Vector3 vTargetPos, float fTime)
		{
			if (pCamera == null)
				return;
			
			HTCamera pGameCamera = pCamera.GetComponent<HTCamera>();
			if (pGameCamera == null)
				pGameCamera = pCamera.gameObject.AddComponent<HTCamera>();

			pGameCamera.MoveTo(vTargetPos, fTime);
		}

		//---------------------------------------
		public static bool IsInCamera(this Camera pCamera, GameObject pGameObj)
		{
			return IsInCamera(pCamera, pGameObj.transform);
		}

		public static bool IsInCamera(this Camera pCamera, Behaviour pBehavior)
		{
			return IsInCamera(pCamera, pBehavior.transform);
		}

		public static bool IsInCamera(this Camera pCamera, Transform pTransform)
		{
			if (pCamera == null)
				return false;

			return IsInCamera(pCamera, pCamera.WorldToViewportPoint(pTransform.position));
		}

		public static bool IsInCamera(this Camera pCamera, Vector3 vViewPoint)
		{
			if (vViewPoint.x < 0.0f || vViewPoint.x > 1.0f)
				return false;

			if (vViewPoint.y < 0.0f || vViewPoint.y > 1.0f)
				return false;

			if (vViewPoint.z < 0.0f || vViewPoint.z > 1.0f)
				return false;

			return true;
		}

		//---------------------------------------
		public static Vector2 WorldToScreenPointProjected(this Camera camera, Vector3 worldPos)
		{
			Vector3 camNormal = camera.transform.forward;
			Vector3 vectorFromCam = worldPos - camera.transform.position;
			float camNormDot = Vector3.Dot(camNormal, vectorFromCam);
			if (camNormDot <= 0)
			{
				// we are behind the camera forward facing plane, project the position in front of the plane
				Vector3 proj = (camNormal * camNormDot * 1.01f);
				worldPos = camera.transform.position + (vectorFromCam - proj);
			}

			return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_Texture2D
	{
		//---------------------------------------
		public static Sprite CreateSprite(this Texture2D pTex)
		{
			if (pTex == null)
				return null;

			return Sprite.Create(pTex, new Rect(0.0f, 0.0f, pTex.width, pTex.height), new Vector2(0.5f, 0.5f));
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_String
	{
		//---------------------------------------
		/// <summary>
		/// 문자열을 Key 값을 기준으로 분할합니다.
		/// </summary>
		/// <param name="cKey">분할 기준 Key</param>
		/// <returns>분할 된 문자열</returns>
		static public string[] Split(this string szSource, char cKey = ',')
		{
			if (string.IsNullOrEmpty(szSource))
				return null;

			return szSource.Trim().Split(cKey);
		}

		//---------------------------------------
		/// <summary>
		/// 문자열을 Boolean 값으로 Parsing합니다.
		/// </summary>
		static public bool ToBoolean(this string szSource)
		{
			szSource = szSource.ToLower();

			try
			{
				if (szSource == "true")
					return true;

				if (szSource == "false")
					return false;

				if (szSource.ToInt() != 0)
					return true;

				return false;
			}
			catch(System.Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// 문자열을 Byte 값으로 Parsing합니다.
		/// </summary>
		static public byte ToByte(this string szSource)
		{
			return byte.Parse(szSource, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 문자열을 Int Array 값으로 Parsing합니다.
		/// </summary>
		static public byte[] ToIntByte(this string szSource, char cKey = ',')
		{
			string[] tokens = Split(szSource, cKey);

			byte[] vRetVals = new byte[tokens.Length];
			for (int nInd = 0; nInd < vRetVals.Length; ++nInd)
				vRetVals[nInd] = tokens[nInd].Trim().ToByte();

			return vRetVals;
		}

		/// <summary>
		/// 문자열을 Int 값으로 Parsing합니다.
		/// </summary>
		static public int ToInt(this string szSource)
		{
			return int.Parse(szSource, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 문자열을 Int Array 값으로 Parsing합니다.
		/// </summary>
		static public int[] ToIntArray(this string szSource, char cKey = ',')
		{
			string[] tokens = Split(szSource, cKey);

			int[] vRetVals = new int[tokens.Length];
			for (int nInd = 0; nInd < vRetVals.Length; ++nInd)
				vRetVals[nInd] = tokens[nInd].Trim().ToInt();

			return vRetVals;
		}

		/// <summary>
		/// 문자열을 Float 값으로 Parsing합니다.
		/// </summary>
		static public float ToFloat(this string szSource)
		{
			return float.Parse(szSource, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 문자열을 Float Array 값으로 Parsing합니다.
		/// </summary>
		static public float[] ToFloatArray(this string szSource, char cKey = ',')
		{
			string[] tokens = Split(szSource, cKey);

			float[] vRetVals = new float[tokens.Length];
			for (int nInd = 0; nInd < vRetVals.Length; ++nInd)
				vRetVals[nInd] = tokens[nInd].Trim().ToFloat();

			return vRetVals;
		}

		//---------------------------------------
		/// <summary>
		/// 문자열을 Vector3 로 Parsing합니다.
		/// </summary>
		static public Vector3 ToVector3(this string szSource, char cKey = ',')
		{
			float[] vArray = szSource.ToFloatArray();
			return new Vector3(vArray[0], vArray[1], vArray[2]);
		}

		/// <summary>
		/// 문자열을 Vector2 로 Parsing합니다.
		/// </summary>
		static public Vector2 ToVector2(this string szSource, char cKey = ',')
		{
			float[] vArray = szSource.ToFloatArray();
			return new Vector2(vArray[0], vArray[1]);
		}

		/// <summary>
		/// 문자열을 Color 로 Parsing합니다.
		/// R G B A 순으로 가져옵니다.
		/// </summary>
		static public Color ToColorRGBA(this string szSource, char cKey = ',')
		{
			float[] vArray = szSource.ToFloatArray();
			return new Color(vArray[0] / 255.0f, vArray[1] / 255.0f, vArray[2] / 255.0f, vArray[3] / 255.0f);
		}

		//---------------------------------------
		/// <summary>
		/// 문자열을 Enum 값으로 Parsing합니다.
		/// </summary>
		static public T ToEnum<T>(this string szSource, T eDefaultValue = default(T))
		{
			return (T)Enum.Parse(typeof(T), szSource);
		}

		//---------------------------------------
		public static string CompressString(this string text)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			var memoryStream = new MemoryStream();
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
			{
				gZipStream.Write(buffer, 0, buffer.Length);
			}

			memoryStream.Position = 0;

			var compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);

			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}

		public static string DecompressString(this string compressedText)
		{
			byte[] gZipBuffer = Convert.FromBase64String(compressedText);
			using (var memoryStream = new MemoryStream())
			{
				int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
				memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

				var buffer = new byte[dataLength];

				memoryStream.Position = 0;
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					gZipStream.Read(buffer, 0, buffer.Length);
				}

				return Encoding.UTF8.GetString(buffer);
			}
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_GameObject
	{
		//---------------------------------------
		public static void ChangeLayer(this GameObject pGameObj, int nTargetLayer, bool bRecursive)
		{
			if (pGameObj == null)
				return;

			pGameObj.layer = nTargetLayer;
			if (bRecursive == false)
				return;

			int nChildCount = pGameObj.transform.childCount;
			for(int nInd = 0; nInd < nChildCount; ++nInd)
			{
				Transform pChildTransform = pGameObj.transform.GetChild(nInd);
				ChangeLayer(pChildTransform.gameObject, nTargetLayer, bRecursive);
			}
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	static public class Extension_Animation
	{
		//---------------------------------------
		public static IEnumerator PlayOnRealtime(this Animation pAnimation, string szClipName, Action pOnComplete)
		{
			AnimationState pCurrState = pAnimation[szClipName];
			bool bIsPlaying = true;
			float fProgressTime = 0f;

			pAnimation.Play(szClipName);

			while (bIsPlaying)
			{
				fProgressTime += TimeUtils.RealTime;
				pCurrState.normalizedTime = fProgressTime / pCurrState.length;
				pAnimation.Sample();

				if (fProgressTime >= pCurrState.length)
				{
					if (pCurrState.wrapMode != WrapMode.Loop)
						bIsPlaying = false;

					else
						fProgressTime = 0.0f;
				}

				yield return new WaitForEndOfFrame();
			}

			Utils.SafeInvoke(pOnComplete);
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	public static class Extension_Enum
	{
		//---------------------------------------
		public static bool Has<T>(this System.Enum type, T value)
		{
			try
			{
				return (((int)(object)type & (int)(object)value) == (int)(object)value);
			}
			catch
			{
				return false;
			}
		}

		public static bool Is<T>(this System.Enum type, T value)
		{
			try
			{
				return (int)(object)type == (int)(object)value;
			}
			catch
			{
				return false;
			}
		}

		//---------------------------------------
		public static T Add<T>(this System.Enum type, T value)
		{
			try
			{
				return (T)(object)(((int)(object)type | (int)(object)value));
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format("Could not append value from enumerated type '{0}'.", typeof(T).Name), ex);
			}
		}
		
		public static T Remove<T>(this System.Enum type, T value)
		{
			try
			{
				return (T)(object)(((int)(object)type & ~(int)(object)value));
			}
			catch (Exception ex)
			{
				throw new ArgumentException(string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name), ex);
			}
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
