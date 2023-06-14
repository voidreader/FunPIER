using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class SpawnActor_Extend_BlackFrags : SpawnActor_Extend
{
	//---------------------------------------
	//[SerializeField]
	//private Collider _collider = null;

	//---------------------------------------
	private static List<SpawnActor_Extend_BlackFrags> _createdFragments = new List<SpawnActor_Extend_BlackFrags>();
	public static List<SpawnActor_Extend_BlackFrags> CreatedFragments { get { return _createdFragments; } }

	private static Vector3 _fragCongregatePos = Vector3.zero;
	public static Vector3 FragCongregatePos
	{
		get { return _fragCongregatePos; }
		set { _fragCongregatePos = value; }
	}

	//---------------------------------------
	private bool _moveCompleted = false;
	private SpawnActor _spawnActorBase = null;

	public bool MoveCompleted
	{
		get { return _moveCompleted; }
		set { _moveCompleted = value; }
	}


	//---------------------------------------
	private void Awake()
	{
		if (_spawnActorBase == null)
			_spawnActorBase = GetComponent<SpawnActor>();
	}

	//---------------------------------------
	public override void Init()
	{
		_createdFragments.Add(this);
		_moveCompleted = false;
		gameObject.transform.localScale = Vector3.one;
	}

	public override void Frame()
	{
		if (_moveCompleted == false && Vector3.Distance(gameObject.transform.position, _fragCongregatePos) > 1.75f)
		{
			Vector3 vMoveVec = (_fragCongregatePos - gameObject.transform.position).normalized;

			_spawnActorBase.m_vMoveVector = vMoveVec;
			_spawnActorBase.m_vViewVector = vMoveVec;
		}
		else
		{
            if (_moveCompleted != true)
            {
                _moveCompleted = true;

                _spawnActorBase.m_vMoveVector = Vector3.zero;
                _spawnActorBase.m_vViewVector = Vector3.zero;
                gameObject.transform.localScale = Vector3.zero;

                if (_spawnActorBase.m_pInst_ExplodeParticle != null)
                {
                    ParticleSystem pExpPart = HT.Utils.InstantiateFromPool(_spawnActorBase.m_pInst_ExplodeParticle);
                    pExpPart.transform.position = gameObject.transform.position;
                    //pExpPart.transform.rotation = gameObject.transform.rotation;
                    //pExpPart.transform.localScale = gameObject.transform.localScale;

                    HT.Utils.SafeDestroy(pExpPart.gameObject, pExpPart.TotalSimulationTime());
                }
            }
		}
	}

	public override void Release()
	{
		_moveCompleted = false;
		_createdFragments.Remove(this);
	}

	//---------------------------------------
	public override void OnEvent_Damage(int nDamage)
	{
		BattleFramework._Instance.m_pEnemyActor.OnDamaged(nDamage);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------