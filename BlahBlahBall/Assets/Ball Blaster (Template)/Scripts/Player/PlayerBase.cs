using UnityEngine;

namespace BallBlaster
{

    [RequireComponent(typeof(Animator))]
    public class PlayerBase : MonoBehaviour
    {
        [SerializeField] private string _isShootingParameterName = "IsShooting";

        private Animator _animator;

        private IPlayerInput _input;
        private GameManager _gameManager;

        #region MonoBehaviour

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _input = GameObject.FindWithTag(Tags.Input).GetComponent<IPlayerInput>();
            _gameManager = GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>();

            _gameManager.GameFinishedEvent += () => _animator.SetBool(_isShootingParameterName, false);
        }

        private void Update()
        {
            if (_gameManager.IsGameStarted && !_gameManager.IsGameFinished)
                _animator.SetBool(_isShootingParameterName, _input.IsShootPressed());
        }

        #endregion
    }

}