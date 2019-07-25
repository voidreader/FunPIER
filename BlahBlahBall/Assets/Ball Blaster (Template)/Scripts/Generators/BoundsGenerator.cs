using UnityEngine;

namespace BallBlaster
{

    public class BoundsGenerator : MonoBehaviour
    {

        public GameObject _boundPrefab;

        #region MonoBehaviour

        private void OnValidate()
        {
            if (_boundPrefab != null && _boundPrefab.GetComponent<BoxCollider2D>() == null)
            {
                Debug.LogError("BoundsGenerator: Bound Prefab has to have BoxCollider2D component.");
                _boundPrefab = null;
            }
        }

        private void Start()
        {
            float cameraWorldSpaceWidth = Camera.main.orthographicSize * 2 / Screen.height * Screen.width;

            float deltaPositionX = cameraWorldSpaceWidth / 2 + _boundPrefab.GetComponent<BoxCollider2D>().size.x / 2;

            Vector2 colliderSize = new Vector2(_boundPrefab.GetComponent<BoxCollider2D>().size.x, Camera.main.orthographicSize * 2);

            GameObject leftBound = Instantiate(_boundPrefab);
            leftBound.transform.position = new Vector3(Camera.main.transform.position.x - deltaPositionX,
                                                       Camera.main.transform.position.y,
                                                       0);
            leftBound.GetComponent<BoxCollider2D>().size = colliderSize;

            GameObject rightBound = Instantiate(_boundPrefab);
            rightBound.transform.position = new Vector3(Camera.main.transform.position.x + deltaPositionX,
                                                        Camera.main.transform.position.y,
                                                        0);
            rightBound.GetComponent<BoxCollider2D>().size = colliderSize;
        }

        #endregion

    }

}