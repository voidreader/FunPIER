using UnityEngine;

namespace BallBlaster
{

    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {

        [SerializeField] private Vector2 _bulletVelocity;

        private Rigidbody2D _rb2D;
        private Collider2D _collider2D;

        public delegate void BoundHit(Transform bulletTransform);
        public event BoundHit BoundHitEvent;

        public int ShootPower { get; set; }

        #region MonoBehaviour

        private void Awake()
        {
            if (!gameObject.CompareTag(Tags.Bullet))
                Debug.LogError("Bullet: has to be tagged as Bullet.");

            _rb2D = GetComponent<Rigidbody2D>();
            _rb2D.isKinematic = true;

            _collider2D = GetComponent<Collider2D>();
            _collider2D.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Tags.UpperBound))
                OnBoundHit();
        }

        #endregion

        public void OnBoundHit()
        {
            DisableMovement();

            if (BoundHitEvent != null)
                BoundHitEvent(transform);
        }

        public void EnableMovement()
        {
            gameObject.SetActive(true);

            _rb2D.velocity = _bulletVelocity;
        }

        public void DisableMovement()
        {
            _rb2D.velocity = Vector2.zero;

            gameObject.SetActive(false);
        }

    }

}