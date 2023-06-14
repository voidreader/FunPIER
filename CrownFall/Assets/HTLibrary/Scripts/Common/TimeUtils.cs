using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class TimeUtils
	{
		//---------------------------------------
		public delegate IEnumerator CustomTimeSyncProc(Action<bool,DateTime> pCallback);
		public static CustomTimeSyncProc customTimeSyncProc = null;

		//---------------------------------------
		static Dictionary<int, float> _timeScales = new Dictionary<int, float>();
		public const float frame60DeltaTime = 1.0f / 60.0f;

		//---------------------------------------
		static public float RealTime
		{
			get { return Mathf.Min(GEnv.TIME_MAXDELTA, Time.unscaledDeltaTime); }
		}

		static public float GameTime
		{
			get { return Mathf.Min(GEnv.TIME_MAXDELTA, Time.deltaTime); }
		}

		static public float SmoothGameTime
		{
			get { return Mathf.Min(GEnv.TIME_MAXDELTA, Time.smoothDeltaTime); }
		}

		static public float FixedTime
		{
			get { return Mathf.Min(GEnv.TIME_MAXDELTA, Time.fixedDeltaTime); }
		}

		static public bool IsGamePaused
		{
			get { return (TimeScale <= 0.0f) ? true : false; }
		}

		//---------------------------------------
		static public float TimeScale
		{
			get { return Time.timeScale; }
		}

		static public float GetTime(eTimeType e)
		{
			switch (e)
			{
				case eTimeType.RealTime:
					return RealTime;

				case eTimeType.SmoothGameTime:
					return SmoothGameTime;

				case eTimeType.GameTime:
				default:
					return GameTime;
			}
		}

		//---------------------------------------
		/// <summary>
		/// 특정 Time Layer의 Time Scale을 설정합니다.
		/// 각 Time Layer의 Time Scale은 곱 연산으로 합쳐져 적용됩니다.
		/// </summary>
		/// <param name="fRatio">Time Scale 값</param>
		/// <param name="nTimeLayer">Time Layer의 Index</param>
		static public void SetTimeScale(float fRatio, int nTimeLayer = GEnv.TIMELAYER_GAME)
		{
			float fPrevRatio = 0.0f;
			if (_timeScales.TryGetValue(nTimeLayer, out fPrevRatio))
				_timeScales[nTimeLayer] = fRatio;

			else
			{
				_timeScales.Add(nTimeLayer, fRatio);
			}
			
			float fTimeScale = 1.0f;
			if (_timeScales.Count > 0)
			{
				Dictionary<int, float>.Enumerator pItor = _timeScales.GetEnumerator();
				while (pItor.MoveNext())
				{
					fTimeScale *= pItor.Current.Value;
					if (fTimeScale <= 0.0f)
						break;
				}
			}

			Time.timeScale = Mathf.Max(fTimeScale, 0.0f);
		}

		//---------------------------------------
		public static bool GetNetworkTime(out DateTime ntpServerTime)
		{
			try
			{
				//default Windows time server
				const string ntpServer = "time.windows.com";

				// NTP message size - 16 bytes of the digest (RFC 2030)
				var ntpData = new byte[48];

				//Setting the Leap Indicator, Version Number and Mode values
				ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

				var addresses = Dns.GetHostEntry(ntpServer).AddressList;

				//The UDP port number assigned to NTP is 123
				var ipEndPoint = new IPEndPoint(addresses[0], 123);
				//NTP uses UDP

				using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
				{
					socket.Connect(ipEndPoint);

					//Stops code hang if NTP is blocked
					socket.ReceiveTimeout = 3000;

					socket.Send(ntpData);
					socket.Receive(ntpData);
					socket.Close();
				}

				//Offset to get to the "Transmit Timestamp" field (time at which the reply 
				//departed the server for the client, in 64-bit timestamp format."
				const byte serverReplyTime = 40;

				//Get the seconds part
				ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

				//Get the seconds fraction
				ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

				//Convert From big-endian to little-endian
				intPart = SwapEndianness(intPart);
				fractPart = SwapEndianness(fractPart);

				var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

				//**UTC** time
				var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

				ntpServerTime = networkDateTime.ToLocalTime();
			}
			catch (Exception)
			{
				HTDebug.PrintLog(eMessageType.Error, "[TimeUtils] Failed get time from NTP Server!");
				ntpServerTime = DateTime.UtcNow;
				return false;
			}

			return true;
		}

		static uint SwapEndianness(ulong x)
		{
			return (uint)(((x & 0x000000ff) << 24) +
						   ((x & 0x0000ff00) << 8) +
						   ((x & 0x00ff0000) >> 8) +
						   ((x & 0xff000000) >> 24));
		}


		/////////////////////////////////////////
		//---------------------------------------
		static private bool timeSyncedFromServer = false;
		public static bool TimeSyncedFromServer { get { return timeSyncedFromServer; } }

		static private TimeSpan timeSyncedSpan;

		//---------------------------------------
		public static void TimeSyncRefreshFromServerTime(Action<bool> pOnCallback)
		{
			HTFramework.Instance.StartCoroutine(TimeSyncRefreshFromServerTime_Internal(pOnCallback));
		}
		
		private static IEnumerator TimeSyncRefreshFromServerTime_Internal(Action<bool> pOnCallback)
		{
			bool bRetVal = true;
			DateTime pServerTime = new DateTime();

			do
			{
				if (customTimeSyncProc != null)
				{
					yield return customTimeSyncProc((bool bResult, DateTime pTime)=> 
					{
						bRetVal = bResult;
						pServerTime = pTime;
					});

					if (bRetVal)
						break;

					HTDebug.PrintLog(eMessageType.Error, "[TimeUtils] Failed sync time from custom time sync proc!");
				}

				if (GetNetworkTime(out pServerTime))
					break;

				HTDebug.PrintLog(eMessageType.Error, "[TimeUtils] Failed to sync time from server!");
				bRetVal = false;
			}
			while (false);

			//-----
			timeSyncedFromServer = bRetVal;
			if (bRetVal)
			{
				timeSyncedSpan = pServerTime.Subtract(DateTime.UtcNow);
				HTDebug.PrintLog(eMessageType.None, string.Format("[TimeUtils] Time spans between server [{0}]", timeSyncedSpan.ToString()));
			}
			else
				timeSyncedSpan = new TimeSpan(0);

			Utils.SafeInvoke(pOnCallback, bRetVal);
		}

		//---------------------------------------
		public static DateTime Now()
		{
			return DateTime.Now.Add(timeSyncedSpan);
		}

		public static DateTime UtcNow()
		{
			return DateTime.UtcNow.Add(timeSyncedSpan);
		}


		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}