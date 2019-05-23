using UnityEngine;
using System.Collections.Generic;

public class KyOptionSound : KyButtonGroup {

	// Update is called once per frame
	public new void Update () {
		base.Update();
		int downIndex = -1;
		foreach (KyButton button in mChildButtons) {
			KyButton.EventType flag = button.EventFlag;
			if ((flag & KyButton.EventType.ButtonDown) != 0) {
				KyAudioManager.Instance.MasterVolume = KyApplication.Instance.GetVolumeFromIndex(button.Index);
				//KyAudioManager.Instance.PlayOneShot("se_ok");
				downIndex = button.Index;
				break;
			}
		}
		if (downIndex >= 0) {
			foreach (KyButton button in mChildButtons) {
				if (button.Index > downIndex) {
					button.Selected = false;
				} else if (button.Index < downIndex) {
					button.Selected = true;
				}
			}
			Selected = true;
		}
	}

	public override void OnChildButtonSelected(KyButton child) {
		mSelectedButton = child;
	}
}
