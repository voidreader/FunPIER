using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class PassiveColumn : MonoBehaviour {
    public Image imageBG, imageIcon;
    public Text textTitle, textPrice, textLevel, textInfo;
    public GameObject Cover;

    PassiveDataRow row;
    

    [SerializeField] int _factor;
    [SerializeField] long _price;

    
    public void InitPassiveColumn(PassiveDataRow r) {
        row = r;


        _factor = int.Parse(r._factor);
        _price = long.Parse(r._price);
        




    }

}
