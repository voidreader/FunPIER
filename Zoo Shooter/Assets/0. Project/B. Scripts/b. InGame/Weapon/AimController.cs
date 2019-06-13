using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
    public static Action ResetAimAction;

    // public Image AimImage;
    // public Image AimLine;
    public SpriteRenderer AimLine;
    

    public float AimSpeed;
    public float AimRange;

    private bool _UpAiming;
    private bool _DownAiming;

    public static bool Wait;

    private Quaternion _startAimRotation;

    private bool _flip;

    private void OnEnable() {
        ResetAimAction += ResetAim;
    }

    private void OnDisable() {
        ResetAimAction -= ResetAim;
    }

    private void Start() {
        _startAimRotation = transform.rotation;
        _flip = false;
        Wait = false;
    }

    public void Init() {
        _UpAiming = true;
        _DownAiming = false;

        

        // AimImage.rectTransform.sizeDelta = new Vector2(AimRange * 100f, AimRange * 100f);
        // AimLine.rectTransform.sizeDelta = new Vector2(-WeaponManager.StartAimDistance, 2f);
    }

    void Update() {


        if (!_flip) {
            RotateAimRight();
        }
        else {
            RotateAimLeft();
        }

        if (_UpAiming) {
            transform.Rotate(-Vector3.back * Time.deltaTime * AimSpeed);
        }
        if (_DownAiming) {
            transform.Rotate(Vector3.back * Time.deltaTime * AimSpeed);
        }

        /*

        if (!Player._isMoving && !Wait && GameManager.IsStartGame && !GameManager.IsGameOver) {
            
        }
        */
    }

    private void RotateAimRight() {
        if (transform.rotation.z >= 0.4f) {
            _UpAiming = false;
            _DownAiming = true;
        }
        if (transform.rotation.z <= 0) {
            _UpAiming = true;
            _DownAiming = false;
        }
    }
    private void RotateAimLeft() {
        if (transform.rotation.z <= -0.4f) {
            _UpAiming = false;
            _DownAiming = true;
        }
        if (transform.rotation.z >= 0f) {
            _UpAiming = true;
            _DownAiming = false;
        }
    }

    private void ResetAim() {
        transform.rotation = _startAimRotation;
        _flip = !_flip;

    }
}
