using UnityEngine;
using System.Collections;
using System;

namespace unityTest_project
{
    public interface IAudioController
    {
        void PlaySound();
    }
    [RequireComponent(typeof(AudioSource))]
    public class SampleUnityClass : MonoBehaviour, IAudioController
    {
        public SampleUnityClassController controller;
        private void OnEnable()
        {
            controller.SetAudioController(this);
        }

        void Start()
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
            audio.Play(44100);
        }

        // encapsulate the code dependent on MonoBehaviour or Unity API
        #region IAudioController implementation
        public void PlaySound()
        {
            Debug.Log("This is amazing. (Dummy func for test purpose)");
        }
        #endregion
    }

    // Unity prohibites instantiating a MonoBehaviour with the new operator,
    // which makes it impossible to use any mocking frameworks out there.
    // SampleUnityClassController serves as a wrapper class to make MonoBehaviour and its subclasses unit testable
    // it implements the main gaming logic and keeps a reference to the MonoBehaviour class through its interface (IAudioController)
    // The gaming logic can be extracted from MonoBehaviour class and tested via SampleUnityClassController
    // the dependency on the non-mockable MonoBehaviour class can be decoupled via the interface (IAudioController) it inherits from
    // IAudioController can be mocked using NSubstitute framework
    [Serializable]
    public class SampleUnityClassController
    {
        private IAudioController audioController;

        public void SetAudioController(IAudioController controller)
        {
            this.audioController = controller;
        }

        public void ApplyAudio()
        {
            this.audioController.PlaySound();
        }
    }
}
