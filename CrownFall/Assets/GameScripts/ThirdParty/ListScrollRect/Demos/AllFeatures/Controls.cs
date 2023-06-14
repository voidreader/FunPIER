using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Controls : MonoBehaviour
{
	#region Inspector Variables

	[SerializeField] private ListScrollRect	listScrollRect;
	[SerializeField] private GameObject		contentFillerOne;
	[SerializeField] private GameObject		contentFillerTwo;
	[SerializeField] private Dropdown		contentFiller;
	[SerializeField] private Dropdown		scrollDirection;
	[SerializeField] private InputField		paddingTopLeft;
	[SerializeField] private InputField		paddingBottomRight;
	[SerializeField] private Text			paddingTopLeftText;
	[SerializeField] private Text			paddingBottomRightText;
	[SerializeField] private InputField		spacing;
	[SerializeField] private InputField		itemCount;
	[SerializeField] private InputField		gotoListItem;
	[SerializeField] private InputField		scrollToListItem;
	[SerializeField] private InputField		maxTexts;
	[SerializeField] private InputField		minTexts;
	[SerializeField] private Toggle			randomEverytime;
	[SerializeField] private GameObject		contentFillerTwoContainer;
	[SerializeField] private GameObject		randomEverythingContainer;
	[SerializeField] private Text			refreshButtonText;
	[SerializeField] private Text			rebuildButtonText;

	#endregion

	#region Member Variables
	
	private int		prevContentFiller;
	private int		prevScrollDirection;
	private string	prevPaddingTopLeft;
	private string	prevPaddingBottomRight;
	private string	prevSpacing;
	private string	prevItemCount;
	private string	prevMinTexts;
	private string	prevMaxTexts;
	private bool	prevRandomEverything;

	#endregion

	#region Properties
	#endregion

	#region Static Methods
	#endregion

	#region Unity Methods

	private void Start()
	{
		// We need the ListScrollRect to be initialize right now, so call Initialize
		if (listScrollRect.Initialize())
		{
			UpdateControls();
			UpdateButtons();
		}
	}

	#endregion

	#region Public Methods

	public void RefreshList()
	{
		if (NeedsRefresh())
		{
			UpdateItemCount();

			float[] updatedContent = GetUpdatedRefreshContent();
			listScrollRect.RefreshContent(updatedContent[0], updatedContent[1], updatedContent[2], updatedContent[3], updatedContent[4]);
		}
		else
		{
			listScrollRect.RefreshContent();
		}

		UpdateRefreshControls();
		UpdateButtons();
	}

	public void RebuildList()
	{
		if (NeedsRebuild())
		{
			// Get the content filler we are going to be using
			IContentFiller iContentFiller;
			if (contentFiller.value == 0)
			{
				iContentFiller = contentFillerOne.GetComponent(typeof(IContentFiller)) as IContentFiller;
			}
			else
			{
				iContentFiller = contentFillerTwo.GetComponent(typeof(IContentFiller)) as IContentFiller;
			}

			// Get the currently selected scrollDirection
			ListScrollRect.ScrollDirection scrollDir = (scrollDirection.value == 0) ? ListScrollRect.ScrollDirection.Vertical : ListScrollRect.ScrollDirection.Horizontal;

			// Set the anchors and offsets to fill the parent, when RebuildContent is called it will set the anchors it needs
			listScrollRect.content.anchorMin = Vector2.zero;
			listScrollRect.content.anchorMax = Vector2.one;
			listScrollRect.content.offsetMax = Vector2.zero;
			listScrollRect.content.offsetMin = Vector2.zero;

			// Set the values in the content filler
			if (contentFiller.value == 0)
			{
				((ContentFillerOne)iContentFiller).IsVertical = (scrollDir == ListScrollRect.ScrollDirection.Vertical);
			}
			else
			{
				((ContentFillerTwo)iContentFiller).IsVertical = (scrollDir == ListScrollRect.ScrollDirection.Vertical);
			
				// Get the min/max texts count
				int minTextsInt = 0;
				int maxTextsInt = 0;

				if (int.TryParse(minTexts.text, out minTextsInt))
				{
					((ContentFillerTwo)iContentFiller).MinTextStrCount = minTextsInt;
				}

				if (int.TryParse(maxTexts.text, out maxTextsInt))
				{
					((ContentFillerTwo)iContentFiller).MaxTextStrCount = maxTextsInt;
				}

				if (((ContentFillerTwo)iContentFiller).MinTextStrCount < 1)
				{
					((ContentFillerTwo)iContentFiller).MinTextStrCount = 1;
				}

				if (((ContentFillerTwo)iContentFiller).MaxTextStrCount < 1)
				{
					((ContentFillerTwo)iContentFiller).MaxTextStrCount = 1;
				}

				if (((ContentFillerTwo)iContentFiller).MinTextStrCount > ((ContentFillerTwo)iContentFiller).MaxTextStrCount)
				{
					((ContentFillerTwo)iContentFiller).MinTextStrCount = ((ContentFillerTwo)iContentFiller).MaxTextStrCount;
				}
			}

			UpdateItemCount();

			float[] updatedContent = GetUpdatedRefreshContent();

			listScrollRect.RebuildContent(iContentFiller, scrollDir, updatedContent[0], updatedContent[1], updatedContent[2], updatedContent[3], updatedContent[4]);
		}
		else if (NeedsRefresh())
		{
			RefreshList();
		}
		else
		{
			listScrollRect.RebuildContent();
		}

		UpdateControls();
		UpdateButtons();
	}

	public void UpdateButtons()
	{
		refreshButtonText.fontStyle = NeedsRefresh() ? FontStyle.Bold : FontStyle.Normal;
		rebuildButtonText.fontStyle = NeedsRebuild() ? FontStyle.Bold : FontStyle.Normal;
	}

	public void GotoListItem()
	{
		int listItemIndex = 0;

		if (int.TryParse(gotoListItem.text, out listItemIndex))
		{
			listScrollRect.GoToListItem(listItemIndex);
		}
		else
		{
			Debug.LogError("GotoListItem needs to be an integer.");
		}
	}

	public void ScrollToListItem()
	{
		int itemIndex = 0;

		if (int.TryParse(scrollToListItem.text, out itemIndex))
		{
			listScrollRect.ScrollToListItem(itemIndex);
		}
		else
		{
			Debug.LogError("ScrollToListItem needs to be an integer.");
		}
	}

	public void RandomEverythingToggled()
	{
		(contentFillerTwo.GetComponent(typeof(IContentFiller)) as ContentFillerTwo).RandomEverytime = randomEverytime.isOn;
	}

	#endregion

	#region Private Methods

	private void UpdateItemCount()
	{
		// Get item count
		int itemCountInt = 0;

		if (int.TryParse(itemCount.text, out itemCountInt))
		{
			// Parsing the item count failed, fall back to the already set item count
			if (contentFiller.value == 0)
			{
				((ContentFillerOne)contentFillerOne.GetComponent(typeof(IContentFiller))).ItemCount	= itemCountInt;
			}
			else
			{
				((ContentFillerTwo)contentFillerTwo.GetComponent(typeof(IContentFiller))).ItemCount	= itemCountInt;
			}
		}
	}

	private void UpdateRefreshControls()
	{
		itemCount.text				= listScrollRect.ContentFillerInterface.GetItemCount().ToString();
		paddingTopLeftText.text		= string.Format("Padding {0}:", (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? "Top" : "Left");
		paddingBottomRightText.text	= string.Format("Padding {0}:", (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? "Bottom" : "Right");
		paddingTopLeft.text			= (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? listScrollRect.PaddingTop.ToString() : listScrollRect.PaddingLeft.ToString();
		paddingBottomRight.text		= (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? listScrollRect.PaddingBottom.ToString() : listScrollRect.PaddingRight.ToString();
		spacing.text				= listScrollRect.Spacing.ToString();

		prevPaddingTopLeft		= paddingTopLeft.text;
		prevPaddingBottomRight	= paddingBottomRight.text;
		prevSpacing				= spacing.text;
		prevItemCount			= itemCount.text;
	}

	private void UpdateControls()
	{
		UpdateRefreshControls();

		bool usingContentFillerOne = (listScrollRect.ContentFillerInterface == (contentFillerOne.GetComponent(typeof(IContentFiller)) as IContentFiller));

		contentFiller.value		= usingContentFillerOne ? 0 : 1;
		scrollDirection.value	= (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical) ? 0 : 1;
		minTexts.text 			= (contentFillerTwo.GetComponent(typeof(IContentFiller)) as ContentFillerTwo).MinTextStrCount.ToString();
		maxTexts.text 			= (contentFillerTwo.GetComponent(typeof(IContentFiller)) as ContentFillerTwo).MaxTextStrCount.ToString();

		contentFillerTwoContainer.SetActive(!usingContentFillerOne);
		randomEverythingContainer.SetActive(!usingContentFillerOne);

		prevContentFiller		= contentFiller.value;
		prevScrollDirection		= scrollDirection.value;
		prevMinTexts 			= minTexts.text;
		prevMaxTexts 			= maxTexts.text;
	}

	private bool NeedsRebuild()
	{
		return	prevContentFiller != contentFiller.value ||
				prevScrollDirection != scrollDirection.value ||
				prevMinTexts != minTexts.text ||
				prevMaxTexts != maxTexts.text;
	}

	private bool NeedsRefresh()
	{
		return	prevPaddingTopLeft != paddingTopLeft.text ||
				prevPaddingBottomRight != paddingBottomRight.text ||
				prevSpacing != spacing.text ||
				prevItemCount != itemCount.text;
	}

	private float[] GetUpdatedRefreshContent()
	{
		bool	isVertical		= (listScrollRect.ScrollDir == ListScrollRect.ScrollDirection.Vertical);
		float	paddingInput1	= 0;
		float	paddingInput2	= 0;
		float	spacingInput	= 0;

		if (!float.TryParse(paddingTopLeft.text, out paddingInput1))
		{
			paddingInput1 = isVertical ? listScrollRect.PaddingTop : listScrollRect.PaddingLeft;
		}

		if (!float.TryParse(paddingBottomRight.text, out paddingInput2))
		{
			paddingInput2 = isVertical ? listScrollRect.PaddingBottom : listScrollRect.PaddingRight;
		}

		if (!float.TryParse(spacing.text, out spacingInput))
		{
			spacingInput = listScrollRect.Spacing;
		}

		float paddingTop	= isVertical ? paddingInput1 : listScrollRect.PaddingTop;
		float paddingBottom	= isVertical ? paddingInput2 : listScrollRect.PaddingBottom;
		float paddingLeft	= !isVertical ? paddingInput1 : listScrollRect.PaddingLeft;
		float paddingRight	= !isVertical ? paddingInput2 : listScrollRect.PaddingRight;

		return new float[] { paddingLeft, paddingRight, paddingTop, paddingBottom, spacingInput };
	}

	#endregion
}
