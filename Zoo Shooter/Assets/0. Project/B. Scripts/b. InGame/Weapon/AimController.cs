using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
    public static Action ResetAimAction;

    public Weapon EquipWeapon;
    public GameObject AimLine;
    public Transform AimTrail;

    public float AimSpeed;
    public float AimRange;

    [SerializeField] private bool _UpAiming;
    [SerializeField] private bool _DownAiming;

    public static bool Wait = true;

    private Quaternion _startAimRotation;

    
    [SerializeField] Quaternion currentRotation;
    [SerializeField] Vector3 currentAuler;


    private void Start() {
        _startAimRotation = transform.rotation;
        
        // Wait = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="w"></param>
    public void Init(Weapon w) {
        EquipWeapon = w;

        _UpAiming = true;
        _DownAiming = false;

        AimSpeed = EquipWeapon.AimSpeed;
        AimRange = EquipWeapon.AimRange;


        AimLine.transform.localScale = new Vector3(AimRange, 1.5f, 1); // Line 길이 설정 
        AimLine.transform.localPosition = EquipWeapon.posGunPoint; // Line 위치 설정
        // AimTrail.transform.localScale = new Vector3(1.2f, GetTrailHeight(AimRange), 1);
        AimTrail.transform.localScale = EquipWeapon.AimTrailScale;
            


        
    }

    #region Trail Y 구하기

    float GetTrailHeight(float width) {
        float step = (width - 0.1f) / 0.1f;
        int div = (int)step;
        Debug.Log("GetTrailHeight step :: " + step);
        Debug.Log("GetTrailHeight  width / div :: " + width.ToString() + "/" + div);
        Debug.Log("GetTrailHeight  :: " + 0.1f + 0.05f * div);
            

        return 0.1f + 0.05f * div;

    }

    #endregion


    void Update() {

        AimLine.gameObject.SetActive(false);
        AimTrail.gameObject.SetActive(false);

        if (AimController.Wait)
            return;

        if (Player.isMoving)
            return;

        if (!GameManager.isPlaying)
            return;

        ////// 체크 끝 ////

        AimLine.gameObject.SetActive(true);
        AimTrail.gameObject.SetActive(true);
        RotateAimRight();


        if (_UpAiming) {
            // transform.Rotate(Vector3.back * Time.deltaTime * AimSpeed);
            transform.Rotate(0, 0, -1 * Time.deltaTime * AimSpeed);
        }
        if (_DownAiming) {
            transform.Rotate(0, 0, 1 * Time.deltaTime * AimSpeed);
            //transform.Rotate(-Vector3.back * Time.deltaTime * AimSpeed);
        }

        currentRotation = transform.localRotation;
        currentAuler = transform.localRotation.eulerAngles;
    }

    /// <summary>
    /// 오른쪽에서 조준..
    /// </summary>
    private void RotateAimRight() {
        if (transform.localRotation.z < -0.45f) {
            _UpAiming = false;
            _DownAiming = true;
        }
        if (transform.localRotation.z > 0) {
            _UpAiming = true;
            _DownAiming = false;
        }
    }

    /// <summary>
    /// 왼쪽에서 조준
    /// </summary>
    private void RotateAimLeft() {
        if (transform.localRotation.z > 1.65f) {
            _UpAiming = false;
            _DownAiming = true;
        }
        if (transform.localRotation.z < 0f) {
            _UpAiming = true;
            _DownAiming = false;
        }
    }

    public void ResetAim() {
        this.transform.localEulerAngles = Vector3.zero;
    }
}
