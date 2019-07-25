using UnityEngine;

namespace BallBlaster
{

    [RequireComponent(typeof(Animator))]
    public class GameStartedDisappearingUIObject : MonoBehaviour
    {

        [SerializeField] private string _hideTriggerParameter = "Hide";
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();

            GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>().GameStartedEvent += OnGameStarted;
        }

        private void OnGameStarted()
        {
            _animator.SetTrigger(_hideTriggerParameter);
            Invoke("Disable", 1f);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }

    }

}