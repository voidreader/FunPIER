using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MergeDropArea : MonoBehaviour, IDropHandler {
    public void OnDrop(PointerEventData eventData) {

        if (MergeSystem.DraggingItem == null)
            return;

        if (MergeSystem.DraggingItem != null && MergeSystem.DraggingItem.IsBattle)
            return;


        if (MergeSystem.DraggingItem) {
            GameManager.main.SetEquipUnit(MergeSystem.DraggingItem);
        }
    }
}
