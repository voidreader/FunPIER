using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class LabelLocalize : MonoBehaviour
{
    public MLocal.rowIds textID;
    UILabel _lbl;

    // Start is called before the first frame update
    void Start()
    {
        _lbl = GetComponent<UILabel>();
        if(_lbl) {
            _lbl.text = PierSystem.GetLocalizedText(textID);
        }
    }

    
}
