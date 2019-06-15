using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
    public class ElevatorPlatform : ParentOnStandPlatform
    {
        /// <summary>
        /// Floor heights measured from the starting position.
        /// </summary>
        [Tooltip ("Floor heights measured from the starting position.")]
        public float[] floorHeights;

        /// <summary>
        /// Speed the elevator moves at.
        /// </summary>
        [Tooltip("Speed the elevator moves at.")]
        public float speed;

        /// <summary>
        /// Do we allow floor to be changed while moving?
        /// </summary>
        [Tooltip("Do we allow floor to be changed while moving?")]
        public bool allowFloorChangeWhenMoving;

        /// <summary>
        /// Should we check the number keys and use them to set target floor.
        /// </summary>
        [Tooltip ("Should we check the number keys and use them to set target floor.")]
        public bool checkNumKeysForFloor = false;

        /// <summary>
        /// 0 for not moving, 1 for up, -1 for down.
        /// </summary>
        protected int dir;

        /// <summary>
        /// Floor we are heading for.
        /// </summary>
        protected int targetFloor;

        /// <summary>
        /// Position of the floor we are heading for.
        /// </summary>
        protected float extent;

        /// <summary>
        /// Cached transform.
        /// </summary>
        protected Transform myTransform;

        /// <summary>
        /// Cached base position height.
        /// </summary>
        protected float basePosition;

        /// <summary>
        /// Gets the target floor.
        /// </summary>
        public int TargetFloor
        {
            get
            {
                return targetFloor;
            }
        }

        /// <summary>
        /// Occurs when a floor reached.
        /// </summary>
        public event System.EventHandler<System.EventArgs> FloorReached;

        /// <summary>
        /// Called when the floor reached.
        /// </summary>
        virtual protected void OnFloorReached()
        {
            if (FloorReached != null)
            {
                FloorReached(this, characterEventArgs);
            }
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        override protected void PostInit()
        {
            base.PostInit();

            if (transform.lossyScale != Vector3.one)
            {
                Debug.LogError("Moving platforms should have a scale of (1,1,1). " +
                               "If you wish to make them larger change the size of the collider and make the visual component a child of the platform.");
            }
            if (floorHeights.Length < 2)
            {
                Debug.LogError("An elevator platform needs at least two floors");
                gameObject.SetActive(false);
            }
            if (speed < 0) Debug.LogWarning("Elevator speed should be positive");
            myTransform = transform;
            basePosition = myTransform.position.y;

        }

        /// <summary>
        /// Do the move.
        /// </summary>
        override protected void DoMove()
        {
            if (!Activated) return;

            if (allowFloorChangeWhenMoving || dir ==  0)
            {
                foreach (Character c in PlatformerProGameManager.Instance.LoadedCharacters)
                {
                    CheckCharacterInput(c);
                }
            }

            if (dir > 0)
            {
                if (myTransform.position.y >= extent)
                {
                    OnFloorReached();
                    dir = 0;
                }
                else
                {
                    float distance = speed * TimeManager.FrameTime;
                    myTransform.Translate(0, distance, 0);
                    if (myTransform.position.y > extent)
                    {
                        float difference = distance - (myTransform.position.y - extent);
                        myTransform.position = new Vector3(myTransform.position.x, extent - difference, myTransform.position.z);
                        OnFloorReached();
                        dir = 0;
                    }
                }
            }
            else if (dir < 0)
            {
                if (myTransform.position.y <= extent)
                {
                    OnFloorReached();
                    dir = 0;
                }
                else
                {
                    float distance = -speed * TimeManager.FrameTime;
                    myTransform.Translate(0, distance, 0);
                    if (myTransform.position.y < extent)
                    {
                        float difference = distance - (myTransform.position.y - extent);
                        myTransform.position = new Vector3(myTransform.position.x, extent - difference, myTransform.position.z);
                        OnFloorReached();
                        dir = 0;
                    }
                }
            }
        }

        protected void CheckCharacterInput(Character c)
        {
            if (c == null || c.StoodOnPlatform != this) return;

            if (c.Input.VerticalAxisDigital == 1) MoveUpOneFloor();
            else if (c.Input.VerticalAxisDigital == -1) MoveDownOneFloor();
            else if (checkNumKeysForFloor)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad0))
                {
                    MoveToFloor(0);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad1))
                {
                    MoveToFloor(1);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad2))
                {
                    MoveToFloor(2);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad3))
                {
                    MoveToFloor(3);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad4))
                {
                    MoveToFloor(4);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad5))
                {
                    MoveToFloor(5);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad6))
                {
                    MoveToFloor(6);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad7))
                {
                    MoveToFloor(7);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad8))
                {
                    MoveToFloor(8);
                }
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha9) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad9))
                {
                    MoveToFloor(9);
                }
            }
        }
    
        /// <summary>
        /// Moves up one floor or not at all if at bottom.
        /// </summary>
        public void MoveUpOneFloor()
        {
            int newFloor = targetFloor + 1;
            if (newFloor >= floorHeights.Length) newFloor = floorHeights.Length;
            MoveToFloor(newFloor);
        }

        /// <summary>
        /// Moves down one floor or not all if at bottom.
        /// </summary>
        public void MoveDownOneFloor()
        {
            int newFloor = targetFloor - 1;
            if (newFloor < 0) newFloor = 0;
            MoveToFloor(newFloor);
        }

        public void MoveToFloor(int floor)
        {
            if (!Activated) return;
            if (!allowFloorChangeWhenMoving && dir != 0) return;

            if (floor != targetFloor && floor >= 0 && floor <= (floorHeights.Length))
            {
                targetFloor = floor;
                extent = basePosition + (targetFloor == 0 ? 0 : floorHeights[targetFloor - 1]);
                if (myTransform.position.y > extent)
                {
                    dir = -1;
                } 
                else if (myTransform.position.y < extent)
                {
                    dir = 1;
                }
                else
                {
                    Debug.LogWarning("Configuration issue in floor heights");
                }
            }
        }
    }
}
