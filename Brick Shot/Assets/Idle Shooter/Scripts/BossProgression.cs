using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BossProgression : MonoBehaviour {

    public GameObject bossHolder;
    public Image fillImage;

    Block[] blocks;
    float allHp, currentHp;
	// Use this for initialization
	void Start () {
		
	}

    bool questCounted = false;

	// Update is called once per frame
	void Update () {
        fillImage.fillAmount = currentHp / allHp;

        if (fillImage.fillAmount < 0.01f && !questCounted) {
            foreach (Level l in Statistics._instance.levels) {
                questCounted = true;
                if (l.type == Level.Type.destroyBoss)
                    l.progression++;
            }
        }

        GetHPs();
    }

    private void OnEnable() {
        fillImage.fillAmount = 1;

        StartCoroutine(AssignBlocks());
    }

    IEnumerator AssignBlocks() {
        while (bossHolder == null) yield return null;

        yield return new WaitForSeconds(0.15f);

        blocks = bossHolder.GetComponentsInChildren<Block>();

        GetHPs();

        allHp = currentHp;
    }

    void GetHPs() {
        if (blocks == null) return;

        currentHp = 0;

        foreach (Block b in blocks) {
            currentHp += (float)b.hp;
        }
    }
}
