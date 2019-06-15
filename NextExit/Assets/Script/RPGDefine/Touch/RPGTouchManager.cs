using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif
public class RPGTouchManager : RPGSingleton<RPGTouchManager>
{
    /// <summary>
    /// UI 카메라.
    /// </summary>
    public Camera uiCamera;
    /// <summary>
    /// 터치 포인트 목록.
    /// </summary>
    public RPGTouch[] Touches = new RPGTouch[2];

    Ray ray;
    RaycastHit hit;

    public override void Init()
    {
        base.Init();

        if (uiCamera == null)
            uiCamera = Camera.main;
        Touches[0] = new RPGTouch();
        Touches[1] = new RPGTouch();
    }

    void Update()
    {
        int touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            // 멀티 터치가 가능한 디바이스.
            for (int i=0; i<2;i++)
            {
                if (touchCount > i)
                    Touches[i].SetTouch(Input.GetTouch(i));
                else
                    Touches[i].IsEnable = false;
            }
        }
        else
        {
            // 멀티 터치가 불가능한 디바이스.
            if (Input.GetMouseButtonDown(0))
                Touches[0].SetTouch(TouchPhase.Began, RPGTouch.MOUSE_POINTER_FINGER_ID, Input.mousePosition, Vector2.zero, Time.deltaTime);
            else if (Input.GetMouseButtonUp(0))
                Touches[0].SetTouch(TouchPhase.Ended, RPGTouch.MOUSE_POINTER_FINGER_ID, Input.mousePosition, Vector2.zero, Time.deltaTime);
            else if (Touches[0].IsEnable && (Touches[0].phase == TouchPhase.Began || Touches[0].phase == TouchPhase.Moved))
                Touches[0].SetTouch(TouchPhase.Moved, RPGTouch.MOUSE_POINTER_FINGER_ID, Input.mousePosition, Vector2.zero, Time.deltaTime);
            else
                Touches[0].IsEnable = false;
        }
    }

    public Transform RaycastForTransform(Vector2 screenPos)
    {
        ray = uiCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out hit, uiCamera.farClipPlane - uiCamera.nearClipPlane, uiCamera.cullingMask))
        {
            return hit.transform;
        }
        return null;
    }

}
