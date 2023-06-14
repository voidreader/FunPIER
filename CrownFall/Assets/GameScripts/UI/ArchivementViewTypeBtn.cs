using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class ArchivementViewTypeBtn : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private eBossType _eType = eBossType.eCommon;
	[SerializeField]
	private ArchivementViewer _viewer = null;

	//---------------------------------------
	private void Awake()
	{
		Toggle pToggle = GetComponent<Toggle>();
		if (pToggle != null)
			pToggle.onValueChanged.AddListener(OnEnabled);

		Button pButton = GetComponent<Button>();
		if (pButton != null)
			pButton.onClick.AddListener(OnClicked);
	}

	private void OnEnabled(bool bOn)
	{
		if (bOn)
			OnClicked();
	}

	private void OnClicked()
	{
		_viewer.OnChangeViewType(_eType);
	}
}


/////////////////////////////////////////
//---------------------------------------