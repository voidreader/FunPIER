using UnityEngine;

namespace BallBlaster
{

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Coin : MonoBehaviour
    {

        [SerializeField] private int _value = 5;
        [SerializeField] private Vector2 _thrownVelocity;

        private Rigidbody2D _rb2D;
        private Collider2D _collider2D;

        public int Value { get { return _value; } }

        private void Awake()
        {
            if (!gameObject.CompareTag(Tags.Coin))
                Debug.LogError("Coin: has to be tagged as Coin.");

            _rb2D = GetComponent<Rigidbody2D>();
            _rb2D.isKinematic = true;

            _collider2D = GetComponent<Collider2D>();
            _collider2D.isTrigger = true;

            gameObject.SetActive(false);
        }

        public void ThrowThisCoin()
        {
            gameObject.transform.parent = null;
            gameObject.SetActive(true);

            _collider2D.isTrigger = false;
            _rb2D.isKinematic = false;

            _rb2D.velocity = _thrownVelocity;
        }

        public void OnPickedUp()
        {
            Destroy(gameObject);
        }
    }

}