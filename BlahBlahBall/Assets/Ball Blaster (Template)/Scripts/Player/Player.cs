using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BallBlaster
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class Player : MonoBehaviour
    {

        [SerializeField] private float _movementSpeed = 8f;

        [Header("- Bounds -")]
        [SerializeField] private float _boundOffset = 0.6f;

        [Header("- Sounds -")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _pickedUpCoinAudioSource;
        [SerializeField] private AudioClip _shotClip;
        [SerializeField] private AudioClip _gameOverClip;

        [Header("- Shooting setting -")]
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _generatedBulletsCount = 500;
        [SerializeField] private Transform[] _bulletPositions;

        [Header("- Shooting Speed/Power -")]
        [SerializeField] private float _shootSpeed = 0.06f;
        [SerializeField] private int _shootPower = 1;
        [SerializeField] private bool _useInspectorValues;

        private Queue<Transform> _bullets = new Queue<Transform>();

        // used for not going out of camera bounds
        private Vector2 _cameraBounds;

        private IPlayerInput _input;
        private GameManager _gameManager;

        public float ShootSpeed { get { return _shootSpeed; } }
        public float ShootPower { get { return _shootPower; } }

        public delegate void OnCoinPickedUp(Coin coinValue);
        public event OnCoinPickedUp CointPickedUpEvent;

        #region MonoBehaviour

        private void OnValidate()
        {
            if (_bulletPrefab.GetComponent<Bullet>() == null)
            {
                Debug.LogError("Player: BulletPrefab has to have Bullet Component.");
                _bulletPrefab = null;
            }
        }

        private void Awake()
        {
            _input = GameObject.FindWithTag(Tags.Input).GetComponent<IPlayerInput>();
            _gameManager = GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>();

            GetComponent<Collider2D>().isTrigger = true;

            float cameraWorldSpaceWidth = Camera.main.orthographicSize * 2 / Screen.height * Screen.width / 2;
            _cameraBounds = new Vector2(Camera.main.transform.position.x - cameraWorldSpaceWidth + _boundOffset,
                                        Camera.main.transform.position.x + cameraWorldSpaceWidth - _boundOffset);

            for (int i = 0; i < _generatedBulletsCount; i++)
            {
                GameObject bullet = Instantiate(_bulletPrefab);
                bullet.GetComponent<Bullet>().BoundHitEvent += AddBulletToQueue;
                bullet.GetComponent<Bullet>().DisableMovement();

                _bullets.Enqueue(bullet.transform);
            }

            // Subscribe to event in Awake method in order to it was called first, befors OnGameStarted method in StoneGenerator
            _gameManager.GameStartedEvent += OnGameStarted;
        }

        private void Update()
        {
            if (_gameManager.IsGameStarted && !_gameManager.IsGameFinished)
            {
                transform.position = new Vector3(Mathf.Lerp(transform.position.x, _input.GetTouchPointX(), _movementSpeed * Time.deltaTime),
                                                 transform.position.y,
                                                 transform.position.z);
                CheckBounds();

                if (_input.IsShootPressed())
                    Shoot();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_gameManager.IsGameFinished)
            {
                if (collision.CompareTag(Tags.Stone))
                {
                    collision.GetComponent<Stone>().DisableMovement();
                    FinishGame();
                }
                else if (collision.CompareTag(Tags.Coin))
                {
                    if (_pickedUpCoinAudioSource)
                        _pickedUpCoinAudioSource.Play();

                    Coin coin = collision.GetComponent<Coin>();

                    if (CointPickedUpEvent != null)
                        CointPickedUpEvent(coin);

                    coin.OnPickedUp();
                }
            }
        }

        #endregion

        private void OnGameStarted()
        {
            if (!_useInspectorValues)
            {
                _shootPower = Data.GetShootPower();
                _shootSpeed = Data.GetShootSpeed();
            }

            if (GetComponent<Animator>())
                GetComponent<Animator>().enabled = false;

            foreach (Transform bullet in _bullets)
                bullet.GetComponent<Bullet>().ShootPower = _shootPower;
        }

        private void FinishGame()
        {
            PlaySound(_gameOverClip);

            _gameManager.FinishGame();
        }

        private void CheckBounds()
        {
            if (transform.position.x < _cameraBounds.x)
                transform.position = new Vector3(_cameraBounds.x, transform.position.y, transform.position.z);
            else if (transform.position.x > _cameraBounds.y)
                transform.position = new Vector3(_cameraBounds.y, transform.position.y, transform.position.z);
        }

        private bool _canShoot = true;

        private void Shoot()
        {
            if (_canShoot)
            {
                _canShoot = false;
                StartCoroutine(CanShoot(_shootSpeed));

                PlaySound(_shotClip);

                if (_bullets.Count >= _bulletPositions.Length)
                {
                    for (int i = 0; i < _bulletPositions.Length; i++)
                    {
                        Transform bullet = _bullets.Dequeue();
                        bullet.position = _bulletPositions[i].position;
                        bullet.GetComponent<Bullet>().EnableMovement();
                    }
                }
                else
                {
                    Debug.LogError("Player: GeneratedBulletsCount was to small.");
                }
            }
        }

        private IEnumerator CanShoot(float time)
        {
            while(time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            _canShoot = true;
        }

        private void AddBulletToQueue(Transform bullet)
        {
            _bullets.Enqueue(bullet);
        }

        private void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

    }

}