using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGL_PartsDisabler : MonoBehaviour
{
	private void Awake()
	{
		if (Debug.isDebugBuild == false)
			gameObject.SetActive(false);
	}

	private void OnCollisionEnter(Collision collision)
	{
		bool bPartsDisable = false;
		if (collision.gameObject.GetComponent<ControllableActor>() != null)
			bPartsDisable = true;

		if (collision.gameObject.GetComponent<Projectile>() != null)
			bPartsDisable = true;

		if (bPartsDisable)
		{
			AIActor_Extend_GL pExtend = BattleFramework._Instance.m_pEnemyActor.GetComponent<AIActor_Extend_GL>();

			for (AIActor_Extend_GL.eParts eParts = AIActor_Extend_GL.eParts.ArmR; eParts < AIActor_Extend_GL.eParts.Max; ++eParts)
			{
				pExtend.OnPartsActivate(eParts);
				pExtend.OnPartsDamaged(pExtend.FindParts(eParts)._parts, int.MaxValue);
			}
		}
	}
}
