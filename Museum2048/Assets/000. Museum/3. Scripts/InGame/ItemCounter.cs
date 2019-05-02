using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCounter : MonoBehaviour {


    public UILabel label;
    public GameObject sign = null;
    public string itemID;
    public static System.Action RefreshItems = delegate { };

    private void Awake() {
        
        RefreshItems += RefreshLabel;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    public void RefreshLabel() {
        if (!label)
            return;

        switch(itemID) {
            case "back":
                label.text = PierSystem.main.itemBack.ToString();
                break;

            case "cleaner":
                label.text = PierSystem.main.itemCleaner.ToString();
                break;

            case "upgrader":
                label.text = PierSystem.main.itemUpgrade.ToString();
                break;


        }

        if (label.text != "0") {
            sign.SetActive(true);
        }
        else
            sign.SetActive(false);
    }
}
