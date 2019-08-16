using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;

public class WeaponManager : MonoBehaviour {

    public static bool isShooting = false;
    public static bool isHit = false; // 명중 여부 체크 
    public static List<Bullet> ListShootingBullets = new List<Bullet>(); // 발사된 총알 리스트(비활성화시 제거)

    public bool isInit = false;
    public Weapon EquipWeapon; // 장착한 무기 
    public AimController CurrentAim;  // Aim Controller 
    public SpriteRenderer CurentWeaponRenderer;


    public Transform GunpointTransform;
    
    public Transform AimMask;

    RaycastHit2D hit;
    [SerializeField] Transform hitObject = null;


    Transform spawnedBullet;
    private int _direction;
    public static float StartAimDistance;

    Quaternion weaponFirstRotation;



    /// <summary>
    /// 무기 초기화 
    /// </summary>
    public void Init(bool force = false) {

        if (force)
            isInit = false;

        if (isInit)
            return;

        isInit = true;

        // EquipWeapon = Stocks.main.ListWeapons[0]; // 임시 
        EquipWeapon = PIER.main.CurrentWeapon;
        InitWeaponSystem();
    }


    public void SetWeaponByEquipManager(Weapon w) {
        CurentWeaponRenderer.sprite = w.WeaponSprite;
        this.transform.localPosition = w.posEquip; // 위치 설정 
        this.transform.localScale = w.posScale;
        GunpointTransform.transform.localPosition = w.posGunPoint; // 건포인트 설정

        AimMask.localPosition = new Vector3(-1, this.transform.localPosition.y, 0);

        // Aim 설정
        CurrentAim.Init(w);

    }

    void InitWeaponSystem() {
        GameManager.main.currentWeapon = EquipWeapon;
        CurentWeaponRenderer.sprite = EquipWeapon.WeaponSprite;

        this.transform.localPosition = EquipWeapon.posEquip; // 위치 설정 
        this.transform.localScale = EquipWeapon.posScale;
        GunpointTransform.transform.localPosition = EquipWeapon.posGunPoint; // 건포인트 설정

        AimMask.localPosition = new Vector3(-1, this.transform.localPosition.y, 0);

        // Aim 설정
        CurrentAim.Init(EquipWeapon);

        _direction = -1; // 방향은 -1로 고정이다. 
    }



    void Update() {
        // RayGunPoint();    
    }


    public void SetDrop(bool isLeft) {
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        this.GetComponent<Rigidbody2D>().isKinematic = false;

        if (isLeft)
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-150f, -20f), UnityEngine.Random.Range(100f, 250f)));
        else
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(20f, 150f), UnityEngine.Random.Range(100f, 250f)));

        this.GetComponent<Rigidbody2D>().AddTorque(360, ForceMode2D.Impulse);
        this.gameObject.layer = 15;


        Destroy(this.gameObject, 4);
    }




    public void AfterShoot() {
        CurrentAim.ResetAim();
    }


    /// <summary>
    /// 발사 시작!
    /// </summary>
    public void Shoot() {

        isHit = false; // 초기화 
        isShooting = true; // 슈팅 시작 
        weaponFirstRotation = this.transform.rotation; // 첫 슛에서 회전값 저장

        ListShootingBullets.Clear(); // 발사 총알 리스트 초기화

        switch (EquipWeapon.CurrentType) {
            case WeaponType.Gun:
                ShootWithGun();
                Invoke("Reload", 1f);
                break;
            case WeaponType.Shotgun:
                ShootWithShotgun();
                Invoke("Reload", 1f);
                break;
            case WeaponType.MachineGun:
                StartCoroutine(ShootWithMachineGun());
                break;
        }

        

    }

    /// <summary>
    /// 명중여부 체크 
    /// </summary>
    public bool RayGunPoint() {

        // 사용 이유. 
        // 쏘기전에 미리 명중 여부를 판단하기 위함. 
        // 총알마다 모두 속도가 다르기 때문에 
        // 빗나감 여부를 판단하기가 어렵다. 

        // → 총알자체에서 Ray를 해야할것같다. 여기서 하면 총알은 맞아도 빗나가는 경우가 발생. 

        hit = Physics2D.Raycast(GunpointTransform.position, GunpointTransform.right * -1);

        

        if (hit.collider != null && (hit.collider.tag == "Body" || hit.collider.tag == "Head")) {
            // hitObject = hit.collider.transform;
            // Debug.DrawLine(GunpointTransform.position, hit.collider.transform.position, Color.red);
            return true;
        }
        else {
            return false;
        }
    }



    /// <summary>
    /// 실제 총알 발사!
    /// </summary>
    void ShotBulletNoForce(bool isMute = false) {
        // Bullet b = GameObject.Instantiate(EquipWeapon.bulletPrefab, null, false).GetComponent<Bullet>();
        // b.transform.position = GunpointTransform.position;
        spawnedBullet = PoolManager.Pools[ConstBox.poolGame].Spawn(EquipWeapon.bulletPrefab, GunpointTransform.position, Quaternion.identity);
        // spawnedBullet.position = GunpointTransform.position;


        if (GameManager.main.player.isLeft)
            spawnedBullet.eulerAngles = new Vector3(this.transform.eulerAngles.z, 90, 0);
        else
            spawnedBullet.eulerAngles = new Vector3(this.transform.eulerAngles.z, -90, 0);

        spawnedBullet.GetComponent<Bullet>().isEnemy = false;
        spawnedBullet.GetComponent<Bullet>().SetBulletOn(GameManager.main.player.isLeft);

        // Shoot 재생 
        if (isMute)
            return;

        PlayShootSound();

    }


    void PlayShootSound() {
        if (EquipWeapon.ShootSound) {
            AudioAssistant.Shot(EquipWeapon.ShootSound);
        }
    }

    /// <summary>
    /// 총기 흔들림효과 
    /// </summary>
    void ShakeAimRotation() {
        float acc = 1 - (EquipWeapon.Accuracy / 100);
        // Debug.Log("ShakeAimRotate :: " + acc);
        transform.rotation = weaponFirstRotation; // 기본값으로 돌리고 시작한다.
        transform.Rotate(0, 0, 1 * UnityEngine.Random.Range(-acc, acc) * 20);

    }


    /// <summary>
    /// 단발형 총 발사 
    /// </summary>
    private void ShootWithGun() {

        Debug.Log("Shoot With Gun");
        ShotBulletNoForce();
        this.transform.DOLocalMove(Vector3.forward * 8, 0.15f).SetLoops(2, LoopType.Yoyo);

        // wait
        AimController.Wait = true; // 한방 쏘고 더이상 조준하지 않음.. 
        isShooting = false;
    }

    /// <summary>
    /// 샷건 발사 
    /// </summary>
    /// <returns></returns>
    void ShootWithShotgun() {

        Quaternion originalRot = this.transform.rotation; // 이전 총구 회전값을 가지고 있는다.
        AimController.Wait = true;

        // Audio
        for (int i =0; i< EquipWeapon.BulletsCount; i++) {
            // 가장 첫번째 총알은 조준한대로 발사된다. 
            if (i == 0) {
                // ShotBullet();
                ShotBulletNoForce();
            }
            else {
                this.transform.rotation = originalRot;
                ShakeAimRotation(); // 2번째 총알부터는 정확성에 따라 발사. 
                //ShotBullet(true);
                ShotBulletNoForce(true);
            }

            // yield return new WaitForSeconds(0.01f);
        }

        this.transform.DOLocalMove(Vector3.forward * 10, 0.15f).SetLoops(2, LoopType.Yoyo);
        isShooting = false;
    }



    /// <summary>
    /// 머신건 발사 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShootWithMachineGun() {

        Debug.Log("ShootWithMachineGun :: " + EquipWeapon.BulletsCount);

        AimController.Wait = true; // 더이상 조준하지 않음. 
        


        for (int i = 0; i < EquipWeapon.BulletsCount; i++) {

            //ShotBullet();
            ShotBulletNoForce();


            // 쏘고 나서 정확도 만큼 틀어지게 만든다. 
            ShakeAimRotation();

            yield return new WaitForSeconds(EquipWeapon.FireRate);
        }

        // yield return new WaitForSeconds(0.1f);

        // 총알 처리 될때까지 대기
        while (ListShootingBullets.Count > 0)
            yield return null;


        isShooting = false;
        Reload();
    }

    /// <summary>
    /// 재장전(소리)
    /// </summary>
    private void Reload() {

        Debug.Log("Reload Check! : " + isHit);

        // 게임 매니저에게 빗나갔음을 알린다. 
        if (!isHit) {
            GameManager.isMissed = true;
        }
        else { // 맞췄을때만 리로드 사운드 
            if (EquipWeapon.ReloadSound) {
                AudioAssistant.Shot(EquipWeapon.ReloadSound);
            }
        }

    }
}
