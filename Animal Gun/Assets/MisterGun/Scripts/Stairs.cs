using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class Stairs : MonoBehaviour
{
    public float Step;

    [HideInInspector]
    public float YPos;

    private int _randStairsCount;

    public BoxCollider2D[] StairColliders;
    public SpriteRenderer[] StairRenderer;
    public SortingGroup CurrentGroup;
    public SpriteRenderer Environment;
    public GameObject EnemyPrefab;

    private Transform _environmentTransforms;
    private Color _currentColor;
    private Color _tempColor;

    private void Update()
    {
        if (CameraFollow.CameraIsMove)
        {
            if (_currentColor.r < _tempColor.r)
            {
                _currentColor.r += 0.0005f;
                _currentColor.g += 0.0005f;
                _currentColor.b += 0.0005f;
            }

            for (int i = 0; i < _randStairsCount; i++)
            {
                StairRenderer[i].color = _currentColor;
            }
            Environment.color = _currentColor;
        }
    }

    public void GenerateStair(bool isLeft)
    {
        _randStairsCount = Random.Range(3, StairRenderer.Length);

        YPos = _randStairsCount * Step;

        float xOffset = 0;

        switch (_randStairsCount)
        {
            case 3:
                xOffset = 3.5f;
                break;
            case 4:
                xOffset = 3f;
                break;
            case 5:
                xOffset = 2.2f;
                break;
            case 6:
                xOffset = 2.5f;
                break;
        }


        if (!isLeft)
        {
            transform.position = new Vector2(xOffset, 0);

            for (int i = 0; i < _randStairsCount; i++)
            {
                StairRenderer[i].gameObject.SetActive(true);
                StairRenderer[i].transform.position = new Vector2(StairRenderer[i].transform.position.x + Step * i, StairRenderer[i].transform.position.y + Step * i);
                _environmentTransforms = StairRenderer[i].transform;
            }
        }
        else
        {
            transform.position = new Vector2(-xOffset, 0);
            Environment.transform.position = new Vector2(Environment.transform.position.x + 0.5f, Environment.transform.position.y);

            for (int i = 0; i < _randStairsCount; i++)
            {
                StairRenderer[i].gameObject.SetActive(true);
                StairRenderer[i].transform.position = new Vector2(StairRenderer[i].transform.position.x - Step * i, StairRenderer[i].transform.position.y + Step * i);
                _environmentTransforms = StairRenderer[i].transform;
            }
        }
        StartCoroutine(CreateEnvironment(isLeft));
    }

    private IEnumerator CreateEnvironment(bool isLeft)
    {
        yield return new WaitForSeconds(0.1f);

        if (!isLeft)
        {
            Environment.transform.position = new Vector2(_environmentTransforms.transform.position.x - Random.Range(0.5f, 2f), _environmentTransforms.transform.position.y + Step);
            EnemyPrefab.transform.position = new Vector2(_environmentTransforms.transform.position.x + 2f, _environmentTransforms.transform.position.y + 0.8f);
            EnemyPrefab.transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            Environment.transform.position = new Vector2(_environmentTransforms.transform.position.x + Random.Range(0.5f, 2f), _environmentTransforms.transform.position.y + Step);
            EnemyPrefab.transform.position = new Vector2(_environmentTransforms.transform.position.x - 2f, _environmentTransforms.transform.position.y + (Step * 2));
        }

    }

    public void ChangeStairsColor(Color currentColor, Color mainColor)
    {
        for (int i = 0; i < _randStairsCount; i++)
        {
            StairRenderer[i].color = currentColor;
        }
        Environment.color = currentColor;
        _currentColor = currentColor;
        _tempColor = mainColor;
    }

    public void ChangeLayer(int LayerID)
    {
        CurrentGroup.sortingOrder = LayerID;
    }

    public void EnableColliders()
    {
        for (int i = 0; i < _randStairsCount; i++)
        {
            StairColliders[i].enabled = true;
        }
    }

    public void EnableEnemy()
    {
        EnemyPrefab.SetActive(true);
    }
}
