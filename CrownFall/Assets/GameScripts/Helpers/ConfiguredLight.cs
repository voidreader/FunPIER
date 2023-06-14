using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class ConfiguredLight : MonoBehaviour
	{
		/////////////////////////////////////////
		//---------------------------------------
		[Header("CONFIG COMMON")]
		[SerializeField]
		[FormerlySerializedAs("m_nRequireRenderingQuality")]
		private int _requireRenderingQuality = 0;
		[SerializeField]
		private bool _castShadow = false;
		[SerializeField]
		private int _requireShadowQuality = 1;

		[Header("CONFIG MOBILE")]
		[SerializeField]
		private ePlatformObject _enablePlatform = ePlatformObject.EnableAll;
		[SerializeField]
		private bool _rangeOverwrapOnMobile = false;
		[SerializeField]
		private float _rangeOverwrap = 1.0f;

		[Header("LIGHT DISABLE INFO")]
		[SerializeField]
		private bool _disableGameObjectTogether = false;


		/////////////////////////////////////////
		//---------------------------------------
		private bool _registEvents = false;
		private Light _light;


		/////////////////////////////////////////
		//---------------------------------------
		void Awake()
		{
			_light = GetComponent<Light>();
			if (_light == null)
			{
				this.enabled = false;
				return;
			}

			bool bLightEnable = HTAfxPref.CheckPlatform(_enablePlatform);
			_light.enabled = bLightEnable;

			if (bLightEnable == false)
			{
				this.enabled = false;
				_light.enabled = false;

				if (_disableGameObjectTogether)
					gameObject.SetActive(false);

				return;
			}

			//-----
			_registEvents = true;
			HTAfxPref.onChangeQuality += OnChangeQuality;

			OnChangeQuality(QualitySettings.GetQualityLevel());
		}

		private void OnDestroy()
		{
			if (_registEvents)
				HTAfxPref.onChangeQuality -= OnChangeQuality;

			_registEvents = false;
		}

		private void OnChangeQuality(int nLevel)
		{
			_light.enabled = ((_requireRenderingQuality <= nLevel) ? true : false);

			bool bShadowCast = _castShadow;
			if (_requireShadowQuality > nLevel)
				bShadowCast = false;

			if (bShadowCast)
				_light.shadows = LightShadows.Hard;
			else
				_light.shadows = LightShadows.None;

			//-----
			if (HTAfxPref.IsMobilePlatform && _rangeOverwrapOnMobile)
				_light.range = _rangeOverwrap;
		}
	}
}
