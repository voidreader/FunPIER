using System;
using Toolkit;
using UnityEngine;

public class FunctionBtns : MonoBehaviour
{
	[SerializeField]
	private GameObject m_NOAD;

	[SerializeField]
	private GameObject m_NOShake;

	[SerializeField]
	private GameObject m_Shake;

	[SerializeField]
	private GameObject m_Audio;

	[SerializeField]
	private GameObject m_NOAudio;

	[SerializeField]
	private GameObject m_Restore;

	public void Refresh()
	{
		this.m_NOAudio.gameObject.SetActive(!MonoSingleton<GameDataManager>.Instance.Audio);
		this.m_Audio.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.Audio);
		this.m_Shake.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.PhoneShake);
		this.m_NOShake.gameObject.SetActive(!MonoSingleton<GameDataManager>.Instance.PhoneShake);
		this.m_NOAD.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.AD);
		this.m_Restore.gameObject.SetActive(false);
		if (MonoSingleton<GameDataManager>.Instance.AD)
		{
			this.m_Audio.gameObject.transform.localPosition = Vector3.zero;
			this.m_NOAudio.gameObject.transform.localPosition = Vector3.zero;
		}
		else
		{
			this.m_Audio.gameObject.transform.localPosition = Vector3.left * 130f;
			this.m_NOAudio.gameObject.transform.localPosition = Vector3.left * 130f;
		}
	}
}
