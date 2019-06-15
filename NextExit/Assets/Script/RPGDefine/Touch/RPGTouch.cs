using UnityEngine;
using System.Collections;

/// <summary>
/// copy that tk2dTouch
/// </summary>
public class RPGTouch
{
    public const int MOUSE_POINTER_FINGER_ID = 9999; //id given to mouse pointer

    public bool IsEnable { get; set; }
    public TouchPhase phase { get; private set; }
    public int fingerId { get; private set; }
    public Vector2 position { get; private set; }
    public Vector2 deltaPosition { get; private set; }
    public float deltaTime { get; private set; }

    public RPGTouch()
    {
        IsEnable = false;
    }

    public RPGTouch(TouchPhase _phase, int _fingerId, Vector2 _position, Vector2 _deltaPosition, float _deltaTime)
        : this()
    {
        SetTouch(_phase, _fingerId, _position, _deltaPosition, _deltaTime);
    }

    public RPGTouch(Touch touch)
        : this()
    {
        SetTouch(touch);
    }

    public void SetTouch(TouchPhase _phase, int _fingerId, Vector2 _position, Vector2 _deltaPosition, float _deltaTime)
    {
        this.phase = _phase;
        this.fingerId = _fingerId;
        this.position = _position;
        this.deltaPosition = _deltaPosition;
        this.deltaTime = _deltaTime;
        IsEnable = true;
    }

    public void SetTouch(Touch touch)
    {
        SetTouch(touch.phase, touch.fingerId, touch.position, touch.deltaPosition, touch.deltaTime);
    }

    public override string ToString()
    {
        return IsEnable + "," + phase.ToString() + "," + fingerId + "," + position + "," + deltaPosition + "," + deltaTime;
    }
}