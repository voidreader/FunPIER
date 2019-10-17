using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class Unit : MonoBehaviour
{

    public UnitDataRow _data;
    public string UID = string.Empty;

    public SpriteRenderer _body, _leg, _face, _weapon;
    public List<Transform> ListAimPoint = new List<Transform>();


    public void SetUnit(int level) {

        SetUnit(UnitData.Instance.GetRow("U" + level.ToString()));
        
    }

    public void SetUnit(UnitDataRow r) {
        _data = r;
        UID = _data._uid;

        _body.sprite = Stock.GetFriendlyUnitBody(_data._spriteBody);
        _leg.sprite = Stock.GetFriendlyUnitBody(_data._spriteLeg);
        _face.sprite = Stock.GetFriendlyUnitFace(_data._spriteFaceIdle);
        _weapon.sprite = Stock.GetFriendlyUnitWeapon(_data._weaponSprite);
        
        _leg.transform.localPosition = new Vector2(_data._legX, _data._legY);
        _face.transform.localPosition = new Vector2(_data._faceX, _data._faceY);
        _weapon.transform.localPosition = new Vector2(_data._weaponX, _data._weaponY);

        ListAimPoint.Clear(); // Aim Point


        SetAimPoint();
    }

    /// <summary>
    /// AimPoint설정 
    /// </summary>
    void SetAimPoint() {
        /*
        Transform[] tr = this.GetComponentsInChildren<Transform>(true);

        for(int i=0; i < tr.Length; i++) {
            if(tr[i].name.Contains("AimPoint")) {
                ListAimPoint.Add(tr[i]);
            }
        }
        */

        if (ListAimPoint.Count == 0)
            return;

        for(int i=0; i<ListAimPoint.Count;i++) {
            ListAimPoint[i].gameObject.SetActive(false);
        }

        // 무기 이름에 따라 AimPoint 다르게 설정
        switch(_data._weaponSprite) {

            case "gunParts1":
                ListAimPoint[0].localPosition = new Vector3(0.297f, 0.07f);
                ListAimPoint[0].gameObject.SetActive(true);
                break;

            case "gunParts2":
                ListAimPoint[0].localPosition = new Vector3(0.392f, 0.065f);
                ListAimPoint[0].gameObject.SetActive(true);
                break;

            case "gunParts3":
            case "gunParts4":
                ListAimPoint[0].localPosition = new Vector3(0.227f, 0.063f);
                ListAimPoint[1].localPosition = new Vector3(0.276f, 0.063f);

                ListAimPoint[0].gameObject.SetActive(true);
                ListAimPoint[1].gameObject.SetActive(true);
                break;

            case "gunParts5":
            case "gunParts6":
                ListAimPoint[0].localPosition = new Vector3(0.305f, 0.192f);
                ListAimPoint[1].localPosition = new Vector3(0.433f, 0.192f);

                ListAimPoint[0].gameObject.SetActive(true);
                ListAimPoint[1].gameObject.SetActive(true);
                break;

            case "gunParts7":
                ListAimPoint[0].localPosition = new Vector3(0.368f, 0.141f);
                ListAimPoint[1].localPosition = new Vector3(0.463f, 0.141f);

                ListAimPoint[0].gameObject.SetActive(true);
                ListAimPoint[1].gameObject.SetActive(true);
                break;

            case "gunParts8":
                ListAimPoint[0].localPosition = new Vector3(0.339f, 0.03f);
                ListAimPoint[0].gameObject.SetActive(true);
                break;

            case "gunParts9":
                ListAimPoint[0].localPosition = new Vector3(0.348f, 0.036f);
                ListAimPoint[0].gameObject.SetActive(true);
                break;

            case "gunParts10":
                ListAimPoint[0].localPosition = new Vector3(0.398f, 0.147f);
                ListAimPoint[1].localPosition = new Vector3(0.32f, 0.082f);
                ListAimPoint[2].localPosition = new Vector3(0.398f, 0.008f);
                ListAimPoint[3].localPosition = new Vector3(0.482f, 0.082f);

                ListAimPoint[0].gameObject.SetActive(true);
                ListAimPoint[1].gameObject.SetActive(true);
                ListAimPoint[2].gameObject.SetActive(true);
                ListAimPoint[3].gameObject.SetActive(true);
                break;

            case "gunParts11":
                ListAimPoint[0].localPosition = new Vector3(0.19f, 0.08f);
                ListAimPoint[1].localPosition = new Vector3(0.11f, 0.012f);
                ListAimPoint[2].localPosition = new Vector3(0.19f, -0.067f);
                ListAimPoint[3].localPosition = new Vector3(0.274f, 0.012f);

                ListAimPoint[4].localPosition = new Vector3(0.443f, 0.08f);
                ListAimPoint[5].localPosition = new Vector3(0.363f, 0.012f);
                ListAimPoint[6].localPosition = new Vector3(0.443f, -0.067f);
                ListAimPoint[7].localPosition = new Vector3(0.527f, 0.012f);

                ListAimPoint[0].gameObject.SetActive(true);
                ListAimPoint[1].gameObject.SetActive(true);
                ListAimPoint[2].gameObject.SetActive(true);
                ListAimPoint[3].gameObject.SetActive(true);
                ListAimPoint[4].gameObject.SetActive(true);
                ListAimPoint[5].gameObject.SetActive(true);
                ListAimPoint[6].gameObject.SetActive(true);
                ListAimPoint[7].gameObject.SetActive(true);
                break;

        }

    }

    public Transform GetAimPoint(int muzzleindex) {
        return ListAimPoint[muzzleindex];
    }

}
