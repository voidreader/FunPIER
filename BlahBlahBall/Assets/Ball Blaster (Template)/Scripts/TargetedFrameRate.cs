using UnityEngine;

namespace BallBlaster
{

    public class TargetedFrameRate : MonoBehaviour
    {

        [SerializeField] private int _targetedFrameRate = 60;

        private void Start()
        {
            Application.targetFrameRate = _targetedFrameRate;
        }

    }

}