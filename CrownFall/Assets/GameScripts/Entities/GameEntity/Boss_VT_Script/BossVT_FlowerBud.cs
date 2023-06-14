using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossVT_FlowerBud : MonoBehaviour
{
	//---------------------------------------
	public enum eBudType
	{
		FlameWall,
		Frenzy,
		Thorns,
		Freezy,

		MAX,
	}

	//---------------------------------------
	[Header("BUD INFO")]
	[SerializeField]
	private Material _colorMateial = null;
	[SerializeField]
	private eBudType _budType = eBudType.MAX;
	[SerializeField]
	private Animation _budAnimator = null;
	[SerializeField]
	private string _budAnim_Remove = null;

	//---------------------------------------
	public Material ColorMateial { get { return _colorMateial; } }
	public eBudType BudType { get { return _budType; } }

	//---------------------------------------
	public void Remove()
	{
		_budAnimator.Play(_budAnim_Remove);
	}
}


/////////////////////////////////////////
//---------------------------------------