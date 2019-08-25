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

            case "CatAunt":
            case "FishMan":
                posY = 50;
                break;

            case "Alien-Yo":
            case "Chun-li":
            case "GasMask":
            case "Gevin":
            case "Halface":
            case "Hero-Je":
            case "HeyGrandfa":
            case "Hollyshit":
            case "Killer":
            case "Lizard":
            case "Princess-A":
            case "ShooterMan":
            case "StarFish":
                posY = 55;
                break;

            case "Clown":
            case "Diablo":
            case "Greenman":
            case "Guile":
            case "ken":
            case "ryu":
            case "S.Witch":
                posY = 60;
                break;


            case "Dr.Bald":
            case "MagicGirl":
            case "Dhalim":
            case "R.Harry":
                posY = 70;
                break;

            case "BadPenguin":
            case "Triangle":
                posY = 75;
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

        this.transform.localPosition = new Vector3(Target.localPosition.x, Target.localPosition.y - posY, 0);


    }

    public void SetHide() {
        this.gameObject.SetActive(false);
    }
}
