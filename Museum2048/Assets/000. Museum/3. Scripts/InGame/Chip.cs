using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using DG.Tweening;
    

public class Chip : MonoBehaviour {

    public int id;
    public Vector3 originScale;

    // Start is called before the first frame update
    void Start() {
        
    }

    /// <summary>
    /// 칩 위치 초기화 및 위치잡기 
    /// </summary>
    /// <param name="tile"></param>
    public void Init(Transform tile) {
        this.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + GetChipY(id), tile.transform.position.z);
        this.transform.localEulerAngles = Vector3.zero;
        
    }

    /// <summary>
    /// 위치.. 
    /// </summary>
    /// <returns></returns>
    public static float GetChipY(int id) {

        if(InGame.currentTheme == Theme.Car) {

            if (id == 4)
                return 1;
            else if (id == 8)
                return 0.7f;
            else if (id == 16)
                return 0.6f;
            else if (id == 32)
                return 0.5f;
            else if (id == 64)
                return 0.75f;
            else if (id == 128)
                return 0.75f;
            else if (id == 256)
                return 0.47f;
            else if (id == 512)
                return 0.48f;

        }


        return 0.5f; // 일반적으로 0.5f
    }

    

    void OnSpawned() {
        this.transform.localScale = Vector3.zero;



        if (InGame.currentTheme == Theme.Prototype) {

            // 임시로직인데.. 
            if (id == 2)
                originScale = Vector3.one * 0.5f;
            else if (id == 4)
                originScale = Vector3.one * 0.6f;
            else if (id == 8)
                originScale = Vector3.one * 0.7f;
            else if (id == 16)
                originScale = Vector3.one * 0.8f;
            else if (id == 32)
                originScale = Vector3.one * 0.9f;
            else if (id == 64)
                originScale = Vector3.one * 1f;
            else if (id == 128)
                originScale = Vector3.one * 1f;
            else if (id == 256)
                originScale = Vector3.one * 1.2f;
            else if (id == 512)
                originScale = Vector3.one * 1.3f;
            else if (id == 1024)
                originScale = Vector3.one * 1.4f;
            else if (id == 2048)
                originScale = Vector3.one * 1.5f;
        }
        else {
            originScale = Vector3.one;
        }

        this.transform.DOScale(originScale, 0.2f).SetEase(Ease.OutBack);
    }


    void OnDespawned() {
        this.transform.DOKill();
    }

    public int GetScore() {
        switch(id) {
            case 2:
                return id;
            case 4:
                return id;
            case 8:
                return id;
            case 16:
                return id;
            case 32:
                return id * 2;
            case 64:
                return id * 2;
            case 128:
                return id * 2;
            case 256:
                return id * 3;
            case 512:
                return id * 3;
            case 1024:
                return id * 4;
            case 2048:
                return id * 4;
            case 4096:
                return id * 6;
            case 8192:
                return id * 8;
            case 16384:
                return id * 10;
        }

        return 0;
    }


    
}
