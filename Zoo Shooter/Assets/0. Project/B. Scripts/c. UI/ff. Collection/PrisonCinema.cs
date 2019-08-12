using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrisonCinema : MonoBehaviour
{

    public List<Button> ListPosters;
    public List<Prisoner> ListPrisoners;
    public Text TextGate;

    public int MinList, MaxList;

    public int CurrentList;
    public int MaxListLevel; 
    public bool IsOpen = false;

    /// <summary>
    /// 시네마 세팅 
    /// </summary>
    public void SetCinema() {

        this.gameObject.SetActive(true);

        CurrentList = PIER.CurrentList;
        MaxListLevel = PIER.main.GetMaxLevelFromList(CurrentList);
        TextGate.text = string.Empty;


        int posterIndex = MinList;
        IsOpen = false;

        // 초기 세팅을 진행한다. 
        // 

        for(int i=0; i<5; i++) {

            // ListPosters[i].image = Stocks.GetPosterSprite(posterIndex);
            if (posterIndex < PIER.CurrentList) {
                // ListPosters[i].GetComponent<Image>().sprite = Stocks.GetPosterSprite(posterIndex);
                ListPosters[i].GetComponent<CinemaPoster>().SetPoster(posterIndex);
                IsOpen = true;
       
            }
            else {
                //ListPosters[i].GetComponent<Image>().sprite = Stocks.main.SpriteComingSoon;
                ListPosters[i].GetComponent<CinemaPoster>().SetComingSoon(posterIndex);
            }

            posterIndex++;
        }

        // 죄수들 초기화
        for(int i=0;i<ListPrisoners.Count;i++) {
            ListPrisoners[i].SetHide();
        }

        // 하나라도 오픈되어 있다면, 
        if(IsOpen) {
            SetOpen();
        }
        else {
            SetClose();
        }
    }

    public void SetOpen() {
        if (TextGate.text == "NOW SHOWING")
            return;

        int prisonerCount = Random.Range(2, 8);
        TextGate.text = "NOW SHOWING";
        IsOpen = true;


        for(int i = 0; i < prisonerCount; i++) {
            ListPrisoners[i].SetPrisoner();
        }
        
        
    }

    public void SetClose() {
        TextGate.text = "CLOSED";
        IsOpen = false;
    }
}
