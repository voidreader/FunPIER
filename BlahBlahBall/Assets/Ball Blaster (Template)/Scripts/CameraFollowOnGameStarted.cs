using System.Collections;
using UnityEngine;

namespace BallBlaster
{

    public class CameraFollowOnGameStarted : MonoBehaviour
    {

        [SerializeField] private Vector3 _desiredPosition;
        [SerializeField] private float _followSpeed;

        private void Start()
        {
            GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>().GameStartedEvent += () => StartCoroutine(GoToPosition());
        }

        private IEnumerator GoToPosition()
        {
            while (Vector3.Distance(transform.position, _desiredPosition) >= 0.005f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _desiredPosition, _followSpeed * Time.deltaTime);

                yield return null;
            }
        }

    }

}