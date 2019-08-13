using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonerShadow : MonoBehaviour
{
    public Transform Target = null;
    int posY = 60;

    private void OnDisable() {
        Target = null;
    }

    public void SetTarget(Transform t) {
        this.gameObject.SetActive(true);

        string name = t.gameObject.GetComponent<Image>().sprite.name;
        switch(name) {
            case "Dr.Bald":
            case "MagicGirl":
                posY = 70;
                break;
                

            default:
                posY = 65;
                break;
        }

        


        Target = t;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
            return;

        this.transform.localPosition = new Vector3(Target.localPosition.x, Target.localPosition.y - 60, 0);


    }

    public void SetHide() {
        this.gameObject.SetActive(false);
    }
}
