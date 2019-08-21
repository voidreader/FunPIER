using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;

public enum WeaponDropType {
    NoDrop,
    Normal,
    BigHit,
    HighJump
}

public class EnemyWeapon : MonoBehaviour
{
    public string _spriteName = string.Empty;
    public SpriteRenderer _sprite;
    public Weapon _weaponData;
    public Transform _gunPoint;
    public Transform _aimPoint;

    public bool _canShoot = false; // 쏠 수 있음 
    public bool _aiming = false; // 조준 중 

    
    
    
    RaycastHit2D ray;

    public void InitRotation(bool isLeft) {

        ResetStatus();
        this.transform.localEulerAngles = new Vector3(0, this.transform.localEulerAngles.y, 0);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gun"></param>
    public void SetEnemyWeapon(string gun) {

        this.gameObject.SetActive(true);

        // 스프라이트와 웨폰 데이터 처리
        for(int i =0; i<Stocks.main.ListWeaponSprites.Count;i++) {

            if (Stocks.main.ListWeaponSprites[i].name == gun) {
                _sprite.sprite = Stocks.main.ListWeaponSprites[i];
                break;
            }
          
        }

        for (int i = 0; i < Stocks.main.ListWeapons.Count; i++) {

            if (Stocks.main.ListWeapons[i].name == gun) {
                _weaponData = Stocks.main.ListWeapons[i];
                break;
            }
        }

        _gunPoint.transform.localPosition = _weaponData.posGunPoint; // 건포인트
        this.transform.localScale = _weaponData.posScale; // 크기 

        
            
    } 


    public void ResetStatus() {
        _canShoot = false; 
        _aiming = false; 
    }

    public void Shoot() {

        Debug.Log("Enemy Shoot Called");


        _canShoot = true;
    }


    private void Update() {


        if (!_canShoot)
            return;



        if (!_aiming) { // 조준 처리

            ray = Physics2D.Linecast(_gunPoint.position, _aimPoint.position);
            if(!ray || ray.collider.tag != "Player")
                this.transform.Rotate(-Vector3.back * Time.deltaTime * 60);
            else {

                this.transform.Rotate(-Vector3.back * Time.deltaTime * 60); // 몸통쪽을 조준하게끔 조금더 회전해준다. 
                _aiming = true;
                Debug.Log("Enemy Aim Completed ");
                _canShoot = false; // 발사 처리 

                GameManager.main.player.SetLargeCollider();
                Invoke("ShotBullet", 0.25f);

            }
        }
    }


    void ShotBullet() {


        // Bullet b = GameObject.Instantiate(Stocks.main.prefabEnemyBullet, null, false).GetComponent<Bullet>();
        //b.transform.position = _gunPoint.position;
        Bullet b = PoolManager.Pools[ConstBox.poolGame].Spawn(Stocks.main.prefabEnemyBullet, _gunPoint.position, Quaternion.identity).GetComponent<Bullet>();
        b.isEnemy = true;

        if (GameManager.main.enemy.isLeft)
            b.transform.eulerAngles = new Vector3(this.transform.eulerAngles.z, 90, 0);
        else
            b.transform.eulerAngles = new Vector3(this.transform.eulerAngles.z, -90, 0);

        b.SetBulletOn(GameManager.main.enemy.isLeft);

        AudioAssistant.Shot(Stocks.main.clipEnemyShotSound);
    }

    public void SetDrop(bool isLeft, WeaponDropType drop) {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Rigidbody2D>().isKinematic = false;

        float rotateAngle = 360;
        if(isLeft)
            rotateAngle = 360;
        else
            rotateAngle = -360;

        switch (drop) {
            case WeaponDropType.NoDrop: // 아무런 힘을 가하지 않는다. 
                this.transform.DORotate(new Vector3(0, 0, rotateAngle), Random.Range(1.2f, 2.4f), RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                break;

            case WeaponDropType.Normal:
                if (isLeft)
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-150f, -20f), Random.Range(100f, 250f)));
                else
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(20f, 150f), Random.Range(100f, 250f)));

                this.transform.DORotate(new Vector3(0, 0, rotateAngle), Random.Range(0.8f, 2), RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

                break;

            case WeaponDropType.BigHit:
                if (isLeft) 
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(-250, 800));
                else 
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 800));
                
                this.transform.DORotate(new Vector3(0, 0, rotateAngle), 0.4f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

                break;

            case WeaponDropType.HighJump:
                this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 600));
                this.transform.DORotate(new Vector3(0, 0, rotateAngle), 0.4f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                break;
        }

        this.gameObject.layer = 15;
        Destroy(this.gameObject, 4);
    }
}
