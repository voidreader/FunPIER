using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class Field_Garden : Field
{
	//---------------------------------------
	public enum eGardenBud
	{
		E,
		W,
		N,
		S,
	}

	//---------------------------------------
	[Header("GARDEN INFO")]
	[SerializeField]
	private BossVT_FlowerBud _bud_E = null;
	[SerializeField]
	private BossVT_FlowerBud _bud_W = null;
	[SerializeField]
	private BossVT_FlowerBud _bud_N = null;
	[SerializeField]
	private BossVT_FlowerBud _bud_S = null;

	//---------------------------------------
	public BossVT_FlowerBud GetBud(eGardenBud eBud)
	{
		switch(eBud)
		{
			case eGardenBud.E:
				return _bud_E;

			case eGardenBud.W:
				return _bud_W;

			case eGardenBud.N:
				return _bud_N;

			case eGardenBud.S:
				return _bud_S;
		}

		return null;
	}
}


/////////////////////////////////////////
//---------------------------------------