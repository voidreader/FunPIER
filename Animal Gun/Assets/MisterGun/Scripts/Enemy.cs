using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Transform StartLineTransform;
    public Transform EndLineTransform;
    public Transform StartAimLineTransform;
    public Transform EndAimLineTransform;

    public Rigidbody2D EnemyRB;
    public Bullet BulletPrefab;
    public Transform GunpointTransform;
    public Transform WeaponTransform;
    public AudioClip ShootSound;

    public SpriteRenderer HeadRenderer;
    public SpriteRenderer BodyRenderer;
    public Sprite[] HeadSpriteList;
    public Sprite[] BodySpriteList;

    private float _moveSpeed;

    private int _direction;

    [HideInInspector]
    public bool _canShoot;

    private bool _canMove;
    private bool _stop;
    private bool _aiming;
    private bool _isGrounded;

    private void OnEnable () {
        _stop = false;
        _canShoot = false;
        _aiming = false;
        _isGrounded = false;

        if (EnemyRB.transform.position.x > 0)
        {
            _direction = -1;
        }
        else
        {
            _direction = 1;
        }
    }

    private void Start()
    {
        Init();
        _moveSpeed = Random.Range(2f, 4f);
    }

    private void FixedUpdate () {
        if (GameManager.IsStartGame && !GameManager.IsGameOver)
        {
            if (!_stop)
            {
                _canMove = Physics2D.Linecast(StartLineTransform.position, EndLineTransform.position);
                if (_canMove)
                {
                    Move();
                }
                else
                {
                    EnemyRB.velocity = Vector2.zero;
                    _stop = true;
                }
            }
            if (_stop && _canShoot && !_aiming)
            {
                if (!Physics2D.Linecast(StartAimLineTransform.position, EndAimLineTransform.position))
                {
                    WeaponTransform.Rotate(Vector3.back * Time.deltaTime * 40f);
                }
                else
                {
                    _aiming = true;
                    Invoke("Shoot", 0.5f);
                }
            }
        }
    }

    private void Init()
    {
        int randID = Random.Range(0, HeadSpriteList.Length);
        HeadRenderer.sprite = HeadSpriteList[randID];
        BodyRenderer.sprite = BodySpriteList[randID];
        HeadRenderer.transform.localPosition = new Vector2(HeadRenderer.transform.localPosition.x, BodyRenderer.sprite.bounds.size.y);
        WeaponTransform.transform.position = new Vector2(WeaponTransform.transform.position.x, BodyRenderer.transform.position.y);
        gameObject.AddComponent<BoxCollider2D>();
        HeadRenderer.gameObject.AddComponent<BoxCollider2D>();
    }

    private void Move()
    {
        if (_isGrounded)
        {
            if (EnemyRB.transform.position.x > 0)
            {
                EnemyRB.velocity = -transform.right * _moveSpeed;
            }
            else
            {
                EnemyRB.velocity = transform.right * _moveSpeed;
            }
        }
    }

    private void Shoot()
    {
        Bullet newBullet = Instantiate(BulletPrefab, null, false);
        newBullet.transform.position = GunpointTransform.position;
        newBullet.AddBulletForce(WeaponTransform, _direction);
        AudioManager.Instance.PlayAudio(ShootSound);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Stair")
        {
            _isGrounded = true;
        }
    }
}
