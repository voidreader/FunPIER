using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeaponManager : MonoBehaviour
{
    public bool _isAimPossible = false;
    private Quaternion _startAimRotation;

    public bool _UpAiming = true;
    public bool _isLeft = false;
    public float _aimSpeed;

    public Transform GunPointTransform;
    public GameObject BulletPrefab;
    int _direction = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartAim(bool left) {
        _startAimRotation = this.transform.rotation;
        _isAimPossible = true;
        _UpAiming = true;
        _isLeft = left;
    }

    // Update is called once per frame
    void Update() {
        if (!_isAimPossible)
            return;

        if(_UpAiming) {
            transform.Rotate(Vector3.back * Time.deltaTime * _aimSpeed); // 위로 들기 
        }
        else {
            transform.Rotate(-Vector3.back * Time.deltaTime * _aimSpeed); // 아래로 내리기 
        }

        CheckRotateRange();


    }

    public void Shoot() {

        /*
        _direction *= -1;
        PlayerBullet newBullet = GameObject.Instantiate(BulletPrefab, null, false).GetComponent<PlayerBullet>();
        newBullet.transform.position = GunPointTransform.position;
        */

        PlayerBullet b = GameObject.Instantiate(BulletPrefab, null, false).GetComponent<PlayerBullet>();
        b.transform.position = GunPointTransform.position;
        b.transform.rotation = Quaternion.identity;
        // b.transform.rotation = this.transform.rotation;

        b.AddBulletForce(transform, _direction);
        // BulletPrefab.transform.rotation = this.transform.rotation;
        // newBullet.AddBulletForce(transform, _direction);
        // Bullet b = Instantiate()
    }

    void CheckRotateRange() {
        if(_UpAiming) {

            // Debug.Log("eulerAngles z :: " + transform.rotation.z);
            if (transform.rotation.z < -0.4f) {
                // Debug.Log("Change Aim Direction");
                _UpAiming = false;
                transform.eulerAngles = new Vector3(0, 0, -40);
            }

        }
        else {
            if (transform.rotation.z > 0) {
                _UpAiming = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

        }
    }
}
