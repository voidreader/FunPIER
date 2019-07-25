using UnityEngine;
using UnityEngine.UI;

namespace BallBlaster
{

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Stone : MonoBehaviour
    {

        [Header("- Animator parameters -")]
        [SerializeField]
        private string _hitTriggerParameter = "Hit";

        [Header("- Settings -")]
        [SerializeField]
        private Vector2 _startFlyingVelocity;

        [Header("- HP -")]
        [SerializeField]
        private int _hp = 20;
        [SerializeField] private Text _hpText;

        [Header("- Child stones -")]
        [SerializeField]
        private Stone[] _childStones;

        [Header("- Coins -")]
        [SerializeField]
        private Coin[] _coins;

        [Header("- Sound -")]
        [SerializeField]
        private AudioSource _crashAudioSource;

        private Rigidbody2D _rb2D;
        private Collider2D _collider2D;
        private Animator _animator;

        private GameManager _gameManager;

        public Stone[] ChildStones { get { return _childStones; } }

        public delegate void OnBroken();
        public event OnBroken OnBrokenEvent;

        public delegate void OnBulletHit(float damage);
        public event OnBulletHit OnBulletHitEvent;

        #region 

        private void Awake()
        {
            if (!gameObject.CompareTag(Tags.Stone))
                Debug.LogError("Stone: has to be tagged as Stone.");

            _rb2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();

            _gameManager = GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>();
        }

        private void Start()
        {
            foreach (Stone childStone in _childStones)
            {
                childStone.gameObject.SetActive(false);
                childStone.DisableMovement();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.Bullet) && !_gameManager.IsGameFinished)
            {
                int newHp = _hp - collision.GetComponent<Bullet>().ShootPower;
                if (OnBulletHitEvent != null)
                {
                    if (newHp < 0)
                        OnBulletHitEvent(_hp);
                    else
                        OnBulletHitEvent(collision.GetComponent<Bullet>().ShootPower);
                }

                UpdateHP(newHp);

                _animator.SetTrigger(_hitTriggerParameter);

                if (_hp <= 0)
                    StoneBroken();
            }
        }

        #endregion

        public void EnableMovement()
        {
            _collider2D.isTrigger = false;
            _rb2D.isKinematic = false;

            _rb2D.velocity = _startFlyingVelocity;
        }

        public void DisableMovement()
        {
            _collider2D.isTrigger = true;
            _rb2D.isKinematic = true;

            _rb2D.velocity = Vector2.zero;
            _rb2D.angularVelocity = 0;
        }

        private void StoneBroken()
        {
            DropChildStones();
            DropCoins();

            if (_crashAudioSource)
                _crashAudioSource.Play();

            // Vibration
            if (_childStones.Length == 0)
                LightVibration.VibrationManager.VibrateMedium();
            else
                LightVibration.VibrationManager.VibrateLight();

            if (OnBrokenEvent != null)
                OnBrokenEvent();

            Destroy(gameObject);
        }

        private void DropChildStones()
        {
            foreach (Stone childStone in _childStones)
            {
                childStone.gameObject.SetActive(true);
                childStone.transform.parent = null;

                childStone.EnableMovement();
            }
        }

        private void DropCoins()
        {
            foreach (Coin coin in _coins)
            {
                coin.ThrowThisCoin();
            }
        }

        public void UpdateHP(int newHP)
        {
            _hp = newHP;
            _hpText.text = NumberFormatter.ToKMB(_hp);
        }

    }

}