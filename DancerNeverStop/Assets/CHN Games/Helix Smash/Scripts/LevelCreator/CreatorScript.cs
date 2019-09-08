using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorScript : MonoBehaviour
{
    [System.Serializable]
    public class StepPattern
    {
        public string StepPatternName;
        public int count;
        public float height;
        public GameObject PlatformSample;
        public GameObject ObstacleSample; 
    }
    [System.Serializable]
    public class Step
    {
        public int StepPatternId;
        public Color StepColor;
        public float StepSizeX=1,StepSizeZ=1;
        [Range(0,360f)]
        public float StepOffsetAngle; 
    }
    [System.Serializable]
    public class AutoGeneratorColorPair
    {
        public Gradient normalPlatformGradientColor, obstaclePlatformGradientColor;
    }
    public List<StepPattern> StepPatterns;
    public List<AutoGeneratorColorPair> AutoGeneratorColorPairs;
    public List<AnimationCurve> AutoGenerateScales;
    public AnimationCurve autoGeneratorHardnessCurve;
    public int addCount, selectedStepPatternIndex,levelIndex;
    public float speedToAngle, speedTrapOccurenceDecreaser,trapOccurenceDecreaser;
    public bool addingSteps, stepModifiers,autoGenerator;
    public bool[] obstacleOrNotToggles = new bool[1];
    public Gradient addStepsNormalColorGradient = new Gradient(), addStepsObstacleColorGradient = new Gradient();
    public AnimationCurve addStepsScaleXCurve = AnimationCurve.Constant(0, 1, 1), addStepsScaleZCurve = AnimationCurve.Constant(0, 1, 1), addStepsAngleCurve = AnimationCurve.Constant(0, 1, 0), addStepsSpeedCurve = AnimationCurve.Constant(0, 1, 0);
    public Gradient modifierStepsNormalColorGradient = new Gradient(), modifierStepsObstacleColorGradient = new Gradient();
    public AnimationCurve modifierStepsScaleXCurve = AnimationCurve.Constant(0, 1, 1), modifierStepsScaleZCurve = AnimationCurve.Constant(0, 1, 1), modifierStepsAngleCurve = AnimationCurve.Constant(0, 1, 0), modifierStepsSpeedCurve = AnimationCurve.Constant(0, 1, 0);
    public int StepIndexMin, StepIndexMax;
}