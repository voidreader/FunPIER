using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTNetworkDebugger : MonoBehaviour
	{
		//---------------------------------------
		[SerializeField]
		private int _graphKillobyte = 512;
		[SerializeField]
		private Text _maxKillobyte = null;
		[SerializeField]
		private Slider _graph_Send = null;
		[SerializeField]
		private Slider _graph_Recv = null;

		//---------------------------------------
		private static long _sendPacketBytes = 0;
		private static long _recvPacketBytes = 0;

		//---------------------------------------
		private void Awake()
		{
			_maxKillobyte.text = string.Format("{0}kb", _graphKillobyte);
			_graph_Send.value = 0.0f;
			_graph_Recv.value = 0.0f;

			//-----
			HTNetwork_Server.Instance.onEveryPacketSend += OnPacketSend;
			HTNetwork_Client.Instance.onEveryPacketSend += OnPacketSend;

			HTNetwork_Server.Instance.onEveryPacketRecieved += OnPacketRecieve;
			HTNetwork_Client.Instance.onEveryPacketRecieved += OnPacketRecieve;
		}

		//---------------------------------------
		private void Update()
		{
			_graph_Send.value = ((_sendPacketBytes / 1024.0f) / _graphKillobyte);
			_graph_Recv.value = ((_recvPacketBytes / 1024.0f) / _graphKillobyte);

			_sendPacketBytes = 0;
			_recvPacketBytes = 0;
		}

		//---------------------------------------
		private void OnPacketSend(HTPacket pPacket)
		{
			_sendPacketBytes += pPacket.DataLength;
		}

		private void OnPacketRecieve(HTPacket pPacket)
		{
			_recvPacketBytes += pPacket.DataLength;
		}

		static public void IncreasePacketBytes_Send(long nByte)
		{
			_sendPacketBytes += nByte;
		}

		static public void IncreasePacketBytes_Recieve(long nByte)
		{
			_recvPacketBytes += nByte;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}