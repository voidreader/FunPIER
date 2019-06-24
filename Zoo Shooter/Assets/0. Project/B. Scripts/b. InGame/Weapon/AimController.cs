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
     
            

        // AimImage.rectTransform.sizeDelta = new Vector2(AimRange * 100f, AimRange * 100f);
        // AimLine.rectTransform.sizeDelta = new Vector2(-WeaponManager.StartAimDistance, 2f);

        
    }

    void Update() {

        if (AimController.Wait)
            return;

        if (!Player.isMoving && GameManager.main.isPlaying && !GameManager.main.isWaiting) {


            /*
            if (!_flip) {
                RotateAimRight();
            }
            else {
                RotateAimLeft();
            }
            */


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
    }

    /// <summary>
    /// 오른쪽에서 조준..
    /// </summary>
    private void RotateAimRight() {
        if (transform.localRotation.z < -0.4f) {
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
        if (transform.localRotation.z > 1.6f) {
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
