using System;
using NUnit.Framework;
using UnityEngine;
using NSubstitute;

namespace unityTest_project
{
    [TestFixture]
    public class SampleUnityClassTest
    {
        [Test]
        public void testMonoBehaviourSubclass()
        {
            // test gaming logic via SampleUnityClassController
            var audioController = Substitute.For<IAudioController> ();
            var sampleController = Substitute.For<SampleUnityClassController> ();
            sampleController.SetAudioController(audioController);

            // when the controller calls ApplyAudio method,
            // the actual function (PlaySound) that commucates with Unity API should have received a function call
            sampleController.ApplyAudio();
            audioController.Received(1).PlaySound();
        }
    }
}
