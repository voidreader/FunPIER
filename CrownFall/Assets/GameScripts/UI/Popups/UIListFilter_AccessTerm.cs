using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace HT
{
	public class UIListFilter_AccessTerm : MonoBehaviour, IContentFiller
	{
		[SerializeField]
		private ListScrollRect _listScrollRect = null;
		[SerializeField]
		private UIListItem_AccessTerm _listItem = null;
		[SerializeField]
		private List<string> _termList = null;

		public void Initialize(List<string> vTermList)
		{
			_termList = vTermList;
		}

		public void BuildList()
		{
			_listScrollRect.RefreshContent();
			_listScrollRect.GoToListItem(0);
		}

		public GameObject GetListItem(int index, int itemType, GameObject obj)
		{
			if (obj == null)
				obj = Instantiate(_listItem.gameObject);

			if (obj != null)
			{
				UIListItem_AccessTerm pComp = obj.GetComponent<UIListItem_AccessTerm>();
				pComp.Init(_termList[index]);
			}

			return obj;
		}

		public int GetItemCount()
		{
			return _termList.Count;
		}

		public int GetItemType(int index)
		{
			return 0;
		}
	}
}
