using System;
using System.Collections.Generic;
using UnityEngine;

public class StairsManager : MonoBehaviour
{
    public static Action EnableColliderAction;

    public GameManager GameManager;
    public Stairs StairsPrefab;
    public Color[] StartColors;
    private Color _startStairsColor;

    [HideInInspector]
    public List<Stairs> StairsList = new List<Stairs>();

    private int _stairsCount;

    private bool _isLeftDirection = true;

    private int _currentStairs;

    private void OnEnable()
    {
        EnableColliderAction += EnableStairCollider;
    }

    private void OnDisable()
    {
        EnableColliderAction -= EnableStairCollider;
    }

    void Start()
    {
        _stairsCount = GameManager.StairsCount;
        _currentStairs = 0;
        int randColorId = UnityEngine.Random.Range(0, StartColors.Length);
        _startStairsColor = StartColors[randColorId];
        Camera.main.backgroundColor = _startStairsColor;

        for (int i = 0; i < _stairsCount; i++)
        {
            // Change stairs direction (left or right)
            _isLeftDirection = !_isLeftDirection;

            Stairs newStair = Instantiate(StairsPrefab, null, false);
            newStair.GenerateStair(_isLeftDirection);
            StairsList.Add(newStair);

            if (i == 0)
            {
                newStair.transform.position = new Vector2(newStair.transform.position.x, -1.6f);
            }
            else
            {
                StairsList[i].transform.position = new Vector2(newStair.transform.position.x, (StairsList[i - 1].transform.position.y + StairsList[i - 1].YPos) - 0.4f);

            }
            // Change stairs colors
            newStair.ChangeStairsColor(_startStairsColor, StartColors[randColorId]);

            _startStairsColor.r -= 0.08f;
            _startStairsColor.g -= 0.08f;
            _startStairsColor.b -= 0.08f;
            newStair.ChangeLayer(-i);
        }
        EnableColliderAction();
    }

    private void EnableStairCollider()
    {
        if (_currentStairs < StairsList.Count)
        {
            StairsList[_currentStairs].EnableColliders();
            StairsList[_currentStairs].EnableEnemy();
            _currentStairs++;
        }
    }
}
