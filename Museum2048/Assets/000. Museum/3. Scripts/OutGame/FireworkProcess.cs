using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class FireworkProcess : MonoBehaviour {
    void OnSpawned() {

        // Tail 효과음 
        if(Random.Range(0,2) % 2 == 0)
            AudioAssistant.Shot("Tail1");
        else
            AudioAssistant.Shot("Tail2");


        StartCoroutine(DelaySound());

    }

    IEnumerator DelaySound() {
        yield return new WaitForSeconds(1.8f);

        int r = Random.Range(0, 3) % 3;


        if (r % 3 == 0)
            AudioAssistant.Shot("SFXFirework2");
        else if (r % 3 == 1)
            AudioAssistant.Shot("SFXFirework3");
        else
            AudioAssistant.Shot("SFXFirework5");
    }
}
