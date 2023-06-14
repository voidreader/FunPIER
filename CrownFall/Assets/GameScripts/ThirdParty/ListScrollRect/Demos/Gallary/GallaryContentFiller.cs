using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GallaryContentFiller : MonoBehaviour, IContentFiller
{
	#region Inspector Variables
	
	[SerializeField] private ListScrollRect listScrollRect;
	[SerializeField] private GameObject		verticalListItem;
	[SerializeField] private GameObject		horizontalListItem;
	[SerializeField] private int			count;

	#endregion

	#region Member Variables

	private Dictionary<string, string>	loadedImageUrls	= new Dictionary<string, string>();
	private Dictionary<string, Texture>	cachedTextures	= new Dictionary<string, Texture>();

	#endregion

	#region Public Methods

	#region IContentFiller Methods

	public GameObject GetListItem(int index, int itemType, GameObject obj)
	{
		if (obj == null)
		{
			obj = Instantiate(listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical ? verticalListItem : horizontalListItem);
		}

		string leftKey	= string.Format("{0}_left", index);
		string rightKey	= string.Format("{0}_right", index);

		string leftUrl	= "";
		string rightUrl	= "";

		// Get the url that goes in the left image
		if (!loadedImageUrls.ContainsKey(leftKey))
		{
			leftUrl = GetRandomImageUrl();
			loadedImageUrls.Add(leftKey, leftUrl);
		}
		else
		{
			leftUrl = loadedImageUrls[leftKey];
		}


		// Get the image that goes in the right image
		if (!loadedImageUrls.ContainsKey(rightKey))
		{
			rightUrl = GetRandomImageUrl();
			loadedImageUrls.Add(rightKey, rightUrl);
		}
		else
		{
			rightUrl = loadedImageUrls[rightKey];
		}

		GallaryItem	gallaryItem = obj.GetComponent<GallaryItem>();

		StartCoroutine(loadImage(leftUrl, gallaryItem, true, leftKey));
		StartCoroutine(loadImage(rightUrl, gallaryItem, false, rightKey));

		return obj;
	}
	
	public int GetItemCount()
	{
		return count;
	}
	
	public int GetItemType(int index)
	{
		return 0;
	}

	#endregion

	private IEnumerator loadImage(string url, GallaryItem gallaryItem, bool left, string key)
	{
		string loadingUrl = left ? gallaryItem.loadingLeftImageUrl : gallaryItem.loadingRightImageUrl;

		// If the url we want to load equals the url we are currntly loading then return
		if (url == loadingUrl)
		{
			yield return null;
		}
		else if (cachedTextures.ContainsKey(url))
		{
			SetTexture(cachedTextures[url], gallaryItem, left);
			yield return null;
		}
		else
		{
			if (gallaryItem != null)
			{
				// Set the url we are loading
				if (left)
				{
					gallaryItem.loadingLeftImageUrl = url;
					gallaryItem.leftImage.gameObject.SetActive(false);
					gallaryItem.leftLoadingText.SetActive(true);
				}
				else
				{
					gallaryItem.loadingRightImageUrl = url;
					gallaryItem.rightImage.gameObject.SetActive(false);
					gallaryItem.rightLoadingText.SetActive(true);
				}

				// Load the image
				WWW www = new WWW(url);

				yield return www;

				loadingUrl = left ? gallaryItem.loadingLeftImageUrl : gallaryItem.loadingRightImageUrl;

				// If the url no longer equals loadingLeftImageUrl then a newer image is try to load
				if (url == loadingUrl)
				{
					if (www.texture.width < 200 && www.texture.height < 200)
					{
						loadedImageUrls[key] = GetRandomImageUrl();
						StartCoroutine(loadImage(loadedImageUrls[key], gallaryItem, left, key));
					}
					else
					{
						cachedTextures[url] = www.texture;
						SetTexture(www.texture, gallaryItem, left);
					}
				}
			}
		}
	}

	private string GetRandomImageUrl()
	{
		return string.Format("http://placekitten.com/{0}/{1}", 10 * Random.Range(20, 50), 10 * Random.Range(20, 50));
	}

	private void SetTexture(Texture texture, GallaryItem gallaryItem, bool left)
	{
		if (gallaryItem == null)
		{
			return;
		}

		if (left)
		{
			gallaryItem.loadingLeftImageUrl					= "";
			gallaryItem.leftImage.texture					= texture;
			gallaryItem.leftAspectRatioFitter.aspectRatio	= (float)texture.width / (float)texture.height;
			gallaryItem.leftImage.gameObject.SetActive(true);
			gallaryItem.leftLoadingText.SetActive(false);
		}
		else
		{
			gallaryItem.loadingRightImageUrl				= "";
			gallaryItem.rightImage.texture					= texture;
			gallaryItem.rightAspectRatioFitter.aspectRatio	= (float)texture.width / (float)texture.height;
			gallaryItem.rightImage.gameObject.SetActive(true);
			gallaryItem.rightLoadingText.SetActive(false);
		}
	}

	/// <summary>
	/// Changes the scroll direction.
	/// </summary>
	public void SetScrollDirection(bool vertical)
	{
		if ((vertical && listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ||
		    (!vertical && listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Horizontal))
		{
			return;
		}

		loadedImageUrls.Clear();
		cachedTextures.Clear();
		
		listScrollRect.content.anchorMin = Vector2.zero;
		listScrollRect.content.anchorMax = Vector2.one;
		
		listScrollRect.content.offsetMax = Vector2.zero;
		listScrollRect.content.offsetMin = Vector2.zero;
		
		listScrollRect.RebuildContent(vertical ? ListScrollRect.ScrollDirection.Vertical : ListScrollRect.ScrollDirection.Horizontal);
	}

	#endregion
}
