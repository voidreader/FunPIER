using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    public GameObject PlayerObj;

    [Space(10)]
    public GameObject obstacleCirclePrefab;
    public int bigCircleNumber;
    public float bigCircleSizeMax;
    public float bigCircleSizeMin;
    public int smallCircleNumber;
    public float smallCircleSizeMax;
    public float smallCircleSizeMin;

    [Space(10)]
    public GameObject obstacleRectanglePrefab;
    public int rectangleNumber;
    public float rectangleSizeMax;
    public float rectangleSizeMin;

    [Space(10)]
    public GameObject ItemPrefab;
    public int itemNumber;


    [Space(10)]
    public GameObject areaEffector;


    float distanceToFirstObstacle = 5;
    float distanceToNextObstacleGroup = 10;


    int obstacleIndex = 0;


    public GameObject obstaclesAndItemsGroupObj;

    GameObject parentObj;

    int mapWidth;



    void Start()
    {
        mapWidth = GameObject.Find("Wall").GetComponent<WallManager>().mapWidth;
        for (int i = 0; i < 3; i++)
        {
            MakeNextObstacleGroup();
        }
    }


    public void MakeNextObstacleGroup()
    {
        parentObj = Instantiate(obstaclesAndItemsGroupObj, new Vector2(0, distanceToFirstObstacle + obstacleIndex * distanceToNextObstacleGroup), Quaternion.identity);

        MakeObstacle(obstacleCirclePrefab, bigCircleNumber, bigCircleSizeMin, bigCircleSizeMax);
        MakeObstacle(obstacleCirclePrefab, smallCircleNumber, smallCircleSizeMin, smallCircleSizeMax);

        MakeObstacle(obstacleRectanglePrefab, rectangleNumber, rectangleSizeMin, rectangleSizeMax);

        MakeObstacle(areaEffector, 1, 1f, 1f);

        MakeObstacle(ItemPrefab, itemNumber, 1f, 1f);


        obstacleIndex++;
    }


    void MakeObstacle(GameObject prefab, int number, float minSize, float maxSize)
    {
        for (int i = 0; i < number; i++)
        {
            float posX = Random.Range(DisplayManager.DISPLAY_LEFT * mapWidth + 1, DisplayManager.DISPLAY_RIGHT * mapWidth - 1);
            float posY = Random.Range(distanceToFirstObstacle + obstacleIndex * distanceToNextObstacleGroup + 1, distanceToFirstObstacle + (obstacleIndex + 1) * distanceToNextObstacleGroup - 1);
            GameObject newObj = Instantiate(prefab, new Vector2(posX, posY), Quaternion.identity);
            float randomSize = Random.Range(minSize, maxSize);
            if (prefab == obstacleRectanglePrefab)
            {
                newObj.transform.localScale = new Vector2(newObj.transform.localScale.x * randomSize, newObj.transform.localScale.y * randomSize * 4);
            }
            else if (prefab == obstacleCirclePrefab)
            {
                newObj.transform.localScale = new Vector2(newObj.transform.localScale.x * randomSize, newObj.transform.localScale.y * randomSize);
            }
            else if (prefab == areaEffector)
            {
                newObj.transform.Rotate(0, 0, Random.Range(0, 360));
            }
            else if (prefab == ItemPrefab)
            {

            }

            newObj.transform.SetParent(parentObj.transform);
        }
    }




}

