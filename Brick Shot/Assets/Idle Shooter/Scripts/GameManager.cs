using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager _instance;
    public Color[] colors; //Block and laser colors
    public Color[] fadeColors; //Fade colors for blocks


    public List<GameObject> blocks; //Spawned blocks in the screen
    public List<Block> blocksToSpawn; //The available blocks to spawn
    public GameObject laser; //laser prefab
    public GameObject[] blockMaps; //Boss waves to spawn

    public int blocksPerWave;
    public long averageHp;

    public GameObject[] explosionPrefab;

    Transform prevWaveCenter; //Previous wave transform -> if its 'y' position is below a value the next wave spawns

    int waveNum = 0; //every 5th wave is boss wave

    public float boostTime;
    public float tapBoostTime;

    public Sprite[] bgs; //background sprites, they are changed after every level up
    public GameObject bg;

    public GameObject minusText;

    public GameObject bossProgress;

    public bool isPaused = false;

    public bool hasBackground = true;
    public Sprite square;

    private void Awake() {
        _instance = this;
    }

    bool firstLaunch;
    public GameObject tutorialMsg;
    // Use this for initialization
    void Start() {
        Application.targetFrameRate = 300;

        bg.GetComponent<SpriteRenderer>().sprite = bgs[Random.Range(0, bgs.Length)]; //choosing random background

        blocks = new List<GameObject>();

        firstLaunch = true;


        if (!PlayerPrefs.HasKey("FIRST_LAUNCH")) {
            PlayerPrefs.SetInt("FIRST_LAUNCH", 1);
            StartCoroutine(FirstLaunch());
        } else firstLaunch = false;
    }

    /// <summary>
    /// First time launching the game -> showing tutorial messages
    /// </summary>
    /// <returns></returns>
    IEnumerator FirstLaunch() {
        tutorialMsg.SetActive(true);
        yield return new WaitForSeconds(8f);

        tutorialMsg.SetActive(false);
        firstLaunch = false;
    }
    // Update is called once per frame
    void Update() {
        boostTime -= Time.deltaTime;
        tapBoostTime -= Time.deltaTime;

        StartNewWave();
    }

    public void ChangeBg() {
        StartCoroutine(FadeBg());
    }

    /// <summary>
    /// Fading animation for nicer background changing
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeBg() {

        float t = 1;
        string prevName = bg.GetComponent<SpriteRenderer>().sprite.name;

        while (t > 0) {
            t -= Time.deltaTime;
            bg.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 80 / 255f, 80 / 255f, t);

            yield return null;
        }

        do {
            bg.GetComponent<SpriteRenderer>().sprite = bgs[Random.Range(0, bgs.Length)];
        } while (bg.GetComponent<SpriteRenderer>().sprite.name == prevName);

        while (t < 1) {
            t += Time.deltaTime;
            bg.GetComponent<SpriteRenderer>().color = new Color(80 / 255f, 80 / 255f, 80 / 255f, t);

            yield return null;
        }
    }

    /// <summary>
    /// Starting new wave
    /// </summary>
    void StartNewWave() {
        if (firstLaunch) return;

        if (blocks.Count < 1 || prevWaveCenter.position.y < -5f && prevWaveCenter != null) {
            averageHp = (int)(5 + LevelBar._instance.level * 5f);
            if (LevelBar._instance.level > 9) {
                averageHp += (int)Mathf.Pow((LevelBar._instance.level - 6), 2);
                averageHp = (int)Mathf.Clamp(averageHp, (int)Statistics._instance.ballDamage * 4, (int)Statistics._instance.ballDamage * 40);
            }

            averageHp += (LevelBar._instance.level / 15) * 20 + (LevelBar._instance.level / 30) * 45;

            waveNum++;

            if ((waveNum + 1) % 5 == 0) //if 5th wave -> spawn boss
                BossWave();
            else Spawn();
        }
    }

    /// <summary>
    /// Instantiating new wave gameobject
    /// and making blocks and lasers
    /// </summary>
    void Spawn() {
        bossProgress.SetActive(false);

        GameObject g = new GameObject();
        g.transform.position = new Vector3(0, 8f, 0);
        g.AddComponent<MoveDown>();

        prevWaveCenter = g.transform;

        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < Random.Range(39, 53); i++) {
            long hp = ((long)Random.Range(averageHp * 0.6f + (i / 2f) * 0.2f, averageHp * 0.8f + (i / 2f) * 0.2f));

            GameObject newBlock;
            int n = Random.Range(0, 100);

            if (n > 20) //80% chance to spawn square
                newBlock = Instantiate(blocksToSpawn[0].gameObject);
            else if (n > 3) newBlock = Instantiate(blocksToSpawn[Random.Range(1, blocksToSpawn.Count)].gameObject); //17% chance to spawn trianlge
            else newBlock = Instantiate(laser); //3% chance to spawn laser

            newBlock.transform.SetParent(g.transform);
            newBlock.transform.localScale = Vector3.one * 0.55f;
            if (n <= 3) {
                newBlock.transform.localScale = Vector3.one * 0.2f;

                if (Random.Range(0, 100) < 25)
                    newBlock.GetComponent<Laser>().isVertical = true;
            }

            newBlock.transform.localPosition = GetBlockPosition(positions);

            if (newBlock.GetComponent<Block>() != null)
                newBlock.GetComponent<Block>().hp = hp;

            if (newBlock.GetComponent<Laser>() == null)
                blocks.Add(newBlock.gameObject);
        }
    }

    /// <summary>
    /// Creating Boss wave
    /// </summary>
    void BossWave() {
        bossProgress.SetActive(true);

        GameObject g = Instantiate(blockMaps[Random.Range(0, blockMaps.Length)]);
        g.transform.position = new Vector3(0, 8f, 0);

        bossProgress.GetComponent<BossProgression>().bossHolder = g;
        prevWaveCenter = g.transform;

        var bs = g.GetComponentsInChildren<Block>();
        foreach (Block b in bs) {
            b.hp = (long)Random.Range(averageHp * 0.6f, averageHp * 0.8f);
            blocks.Add(b.gameObject);
        }
    }

    /// <summary>
    /// Returning Vector3 for block position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Vector3 GetBlockPosition(List<Vector2> pos) {
        int x, y;
        do {
            x = Random.Range(-3, 5);
            y = Random.Range(-3, 4);
        } while (pos.Contains(new Vector2(x, y)));

        pos.Add(new Vector2(x, y));

        Vector3 vec = new Vector3();
        vec = new Vector3(x * 0.67f - 0.3f, y * 0.67f, 0);

        return vec;

    }

    /// <summary>
    /// For Game Over Effect
    /// </summary>
    public void DestroyAllBlocks() {
        foreach (GameObject g in blocks) {
            if (g != null && g.GetComponent<Block>() != null)
                g.GetComponent<Block>().DestroyBlock();
        }

        blocks.Clear();
    }

    public void Pause(bool pause) {
        if (pause) {
            isPaused = true;

            Player._instance.BallsReturn();

        } else {
            isPaused = false;

        }
    }
}
