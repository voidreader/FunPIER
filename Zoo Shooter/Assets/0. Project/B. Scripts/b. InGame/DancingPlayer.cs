using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancingPlayer : MonoBehaviour
{
    public Animator anim;

    int count = 0;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = Vector3.one;
        count = 0;
    }

    void OnDirection() {
        count++;

        if( count % 2 ==0)
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, 1, 1);
    }

    public void SetPosition(Vector3 pos) {
        this.transform.position = pos;
        this.gameObject.SetActive(true);
    }
    
}
