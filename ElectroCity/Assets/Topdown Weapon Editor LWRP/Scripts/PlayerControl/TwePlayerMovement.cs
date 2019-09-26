using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwePlayerMovement : MonoBehaviour
{

    CharacterController cc;

    public float moveSpeed = 1;
    public float gravity;
    public GameObject CameraObject;

    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        moveDirection = Quaternion.Euler(0,CameraObject.transform.rotation.eulerAngles.y,0) * moveDirection;
        moveDirection *= moveSpeed;

        moveDirection.y -= gravity * Time.deltaTime;

        cc.Move(moveDirection * Time.deltaTime);
    }
}
