using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    //TODO merge blocks  by collison
    public decimal hp;
    public decimal maxHp;
    public TextMesh hpText;

    Color fadeColor, startColor;

    GameManager gm;
    SpriteRenderer sr;
    Rigidbody2D rb;

    public int font1, font2, font3;

    public bool coloring = true;

    public SpriteRenderer sp;

    Vector3 baseScale; //for popping animation

    // Use this for initialization
    void Start() {
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();

        hpText = GetComponentInChildren<TextMesh>();
        hpText.text = Statistics._instance.GetSuffix(hp);

        //Setting color
        sr = GetComponent<SpriteRenderer>();
        int colorN = Random.Range(0, gm.colors.Length);
        startColor = sr.color = gm.colors[colorN];

        fadeColor = gm.fadeColors[colorN];


        maxHp = hp;

        if (coloring) {
            hpText.color = sr.color;
            sp.color = new Color(sr.color.r, sr.color.g, sr.color.b, 80f / 255f);
        }

        SetFontSize();

        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update() {
    }

    /// <summary>
    /// Collision events
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision) {
        //Colliding with ball
        if (collision.gameObject.tag == "Ball") {
            Statistics._instance.AddMoney(DMin(hp, collision.gameObject.GetComponent<Ball>().damage));
            if (IAP._instance.doublePurchased) Statistics._instance.AddMoney(DMin(hp, collision.gameObject.GetComponent<Ball>().damage));

            hp -= collision.gameObject.GetComponent<Ball>().damage;

            //If booster is active
            if (GameManager._instance.boostTime > 0) {
                Statistics._instance.AddMoney(collision.gameObject.GetComponent<Ball>().damage * 3);
                hp -= collision.gameObject.GetComponent<Ball>().damage;
            }

            HitByBall(0.05f);
        }


    }

    /// <summary>
    /// Trigger collision events
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        //if the block reaches the bottom boundary
        if (collision.tag == "Boundary") {
            gm.blocks.Remove(gameObject);
            if (LevelBar._instance.destroyedBlocks > -1) {
                float multiplier = 0.1f; // 0.1 means 10% of the current level's allBlocksToUpgrade variable - 0.5 would mean 50%
                LevelBar._instance.destroyedBlocks -= (int)(LevelBar._instance.allBlocksToUpgrade * multiplier); // loosing points from fillbar

                var g = Instantiate(gm.minusText); //Minus text animation
                g.transform.SetParent(gm.minusText.transform.parent);
                g.transform.localPosition = new Vector3(Random.Range(-250, 250), Random.Range(140, 167), 0);
                g.transform.localScale = Vector3.one;
                g.SetActive(true);
            }
        }

        //Laser collision
        if (collision.tag == "Merge") {
            decimal n = (int)(Statistics._instance.ballDamage);
            Statistics._instance.AddMoney(DMin(hp, n));
            hp -= n;
            if (IAP._instance.doublePurchased) Statistics._instance.AddMoney(DMin(hp, n));

            if (hp <= 0) {
                foreach (Level l in Statistics._instance.levels) {
                    if (l.type == Level.Type.laserDestroy)
                        l.progression++;
                }
            }

            HitByBall(0.05f);
        }
    }

    /// <summary>
    /// Destroying a block
    /// </summary>
    public void DestroyBlock() {
        var g = Instantiate(GameManager._instance.explosionPrefab[Random.Range(0, GameManager._instance.explosionPrefab.Length)]); //Explosion effect
        g.transform.position = transform.position;
        g.GetComponent<ParticleSystem>().startColor = sr.color;

        Destroy(gameObject);
    }

    /// <summary>
    /// OnHit event, scaling animation, checking HP
    /// </summary>
    /// <param name="scaleDif"></param>
    public void HitByBall(float scaleDif) {
        SetFontSize();

        if (hp <= 0) {
            gm.blocks.Remove(gameObject);

            LevelBar._instance.destroyedBlocks++;


            foreach(Level l in Statistics._instance.levels) {
                if (l.type == Level.Type.destroyBlock)
                    l.progression++;
            }

            DestroyBlock();
        }

        hpText.text = Statistics._instance.GetSuffix(hp);

        if (coloring) {
            sr.color = Color.Lerp(startColor, fadeColor, 1 - (float)(hp / maxHp));
            hpText.color = sr.color;
            sp.color = new Color(sr.color.r, sr.color.g, sr.color.b, 80f / 255f);
        }

        StopAllCoroutines();
        StartCoroutine(Scale(scaleDif));
    }

    IEnumerator Scale(float scaleDif) {
        float t = 0;

        while (t < 1) {
            t += Time.deltaTime * 17;
            transform.localScale = Vector3.Lerp(baseScale, baseScale - new Vector3(scaleDif, scaleDif, 0), t);

            yield return null;
        }

        StartCoroutine(ScaleBack(-scaleDif));
    }

    IEnumerator ScaleBack(float scaleDif) {
        //Vector3 baseScale = transform.localScale;
        float t = 0;

        while (t < 1) {
            t += Time.deltaTime * 17;
            transform.localScale = Vector3.Lerp(baseScale - new Vector3(scaleDif, scaleDif, 0), baseScale, t);

            yield return null;
        }
    }

    /// <summary>
    /// On Mouse / Touch event
    /// Called when Tapping a block
    /// </summary>
    private void OnMouseDown() {
        if (GameManager._instance.isPaused) return;

        Player._instance.isBlockTapped = true;

        foreach (Level l in Statistics._instance.levels) {
            if (l.type == Level.Type.tapDamage)
                l.progression += (int)Statistics._instance.tapDamage;
        }

        int multiplier = 1;
        if (gm.tapBoostTime > 0) multiplier = 5;

        hp -= Statistics._instance.tapDamage * multiplier;
        Statistics._instance.AddMoney(DMin(hp, Statistics._instance.tapDamage * 5));

        HitByBall(0.05f);
    }

    private void OnMouseOver() {

    }

    private void OnMouseUp() {
    }

    /// <summary>
    /// Called when mouse / touch is no more over a block
    /// </summary>
    private void OnMouseExit() {
        Player._instance.isBlockTapped = false;
    }

    decimal DMin(decimal d1, decimal d2) {
        if (d1 < d2) return d1;
        else return d2;
    }

    void SetFontSize() {
        if (hp < 10)
            hpText.fontSize = font1;
        else if (hp < 100) hpText.fontSize = font2;
        else hpText.fontSize = font3;
    }

    private void OnDestroy() {
        Player._instance.isBlockTapped = false;
    }
}
