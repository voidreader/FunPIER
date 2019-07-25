using UnityEngine;

namespace BallBlaster
{

    public class GameFinishedAppearingObject : MonoBehaviour
    {

        private void Start()
        {
            GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>().GameFinishedEvent += () => gameObject.SetActive(true);

            gameObject.SetActive(false);
        }

    }

}