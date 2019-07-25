using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BallBlaster
{

    [RequireComponent(typeof(Image))]
    public class UniversalInput : MonoBehaviour, IPlayerInput, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {

        private Camera _camera;

        private float _touchPositionX;
        private bool _isPressed;
        private bool _isPointerDown;
        private bool _isPointerUp;

        private void Awake()
        {
            _camera = Camera.main;

            if (!gameObject.CompareTag(Tags.Input))
                Debug.LogError("DesktopInput: has to be tagged as Input.");

            GetComponent<Image>().raycastTarget = true;
        }

        #region Interface_Implementation

        public bool IsPointerDown()
        {
            if (_isPointerDown)
            {
                _isPointerDown = false;
                return true;
            }

            return false;
        }

        public bool IsPointerUp()
        {
            if (_isPointerUp)
            {
                _isPointerUp = false;
                return true;
            }

            return false;
        }

        public float GetTouchPointX()
        {
            return _touchPositionX;
        }

        public bool IsShootPressed()
        {
            return _isPressed;
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdatePositionX();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
            _isPointerUp = true;

            UpdatePositionX();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            _isPointerDown = true;

            UpdatePositionX();
        }

        #endregion

        private void UpdatePositionX()
        {
            _touchPositionX = _camera.ScreenToWorldPoint(Input.mousePosition).x;
        }

    }
}