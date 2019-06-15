//---------------------------------------------------
// Transform의 좌표를 이동버튼으로 이동가능하도록 합니다.
//---------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
//[CanEditMultipleObjects]
public class RPGEditorMoveTransform : Editor
{
    const float PixelPerMeter = 20.0f;
    float m_moveSize = 1.0f / PixelPerMeter;


    public override void OnInspectorGUI()
    {
        Transform t = target as Transform;
        

        if(GUILayout.Button("Reset Transforms")) {
            Undo.RecordObject(t, "Reset Transforms " + t.name);
            t.transform.position = Vector3.zero;
            t.transform.rotation = Quaternion.identity;
            t.transform.localScale = Vector3.one;
        }

        // Tranform의 기본형태를 유지하도록 합니다.
        //EditorGUIUtility.LookLikeControls();
        EditorGUI.indentLevel = 0;
        // Pixel Per Meter를 적용하도록 조정.
        t.localPosition = EditorGUILayout.Vector3Field("Position", t.localPosition * PixelPerMeter, GUILayout.MinWidth(200)) / PixelPerMeter;
        t.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles, GUILayout.MinWidth(200));
        t.localScale = EditorGUILayout.Vector3Field("Scale", t.localScale, GUILayout.MinWidth(200));
        //EditorGUIUtility.LookLikeInspector();
    }


    public void OnSceneGUI ()
    {
        Transform t = target as Transform;

        Event e = Event.current;
        int id = GUIUtility.GetControlID(FocusType.Passive);
        EventType type = e.GetTypeForControl(id);

        if (type == EventType.KeyDown)
        {
            Vector3 pos = t.localPosition;
            switch(e.keyCode)
            {
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                    m_moveSize = 10.0f / PixelPerMeter;
                    break;
                case KeyCode.UpArrow: pos.y += m_moveSize; break;
                case KeyCode.DownArrow: pos.y -= m_moveSize; break;
                case KeyCode.LeftArrow: pos.x -= m_moveSize; break;
                case KeyCode.RightArrow: pos.x += m_moveSize; break;
            }
            t.localPosition = pos;
            e.Use();
        }
        if (type == EventType.KeyUp)
        {
            switch(e.keyCode)
            {
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                    m_moveSize = 1.0f / PixelPerMeter;
                    break;
            }
        }
    }
}
