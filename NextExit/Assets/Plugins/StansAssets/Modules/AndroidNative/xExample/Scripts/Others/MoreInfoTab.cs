using UnityEngine;
using System.Collections;

public sealed class MoreInfoTab : FeatureTab {

	public void OnAskQuestionsClick () {
		AndroidSocialGate.SendMail("Send Mail", "", "Android Native Plugin Question", "stans.assets@gmail.com");
	}

	public void OnGetPluginsClick () {
		string url = "http://goo.gl/g8LWlC";
		Application.OpenURL(url);
	}

	public void OnDocumentationClick () {
		string url = "http://goo.gl/pTcIR8";
		Application.OpenURL(url);
	}

	public void OnMorePluginsClick () {
		string url = "http://goo.gl/MgEirV";
		Application.OpenURL(url);
	}

}
