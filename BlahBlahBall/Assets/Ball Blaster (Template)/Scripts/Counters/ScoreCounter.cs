using UnityEngine;
using UnityEngine.UI;

namespace BallBlaster
{

    public class ScoreCounter : MonoBehaviour
    {

        [Header("- Best score -")]
        [SerializeField] private Text _bestScoreText;

        [Header("- Current score -")]
        [SerializeField] private Text _currentScoreText;
        [SerializeField] private Animator _currentScoreAnimator;
        [SerializeField] private string _hitTriggerParameter = "Hit";

        [Header("- Stone Generator -")]
        [SerializeField] private StoneGenerator _stoneGenerator;

        private float _currentScore;

        #region MonoBehaviour

        private void Start()
        {
            // Update score texts
            _bestScoreText.text = NumberFormatter.ToKMB(Data.GetBestScore());
            UpdateCurrentScoreText();

            // Subscribe to GameFinish event
            GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>().GameFinishedEvent += OnGameFinished;
            // Subscribe to Spawn Stone event
            _stoneGenerator.OnStoneSpawnedEvent += SubscribeStonesOnBulletHitEvent;
        }

        #endregion

        private void SubscribeStonesOnBulletHitEvent(Stone stone)
        {
            stone.OnBulletHitEvent += BulletHitStone;

            foreach (Stone s in stone.ChildStones)
                SubscribeStonesOnBulletHitEvent(s);
        }

        private void BulletHitStone(float damage)
        {
            // Update current score
            _currentScore += damage;
            UpdateCurrentScoreText();

            // Animate current score text
            if (_currentScoreAnimator)
                _currentScoreAnimator.SetTrigger(_hitTriggerParameter);
        }

        private void UpdateCurrentScoreText()
        {
            _currentScoreText.text = NumberFormatter.ToKMB(_currentScore);
        }

        // Update best score
        private void OnGameFinished()
        {
            if ((int)_currentScore > Data.GetBestScore())
                Data.SetBestScore((int)_currentScore);
        }

    }

}