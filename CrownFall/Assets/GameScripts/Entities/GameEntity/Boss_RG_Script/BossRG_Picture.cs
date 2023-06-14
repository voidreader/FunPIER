using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossRG_Picture : MonoBehaviour
{
	//---------------------------------------
	public static BossRG_Picture _bossPicture = null;

    public static void BossPictureDisable()
    {
        BossRG_Picture pPrevBossPicture = _bossPicture;
        _bossPicture = null;

        if (pPrevBossPicture != null)
            pPrevBossPicture.UpdatePicture();
    }

    //---------------------------------------
    [Header("OBJECTS")]
    [SerializeField]
    private GameObject _headRoot = null;
    [SerializeField]
    private GameObject _attackDummy = null;
	[SerializeField]
	private GameObject _areaHeightDummy = null;
	[SerializeField]
    private GameObj_DamageReciver _damageReciver = null;

    [Header("HEADER")]
    [SerializeField]
    private MeshRenderer _pictureMesh = null;
    [SerializeField]
    private Animation _pictureAnim = null;
    [SerializeField]
    private string _pictureAnim_On = null;
    [SerializeField]
    private string _pictureAnim_Off = null;
    [SerializeField]
    private Texture _pictureTex_Blank = null;
    [SerializeField]
    private Texture _pictureTex_Person = null;

	[Header("ECTOPLASM")]
	[SerializeField]
	private BoxCollider _ectoplasm_Collider = null;
	[SerializeField]
	private float[] _ectoplasm_Width = null;

	//---------------------------------------
	private Material _pictureMaterial = null;
    private Animation _animation = null;

	//---------------------------------------
	private void Awake()
    {
        SetReciverActivator(false);

        _animation = GetComponent<Animation>();

        //-----
        _pictureMaterial = Instantiate(_pictureMesh.material);
        _pictureMaterial.mainTexture = _pictureTex_Blank;

        _pictureMesh.material = _pictureMaterial;

		//-----
		Vector3 vSize = _ectoplasm_Collider.size;
		vSize.z = _ectoplasm_Width[(int)GameFramework._Instance.m_pPlayerData.m_eDifficulty];
    }

	//---------------------------------------
	public void SetIsBossPicture()
	{
        _bossPicture = this;
    }

    public void SetReciverActivator(bool bEnable)
    {
        _damageReciver.gameObject.SetActive(bEnable);
    }

    //---------------------------------------
    public void UpdatePicture()
    {
        UpdateTextures();

        if (_bossPicture == this)
            SetReciverActivator(true);
        else
            SetReciverActivator(false);
    }

    private void UpdateTextures()
    {
        float fDelay = PictureOff();
        Invoke("PictureOn", fDelay);
    }

    private float PictureOff()
    {
        _pictureAnim.Play(_pictureAnim_Off);
        return _pictureAnim.GetClip(_pictureAnim_Off).length;
    }

    private float PictureOn()
    {
        PictureTextureUpdate();

        _pictureAnim.Play(_pictureAnim_On);
        return _pictureAnim.GetClip(_pictureAnim_On).length;
    }

    private void PictureTextureUpdate()
    {
        if (_bossPicture == this)
            _pictureMaterial.mainTexture = _pictureTex_Person;
        else
            _pictureMaterial.mainTexture = _pictureTex_Blank;
    }

    //---------------------------------------
    public void PictureAttack()
	{
		if (_bossPicture == this)
			return;

        IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

		//-----
		Vector3 vView = pPlayer.transform.position - _attackDummy.transform.position;
        vView.y = 0.0f;
        vView.Normalize();

        _headRoot.transform.forward = vView;
        Vector3 vCurEuler = _headRoot.transform.rotation.eulerAngles;
        _headRoot.transform.rotation = Quaternion.Euler(vCurEuler + new Vector3(0.0f, 0.0f, 90.0f));

		//-----
		Vector3 vAreaPos = _headRoot.transform.position;
		vAreaPos.y = _areaHeightDummy.transform.position.y;

		float fWidth = _ectoplasm_Width [(int)GameFramework._Instance.m_pPlayerData.m_eDifficulty];
		Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert_Simple(vAreaPos, fWidth, 15.0f, 0.5f);
		pAlert.transform.forward = _headRoot.transform.forward;

		//-----
		_animation.Play("PICUTRE_ATTACK_LASER");
	}
}


/////////////////////////////////////////
//---------------------------------------