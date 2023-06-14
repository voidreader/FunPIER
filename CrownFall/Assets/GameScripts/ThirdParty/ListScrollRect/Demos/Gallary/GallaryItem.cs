using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GallaryItem : MonoBehaviour
{
	public string loadingLeftImageUrl;
	public string loadingRightImageUrl;

	public RawImage leftImage;
	public RawImage rightImage;

	public GameObject leftLoadingText;
	public GameObject rightLoadingText;

	public AspectRatioFitter leftAspectRatioFitter;
	public AspectRatioFitter rightAspectRatioFitter;
}
