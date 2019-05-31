using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpriteChanger : MonoBehaviour {

    public Sprite[] Sprites;
    public SpriteRenderer CurrentRenderer;

    public Transform CurrentStairs;

    private void OnEnable()
    {
        CurrentRenderer.sprite = Sprites[Random.Range(0, Sprites.Length)];
        if (CurrentRenderer.sprite.name == "4" || CurrentRenderer.sprite.name == "6")
        {
            Invoke("ChangeDirection", 0.1f);
        }
    }

    private void ChangeDirection()
    {
        if (CurrentStairs.transform.position.x > 0)
        {
            CurrentRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            CurrentRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
