using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossModifier : MonoBehaviour {

    public int multiplier;
	// Use this for initialization
	IEnumerator Start () {
        Block b = GetComponent<Block>();

        while (b.maxHp == 0) yield return null;

        b.hp = b.maxHp = b.maxHp * multiplier;
        b.hpText.text = Statistics._instance.GetSuffix(b.hp);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
