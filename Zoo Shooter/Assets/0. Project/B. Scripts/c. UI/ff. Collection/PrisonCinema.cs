using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonCinema : MonoBehaviour
{

    public List<Button> ListPosters;
    public Text TextGate;

    public int MinList, MaxList;

    public int CurrentList;

    /// <summary>
    /// 시네마 세팅 
    /// </summary>
    public void SetCinema() {

        CurrentList = PIER.CurrentList;

        int posterIndex = MinList;
        bool isOpen = false; 

        for(int i=0; i<5; i++) {

            // ListPosters[i].image = Stocks.GetPosterSprite(posterIndex);
            if (posterIndex < PIER.CurrentList) {
                ListPosters[i].GetComponent<Image>().sprite = Stocks.GetPosterSprite(posterIndex);
                isOpen = true;
       
            }
            else {
                ListPosters[i].GetComponent<Image>().sprite = Stocks.main.SpriteComingSoon;

            }

            posterIndex++;

        }

        if(isOpen) {
            TextGate.text = "NOW SHOWING";
        }
        else {
            TextGate.text = "CLOSED";
        }
        
        
    }
}
