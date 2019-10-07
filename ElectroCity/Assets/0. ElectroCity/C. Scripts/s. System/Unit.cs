using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class Unit : MonoBehaviour
{

    public UnitDataRow _data;
    public string UID = string.Empty;

    public SpriteRenderer _body, _leg, _face, _weapon;


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

    }

}
