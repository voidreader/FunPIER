using System.Collections;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Action<int> ChangeSpriteAction;

    public AudioClip FootStep;
    public Transform StartLinePos;
    public Transform EndLinePos;
    public Transform StartMoveLinePos;
    public Transform EndMoveLinePos;
    public Rigidbody2D PlayerRB;
    public GameObject AimImage;
    public StairsManager Stairs;

    public SpriteRenderer PlayerRenderer;
    private Sprite[] Sprites;

    private float _moveSpeed;

    private bool _isStairs;
    private bool _canMove;
    private bool _isFlip;
    private bool _moveFlag;
    private bool _footStep;

    public static bool _isMoving;

    private Vector3 movemant;
    private int _currentStairs;

    private void OnEnable()
    {
        ChangeSpriteAction += ChangeSprite;
    }

    private void OnDisable()
    {
        ChangeSpriteAction -= ChangeSprite;
    }

    void Start()
    {
        _moveSpeed = 5f;
        _isMoving = false;
        _isFlip = false;
        _moveFlag = false;
        _currentStairs = 0;
        _footStep = false;
        Sprites = Resources.LoadAll<Sprite>("Characters/Characters");
        ChangeSprite(PlayerPrefs.GetInt("CurrenSkin"));
    }

    private void Update()
    {
        if (GameManager.IsStartGame && !GameManager.IsGameOver)
        {
            if (Input.GetMouseButtonDown(0) && !AimController.Wait)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    void FixedUpdate()
    {
        Raycasting();
        Behaviours();
    }

    private void ChangeSprite(int id)
    {
        PlayerRenderer.sprite = Sprites[id];
    }

    private void Raycasting()
    {
        _isStairs = Physics2D.Linecast(StartLinePos.position, EndLinePos.position);
        _canMove = Physics2D.Linecast(StartMoveLinePos.position, EndMoveLinePos.position);
    }

    private void Behaviours()
    {
        if (_isMoving && !GameManager.IsGameOver)
        {
            if (_canMove || _moveFlag)
            {
                if (!_isStairs)
                {
                    Move(transform.localScale.x, 0);
                }
                else
                {
                    Move(0, 1f);
                    if (!_footStep)
                    {
                        _footStep = true;
                        AudioManager.Instance.PlayAudio(FootStep);
                        Invoke("ResetStep",0.1f);
                    }
                }
            }
            if (!_canMove && !_isFlip)
            {
                _isFlip = true;
                StartCoroutine(Flip());
            }
        }
    }

    private void ResetStep()
    {
        _footStep = false;
    }

    public void Move(float x, float y)
    {
        movemant.Set(x, y, 0);
        movemant = movemant.normalized * _moveSpeed * Time.deltaTime;
        PlayerRB.MovePosition(transform.position + movemant);
    }

    private IEnumerator Shoot()
    {

        Debug.Log("Shoot!");

        WeaponManager.ShootAction();
        AimImage.SetActive(false);
        yield return new WaitForSeconds(1f);
        if (Stairs.StairsList[_currentStairs].EnemyPrefab == null)
        {
            Debug.Log("Enemy Killed");

            _isMoving = true;
            _moveFlag = true;
            AimController.ResetAimAction();
            _currentStairs++;
        }
        else
        {

            Debug.Log("Aim Missed!!!!");
            Stairs.StairsList[_currentStairs].EnemyPrefab.GetComponent<Enemy>()._canShoot = true;
        }
    }

    private IEnumerator Flip()
    {
        yield return new WaitForSeconds(0.15f);
        _moveFlag = false;
        transform.localScale = new Vector3(transform.localScale.x * -1f, 1f, 1f);
        _isFlip = false;
        _isMoving = false;
        AimImage.SetActive(true);
        StairsManager.EnableColliderAction();
        CameraFollow.CameraIsMove = true;
        AimController.Wait = false;
    }
}
