using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class AudioManager : MonoSingleton<AudioManager>
{
	[SerializeField]
	private List<AudioClip> m_Combine = new List<AudioClip>();

	[SerializeField]
	private AudioClip m_Hit;

	[SerializeField]
	private AudioClip m_Money;

	protected override void Awake()
	{
		base.Awake();
		base.gameObject.SetActive(true);
	}

	public void PlayCombineAudio()
	{
		if (MonoSingleton<GameDataManager>.Instance.Audio && this.m_Combine != null && this.m_Combine.Count > 0)
		{
			AudioSource.PlayClipAtPoint(this.m_Combine[0], base.transform.position);
		}
	}

	public void PlayCombineAudio(int count)
	{
		if (MonoSingleton<GameDataManager>.Instance.Audio && this.m_Combine != null && this.m_Combine.Count > 0)
		{
			AudioSource.PlayClipAtPoint((count <= this.m_Combine.Count - 1) ? this.m_Combine[count] : this.m_Combine[this.m_Combine.Count - 1], base.transform.position);
		}
	}

	public void PlayHitAudio()
	{
		if (MonoSingleton<GameDataManager>.Instance.Audio)
		{
			AudioSource.PlayClipAtPoint(this.m_Hit, base.transform.position);
		}
	}

	public void PlayMoneyAudio()
	{
		if (MonoSingleton<GameDataManager>.Instance.Audio)
		{
			AudioSource.PlayClipAtPoint(this.m_Money, base.transform.position);
		}
	}
}
