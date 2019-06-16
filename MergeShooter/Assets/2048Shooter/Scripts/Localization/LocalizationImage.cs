using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationImage : MonoBehaviour
{
	[SerializeField]
	private string path;

	[SerializeField]
	private string spriteName;

	private void OnEnable()
	{
		Singleton<Localization>.instance.OnChangeLanguage += new Action(this.OnChangeLanguage);
		this.OnChangeLanguage();
	}

	private void OnDisable()
	{
		Singleton<Localization>.instance.OnChangeLanguage -= new Action(this.OnChangeLanguage);
	}

	private void OnDestroy()
	{
		Singleton<Localization>.instance.OnChangeLanguage -= new Action(this.OnChangeLanguage);
	}

	private void OnChangeLanguage()
	{
		Image component = base.GetComponent<Image>();
		if (component != null)
		{
			string path = (!string.IsNullOrEmpty(this.path)) ? this.path : "UI";
			Sprite sprite = Resources.Load<Sprite>(Path.Combine(path, this.spriteName + "_" + Singleton<Localization>.instance.CurrentLanguage));
			component.sprite = sprite;
		}
	}
}
