using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controling the waves
/// </summary>
public class MoveDown : MonoBehaviour {

    public float speed;
    int laserNum; //How many lasers are in the wave (lasers are not counted in perfect wave so we need to know the amount of the lasers

    // Use this for initialization
    void Start() {
        StartCoroutine(StartGravity());
        laserNum = GetComponentsInChildren<Laser>().Length;
    }

    private void FixedUpdate() {
        if (GameManager._instance.isPaused) return;

        //Moving down
        transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
    }

    // Update is called once per frame
    void Update() {
        if (GameManager._instance.isPaused) return;

        if (speed != 2f)
            speed = Statistics._instance.blockSpeed;

        //if only lasers left in the wave
        if (transform.childCount - laserNum < 1) DestructionBonus();

        if (transform.position.y < -10) Destroy(gameObject);
    }

    /// <summary>
    /// Perfect Wave effect
    /// </summary>
    void DestructionBonus() {
        var g = Instantiate(Resources.Load<GameObject>("perfectWave" + Random.Range(0, 3)));
        g.transform.position = Vector3.zero;
        g.transform.localScale = Vector3.one;

        g.GetComponentsInChildren<TextMesh>()[1].text = "+ " + Statistics._instance.GetSuffix(Statistics._instance.DMax(75, (decimal.Round(Statistics._instance.ballDamage * (int)Statistics._instance.fireRate * (int)Statistics._instance.fireRate * LevelBar._instance.level))));

        Statistics._instance.AddMoney(decimal.Round(Statistics._instance.DMax(75, (decimal.Round(Statistics._instance.ballDamage * (int)Statistics._instance.fireRate * (int)Statistics._instance.fireRate * LevelBar._instance.level * 2)))));

        foreach (Level l in Statistics._instance.levels) {
            if (l.type == Level.Type.perfectWave)
                l.progression++;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Increasing velocity, so the wave goes down faster at the beginning
    /// </summary>
    /// <returns></returns>
    IEnumerator StartGravity() {
        speed = 2f;

        yield return new WaitForSeconds(1.5f);

        speed = Statistics._instance.blockSpeed;

    }
}
