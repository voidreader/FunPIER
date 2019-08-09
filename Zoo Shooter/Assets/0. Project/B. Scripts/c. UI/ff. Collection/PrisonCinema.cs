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
    public int MaxListLevel; 
    public bool IsOpen = false;

    /// <summary>
    /// 시네마 세팅 
    /// </summary>
    public void SetCinema() {

        CurrentList = PIER.CurrentList;
        MaxListLevel = PIER.main.GetMaxLevelFromList(CurrentList);


        int posterIndex = MinList;
        bool isOpen = false; 

        // 초기 세팅을 진행한다. 
        // 

        for(int i=0; i<5; i++) {

            // ListPosters[i].image = Stocks.GetPosterSprite(posterIndex);
            if (posterIndex < PIER.CurrentList) {
                // ListPosters[i].GetComponent<Image>().sprite = Stocks.GetPosterSprite(posterIndex);
                ListPosters[i].GetComponent<CinemaPoster>().SetPoster(posterIndex);
                isOpen = true;
       
            }
            else {
                //ListPosters[i].GetComponent<Image>().sprite = Stocks.main.SpriteComingSoon;
                ListPosters[i].GetComponent<CinemaPoster>().SetComingSoon();
            }

            posterIndex++;

        }

        // 하나라도 오픈되어 있다면, 
        if(isOpen) {
            SetOpen();
        }
        else {
            SetClose();
        }
    }

    public void SetOpen() {
        if (IsOpen)
            return;

        TextGate.text = "NOW SHOWING";
        IsOpen = true;
        
    }

    public void SetClose() {
        TextGate.text = "CLOSED";
        IsOpen = false;
    }
}
