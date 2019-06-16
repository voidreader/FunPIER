using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class Brick : MonoBehaviour
{
	/*private sealed class _MoveDir_c__AnonStorey0
	{
		internal bool destory;

		internal Brick _this;

		internal void __m__0()
		{
			this._this.m_Tween = false;
			this._this.m_Destory = this.destory;
		}
	}*/

	public int m_Array;

	public int m_Index;

	public int m_Number;

	public Image m_Image;

	public bool m_Tween;

	public bool m_Destory;

	public bool m_Boomb;

	public int m_Count;

	[SerializeField]
	private float m_Time = 0.2f; // 블럭 움직이는 속도

	// ANCHOR Brick Init, array: 가로 인덱스, index: 세로 인덱스
	public void Init(int array, int index, int number, int maxvalue)
	{
		this.m_Array = array;
		this.m_Index = index;
		this.m_Number = number;
		this.m_Image.raycastTarget = false;
		this.UpdateSize();
		this.m_Image.sprite = MonoSingleton<ConfigeManager>.Instance.GetValueIcon((this.m_Number <= maxvalue) ? this.m_Number : (-1));
	
		this.m_Tween = false; // 블럭 움직일 동안 터치 안되게
	}

	public void RefreshNumber(int number, int maxvalue)
	{
		this.m_Number = number;
		this.UpdateSize();
		this.m_Image.sprite = MonoSingleton<ConfigeManager>.Instance.GetValueIcon((this.m_Number <= maxvalue) ? this.m_Number : (-1));
	}

	public void RefreshNumber(int maxvalue)
	{
		this.m_Number *= (int)Mathf.Pow(2f, (float)this.m_Count);
		this.m_Count = 0;
		this.UpdateSize();
		this.m_Image.sprite = MonoSingleton<ConfigeManager>.Instance.GetValueIcon((this.m_Number <= maxvalue) ? this.m_Number : (-1));
	}

	public void UpdateSize()
	{
		this.m_Image.transform.GetComponent<RectTransform>().sizeDelta = ((MonoSingleton<GameDataManager>.Instance.UseSkinType != SkinType.NEON) ? (Vector2.one * 101f) : (Vector2.one * 120f));
	}
	
	// 블럭 합체 중
	private void MoveDir(Vector3 dir, float time, bool destory = false)
	{
		this.m_Tween = true;
		Vector3 localPosition = base.transform.localPosition;
		base.transform.DOLocalMove(localPosition + dir * 118f, time, false).OnComplete(delegate
		{
			this.m_Tween = false;
			this.m_Destory = destory;
		});
	}

	// 블럭 발사
	public void MoveTarget()
	{
		this.m_Tween = true;
		Vector3 endValue = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)this.m_Array + Vector3.down * (float)this.m_Index * 118f;
		base.transform.DOLocalMove(endValue, this.m_Time, false).OnComplete(delegate
		{
			this.m_Tween = false;
		});
	}

	[ContextMenu("Shake")]
	public void ShowCombine()
	{
		this.m_Tween = true;
		base.transform.DOScale(Vector3.one * 1.5f, this.m_Time).OnComplete(delegate
		{
			base.transform.localScale = Vector3.one;
			this.m_Tween = false;

			int combineNumber = this.m_Number * (int)Mathf.Pow(2f, (float)this.m_Count);
			Debug.Log("MoveDir ::::: " + combineNumber);
			
			GamePlayManager.Instance.BrickCombineCount[combineNumber]++;
		});
	}

	public void MoveToCombine(int dir)
	{
		if (dir <= 0 || dir > 4)
		{
			SingleInstance<DebugManager>.Instance.LogError("Bricks.MoveToCombine dir error,value =" + dir);
			return;
		}
		switch (dir)
		{
		case 1:
			this.MoveDir(Vector3.left, this.m_Time, true);
			break;
		case 2:
			this.MoveDir(Vector3.right, this.m_Time, true);
			break;
		case 3:
			this.MoveDir(Vector3.up, this.m_Time, true);
			break;
		case 4:
			this.MoveDir(Vector3.down, this.m_Time, true);
			break;
		}
	}

	public void ButtonState(bool state = true)
	{
		this.m_Image.raycastTarget = true;
	}

	// ANCHOR 2 ^ count
	public void SetCount(int count)
	{
		this.m_Count = count;
	}

	public void MoveToDeep(float delay = 0f)
	{
		this.m_Tween = true;
		this.m_Index += 7;
		Vector3 endValue = new Vector3(-237f, -57f, 0f) + Vector3.right * 118f * (float)this.m_Array + Vector3.down * (float)this.m_Index * 118f;
		base.transform.DOLocalMove(endValue, 0.5f, false).OnComplete(delegate
		{
			this.m_Tween = false;
			UnityEngine.Object.Destroy(base.gameObject);
		}).SetDelay(delay);
	}

	// ANCHOR Brick Touch 블럭 제거 아이템용 
	public void MoveTouch()
	{
		Debug.Log("_____ MoveTouch _____ " + this.m_Number);

		if(ItemManager.Instance.type == ItemType.selectDeleteBrick)
			GamePlayManager.Instance.deleteOneBrick(this);
		else if(ItemManager.Instance.type == ItemType.underNumberDeleteBrick)
			GamePlayManager.Instance.deleteBricksUnderNumber(this.m_Number);
		else if(ItemManager.Instance.type == ItemType.selectUpgradeBrick)
		{
			if(this.m_Number != GamePlayManager.Instance.m_MaxValue)
				GamePlayManager.Instance.UpdateBrickNumber(this);
		}
		else if(ItemManager.Instance.type == ItemType.lineDelete)
			GamePlayManager.Instance.deleteBricksVerticalLine(this.m_Array);
		else if(ItemManager.Instance.type == ItemType.selectBrickChangeBoomb)
			GamePlayManager.Instance.ProcessBrickBoomb(this.m_Array, this.m_Index);
	}
}
