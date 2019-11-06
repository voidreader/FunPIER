using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashArea : MonoBehaviour, IDropHandler {
    public void OnDrop(PointerEventData eventData) {

        Debug.Log(">> OnDrop in TrashArea :: " + this.name);


        if (MergeSystem.DraggingItem == null)
            return;


        if (MergeSystem.DraggingItem && MergeSystem.DraggingItem.IsBattle)
            return;

        MergeSystem.DraggingItem.IsAlive = false;
    }
}
