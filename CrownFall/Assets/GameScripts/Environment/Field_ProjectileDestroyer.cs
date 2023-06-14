using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class Field_ProjectileDestroyer : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private string[] _nameConditions = null;

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		Projectile pProj = other.GetComponent<Projectile>();
		Projectile_Parabola pParabola = other.GetComponent<Projectile_Parabola>();
		Field_DamageObject pDamageObj = other.GetComponent<Field_DamageObject>();
		if (pProj == null && pParabola == null && pDamageObj == null)
			return;

		for (int nInd = 0; nInd < _nameConditions.Length; ++nInd)
		{
			if (other.name.IndexOf(_nameConditions[nInd]) >= 0)
			{
				if (pProj != null)
					HT.Utils.SafeDestroy(other.gameObject);
				
				if (pParabola != null)
					HT.Utils.SafeDestroy(other.gameObject);

				if (pDamageObj != null)
					pDamageObj.Release(true);
			}
		}
	}
}


/////////////////////////////////////////
//---------------------------------------