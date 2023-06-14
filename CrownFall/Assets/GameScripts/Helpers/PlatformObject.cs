using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
namespace HT
{
	public class PlatformObject : MonoBehaviour
	{
		//---------------------------------------
		[SerializeField]
		private ePlatform _suboardinatePlatform = ePlatform.PC;

		//---------------------------------------
		private void Awake()
		{
			bool bIsMobilePlatform = HTAfxPref.IsMobilePlatform;
			switch (_suboardinatePlatform)
			{
				case ePlatform.PC:
					if (bIsMobilePlatform)
						gameObject.SetActive(false);

					break;

				case ePlatform.Mobile:
					if (bIsMobilePlatform == false)
						gameObject.SetActive(false);

					break;
			}
		}

		//---------------------------------------
	}
}


/////////////////////////////////////////
//---------------------------------------