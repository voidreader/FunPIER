using System.Collections;
using UnityEngine;

namespace BallBlaster
{

    /// <summary>
    /// Big Stones will be spawned after 
    /// small stones (count = smallStoneCount) and medium stones (count = mediumStonecount) will be spawned.
    /// </summary>
    public class StoneGenerator : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [Header("- Stone Prefabs -")]
        [SerializeField]
        private GameObject[] _smallStonePrefab;
        // How much small stones will be spawned before mediumStones
        [SerializeField] private int _smallStoneCount;
        [SerializeField] private GameObject[] _mediumStonePrefab;
        // How much medium stones will be spawned before bigStones
        [SerializeField] private int _mediumStoneCount;
        [SerializeField] private GameObject[] _bigStonePrefab;

        [Header("- Spawn settings -")]
        [SerializeField]
        private Transform[] _startPoints;
        [SerializeField] private Transform[] _endPoints;
        [SerializeField] private float _startMovementLerpSpeed = 2f;
        [SerializeField] private int _startGeneratedStoneHP = 10;
        [SerializeField] private int _maxStoneCountInScene = 4;

        private int _smallStoneSpawnedCount;
        private int _mediumStoneSpawnedCount;

        private int _brokenStonesCount = 1;
        private int _stoneCountInScene;

        public delegate void OnStoneSpawned(Stone stone);
        public event OnStoneSpawned OnStoneSpawnedEvent;

        #region MonoBehaviour

        private void Start()
        {
            // Subscribe to event in Start method in order to it was called after Player's OnGameStarted method
            // In order to get valid Power and Speed values
            // Stone values will depend on player Speed and Power ability
            GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>().GameStartedEvent += OnGameStarted;
        }

        #endregion

        private void OnGameStarted()
        {
            _startGeneratedStoneHP += (int)(_player.ShootPower * _brokenStonesCount + (0.1f - _player.ShootSpeed) * _brokenStonesCount);

            SpawnStone(_smallStonePrefab[Random.Range(0, _smallStonePrefab.Length)]);
            _smallStoneSpawnedCount++;
        }

        private void SpawnStone(GameObject stonePrefab)
        {
            GameObject stoneObject = Instantiate(stonePrefab);
            Stone stone = stoneObject.GetComponent<Stone>();

            SubscribeOnStonesBrokenEvent(stone);
            IncreaseStonesCountInScene(stone);
            SetStonesHP(stone, Mathf.Max(0, (int)Random.Range(_startGeneratedStoneHP / 0.7f, _startGeneratedStoneHP * 1.3f)));

            stone.DisableMovement();

            int randomPosition = Random.Range(0, _startPoints.Length);
            stoneObject.transform.position = _startPoints[randomPosition].position;

            StartCoroutine(MoveStoneToPosition(stoneObject.transform, _endPoints[randomPosition].position));

            if (OnStoneSpawnedEvent != null)
                OnStoneSpawnedEvent(stone);
        }

        // Subscribe stone and its children to event when stone is broken
        private void SubscribeOnStonesBrokenEvent(Stone stone)
        {
            stone.OnBrokenEvent += OnStoneBroken;

            foreach (Stone s in stone.ChildStones)
                SubscribeOnStonesBrokenEvent(s);
        }

        private void IncreaseStonesCountInScene(Stone stone)
        {
            _stoneCountInScene++;

            foreach (Stone s in stone.ChildStones)
                IncreaseStonesCountInScene(s);
        }

        // Update all HP in stone and ith children stones
        private void SetStonesHP(Stone stone, int newHP)
        {
            stone.UpdateHP(newHP);

            foreach (Stone s in stone.ChildStones)
                SetStonesHP(s, newHP / 2);
        }

        // Update broken stone count and current stone count in the scene
        // Update startedGeneratedStoneHP
        // Spawn a new stone if stoneCountInScene less than maxStoneCountInScene
        private void OnStoneBroken()
        {
            _stoneCountInScene--;

            _brokenStonesCount++;

            _startGeneratedStoneHP += (int)(_player.ShootPower * _brokenStonesCount + (0.1f - _player.ShootSpeed) * _brokenStonesCount);

            if (_stoneCountInScene <= _maxStoneCountInScene)
            {
                if (_smallStoneSpawnedCount <= _smallStoneCount)
                {
                    SpawnSmallStone();
                }
                else if (_mediumStoneSpawnedCount <= _mediumStoneCount)
                {
                    SpawnMediumStone();
                }
                else
                {
                    SpawnBigStone();
                }
            }
        }

        private void SpawnSmallStone()
        {
            SpawnStone(_smallStonePrefab[Random.Range(0, _smallStonePrefab.Length)]);
            _smallStoneSpawnedCount++;
        }

        private void SpawnMediumStone()
        {
            SpawnStone(_mediumStonePrefab[Random.Range(0, _mediumStonePrefab.Length)]);
            _mediumStoneSpawnedCount++;
        }

        private void SpawnBigStone()
        {
            SpawnStone(_bigStonePrefab[Random.Range(0, _bigStonePrefab.Length)]);
        }

        private IEnumerator MoveStoneToPosition(Transform stoneTransform, Vector3 desiredPosition)
        {
            while (stoneTransform && Vector3.Distance(stoneTransform.position, desiredPosition) > 0.005f)
            {
                stoneTransform.position = Vector3.MoveTowards(stoneTransform.position, desiredPosition, _startMovementLerpSpeed * Time.deltaTime);

                yield return null;
            }

            if (stoneTransform)
                stoneTransform.GetComponent<Stone>().EnableMovement();
        }

    }

}