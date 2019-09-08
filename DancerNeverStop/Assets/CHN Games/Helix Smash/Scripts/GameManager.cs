using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
public class GameManager : MonoBehaviour
{
    //Start of Variables which are shown in Inspector
    [Header("------------------------------------Player Settings------------------------------------")]
    public AnimationCurve PlayerJumpCurve;
    public AnimationCurve PlayerBounceScaleXCurve, PlayerBounceScaleYCurve;
    public AnimationCurve overrideMultiplierControlCurve;
    public float PlayerAnimationSpeed;
    public float playerPositionOnZAxis;
    public Vector3 playerScaleWhenItIsBreakingPlatforms;
    [Header("-------------------------------Step Settings-------------------------------")]
    public int stepGenerationCountOnInitializing;
    public float stepBreakingSoundEffectPitchMultiplier;
    public float stepDestroyTime;
    public float stepYAxisForceMultiplier;
    public float stepXAxisForceMultiplier;
    public float stepXAxisForceBreakingTimeMultiplier;
    public float stepTorqueMultiplier;
    [Header("------------------------------------Star Settings------------------------------------")]
    public Vector3 StarGenerationSize;
    public Vector3 StarGenerationPoint;
    public GameObject StarSample;
    public AnimationCurve StarVisibilityCurve;
    public float starTimeInvisibilityMultiplier;
    public float normalStarSpeed, whenPlayerMeteorStarSpeed;
    public int minStarCount, maxStarCount;
    public float starAlphaTransparentMin, starAlphaTransparentMax;
    public float starSizeMin, starSizeMax;
    [Header("------------------------------------Meteor Settings------------------------------------")]
    public float MeteorGenerateTimeCounterRandomMax;
    public float MeteorDestroyTime;
    public float MeteorXAxisVelocityMin, MeteorXAxisVelocityMax, MeteorYAxisVelocityMin, MeteorYAxisVelocityMax;
    [Header("-------------------------------Instantiate Object Samples-------------------------------")]
    public GameObject PlayerSample;
    public GameObject CrackedPlayerSample;
    public GameObject MeteorSample, CenterCylinderSample;
    public GameObject endingPlatformSample;
    [Header("------------------------------------Sound Effects------------------------------------")]
    public AudioClip breakSoundClip;
    public AudioClip bounceSoundClip, successSoundClip, meteorBreakSoundClip, playerBreakSoundClip;
    [Header("------------------------------------Camera Settings------------------------------------")]
    public Transform CameraOrigin;
    public Vector3 CameraGameStartingPosition;
    public Transform Camera;
    public Light sceneLight;
    public float CameraShakeMultiplier;
    public float CameraRotateDegreeWhenPlayerIsMeteor;
    [Header("------------------------------------Background Settings------------------------------------")]
    public AnimationCurve sceneLightCurve;
    public Gradient backgroundBottomColorTransient, backgroundTopColorTransient;
    public BackgroundColorPair[] BackgroundRandomColorPairs;
    public Color overrideBottom, overrideTop;
    [Header("-------------------------------Breaking Bonus Settings-------------------------------")]
    public Image breakBonus;
    public float breakingBonusMax;
    public GameObject breakBonusWarning;
    public Gradient breakBonusColor;
    public float breakingBonusIncreasingSpeed, breakingBonusDecreasingSpeed, breakingBonusDecreasingSpeedWhenMeteor;
    [Header("-------------------------------Splash Decay Settings-------------------------------")]
    public Sprite[] splashSpriteSamples;
    public float splashSpriteScale;
    [Header("----------------------------------Score Settings----------------------------------")]
    public Text scoreValueText;
    public Gradient scoreTextColorGradient;
    public float scoreIncreaseAmount, scoreIncreaseAmountWhenPlayerIsMeteor, scoreScaleWhenMeteorModeActive;
    [Header("-------------------------------Level Indicator Settings-------------------------------")]
    public Slider gameStateSlider;
    public Text levelIndicatorSource, levelIndicatorTarget;
    [Header("-------------------------------Game End UI Settings-------------------------------")]
    public GameEndUI gameEndUI;
    [Header("-------------------------------Second Chance Settings-------------------------------")]
    public bool isSecondChanceActive;
    public int secondChanceTimeCounterMax, secondChanceLimit;
    [Header("-------------------------------Social Media Settings-------------------------------")]
    public List<SocialMediaPair> socialMediaPairs;
    [Header("-------------------------------Live Auto Generator Settings-------------------------------")]
    public bool isLiveAutoGeneratorActive;
    public int autoGeneratorSeed;
    public AnimationCurve autoGeneratorHardnessCurve;
    public List<CreatorScript.AutoGeneratorColorPair> AutoGeneratorColorPairs;
    public List<AnimationCurve> AutoGenerateScales;
    public float speedToAngle, speedTrapOccurenceDecreaser, trapOccurenceDecreaser;
    public int maxHardnessLevelNumber;
    public AnimationCurve totalStepMinCurve, totalStepMaxCurve;
    [Header("-------------------------------Other Settings-------------------------------")]
    public GameObject GameStartUI;
    public Button VibrationButton,VibrationButtonGameEndUI;
    public bool isToleranceEnabled;
    public float hittingToleranceAmount;
    public AudioSource musicAudioSource;
    public Button MusicButton, MusicButtonGameEndUI;
    public Sprite musicOnSprite, musicOffSprite;
    //Start of Variables which are not shown in Inspector
    [HideInInspector]
    public Step[] platformSamples;
    List<StepReferenceToGame> generatedLevelSteps = new List<StepReferenceToGame>();
    Queue<StepGeneration> stepGenerationQueue = new Queue<StepGeneration>();
    Transform Player, CrackedPlayer;
    Transform GeneratedObjects, GarbageObjects;
    float playerTimeCounter, bonusBreakingTime = 0;
    bool isBreakingBonusConsuming;
    Transform StarParent;
    Transform CenterCylinder;
    float meteorTimeCounter, meteorRandomValue, starVisibilityTime;
    float levelEnding;
    bool isNotFirstMovingSteps;
    bool isEndingPlatformGenerated;
    bool isGameEnded;
    Vector3 cameraShakeAmount;
    Coroutine secondChanceCoroutine;
    enum EndingTypes { NONE, COMPLETE, PASSEDALLLEVELS, GAMEOVER };
    EndingTypes endingType = EndingTypes.NONE;
    int currentLevel;
    AudioSource audioSource;
    float scoreValue;
    int secondChanceLimitCounter;
    bool creditOpenedBlocker;
    bool isGameStartMenuDisabled;
    bool isPlayerTouching;
    Transform toleratedStep;
    bool isToleranceEffectActivated;
    bool isVibrationEnabled;
    bool isMusicEnabled;
    void Start()
    {
        Application.targetFrameRate = 60; // This line locks the FPS to 60. 
        if (!PlayerPrefs.HasKey("total_score")) // Section 1 - Comment: If total_score and passed_levels keys are not exist generate and save with values 0 and 1 respectively.
        {
            PlayerPrefs.SetInt("total_score", 0);
        }
        else
        {
            gameEndUI.totalScoreValue.text = PlayerPrefs.GetInt("total_score").ToString();
        }
        if (!PlayerPrefs.HasKey("passed_levels"))
        {
            PlayerPrefs.SetInt("passed_levels", 1);
        }
        PlayerPrefs.Save();// End of  Section 1
        ShuffleStars(); // Call ShuffleStars to generate stars
        LoadLevel((SceneManager.sceneCount == 1) ? PlayerPrefs.GetInt("passed_levels") : -1); // When LevelCreator scene is opened, the scene count becomes 2, so if scene count is 1, that means the gamescene is opened and load the level, else LoadLevel(-1) => That means, the LevelCreatorScene is opened and don't load level normally.
        meteorRandomValue = Random.Range(0, MeteorGenerateTimeCounterRandomMax); // Pick a random value for meteorRandomValue, this is needed for starting of the time counter.
        audioSource = GetComponent<AudioSource>(); // Get audio source component of GameManager object, this is needed for playing sound effects.

        var vibrationStatus = PlayerPrefs.GetInt("vibration",1);
        if (vibrationStatus == 0)
        {
            SetVibration("1_0");
        }else
        {
            SetVibration("1_1");
        }
        var musicStatus = PlayerPrefs.GetInt("music", 1);
        if (musicStatus == 0)
        {
            SetMusicOption("1_0");
        }
        else
        {
            SetMusicOption("1_1");
        }
        foreach (SocialMediaPair pair in socialMediaPairs)
        {
            if (pair.link == "") continue;
            pair.button.onClick.AddListener(() => { Application.OpenURL(pair.link); });
        }
    }
    void Update()
    {
        MovingStars(); // Moves the stars
        StepUpdate(); // Call SteUpdate to moving and deleting steps.
        if (Player != null) PlayerUpdate(); // If there is a player, call PlayerUpdate to move player object and break platforms.
    }
    public void LoadLevel(int levelNumber)
    {
        scoreValue = 0; // Section 2 - Comment: Revert all changes to restart game properly.
        secondChanceLimitCounter = secondChanceLimit;
        scoreValueText.text = "0";
        bonusBreakingTime = 0;
        isBreakingBonusConsuming = false;
        gameStateSlider.value = gameStateSlider.minValue;
        currentLevel = levelNumber;
        endingType = EndingTypes.NONE;
        isGameEnded = false;
        isEndingPlatformGenerated = false;
        levelEnding = 0;
        Camera.localPosition = CameraGameStartingPosition;
        isToleranceEffectActivated = false;
        toleratedStep = null;
        generatedLevelSteps.Clear();
        stepGenerationQueue.Clear();
        if (isVibrationEnabled)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Vibration.Cancel();
#endif
        }
        // End of Section 2

        if (Player != null) Destroy(Player.gameObject); // Section 3 - Comment: Check object exists, if there is, destroy it or don't do anything and generate the new ones.
        Player = Instantiate<GameObject>(PlayerSample).transform;

        if (CrackedPlayer != null) Destroy(CrackedPlayer.gameObject);
        CrackedPlayer = null;

        if (CenterCylinder != null) Destroy(CenterCylinder.gameObject);
        CenterCylinder = Instantiate<GameObject>(CenterCylinderSample).transform;

        if (GarbageObjects != null) Destroy(GarbageObjects.gameObject);
        GarbageObjects = new GameObject("GarbageObjects").transform; // End of Section 3

        if (levelNumber != -1) // Section 4 - Comment: If levelNumber isn't -1, that means, the GameScene is opened, and do Section 3.
        {
            if (GeneratedObjects != null) Destroy(GeneratedObjects.gameObject);
            GeneratedObjects = new GameObject("GeneratedObjects").transform;
        } // End of Section 4

        var backgroundColorPair = BackgroundRandomColorPairs[Random.Range(0, BackgroundRandomColorPairs.Length)]; // Section 5 - Comment: Pick a background color pair randomly, get CameraConrol component of Camera and set it's bottom and top colors to background color pair's.
        var cameraControl = Camera.GetComponent<CameraControl>();
        cameraControl.bottom = backgroundColorPair.BackgroundColorBottom;
        cameraControl.top = backgroundColorPair.BackgroundColorTop; // End of Section 5

        if (levelNumber == -1) // Means the LevelCreatorScene is opened
        {
            GeneratedObjects = GameObject.Find("GeneratedObjects").transform; // GeneratedObjects and assign it.
            for (int i = 0; i < GeneratedObjects.childCount; i++) // Section 6 - Comment: Read stepIndexes of each steps under generatedObjects and add step to generatedLevelSteps. stepIndex[1] => Rotation Speed, stepIndex[2] => StepIndex
            {
                var step = GeneratedObjects.GetChild(i);
                var stepIndexes = step.name.Split('_');
                generatedLevelSteps.Add(new StepReferenceToGame(float.Parse(stepIndexes[1], CultureInfo.InvariantCulture), int.Parse(stepIndexes[2]), step));
                levelEnding += platformSamples[int.Parse(stepIndexes[2])].height; // Accumulate the levelEnding by summing step heights.

                if (i==0)
                {
                    for (int j=0;j < step.childCount;j++)
                    {
                        var mesh = step.GetChild(j).GetChild(0);
                        var rig = mesh.gameObject.AddComponent<Rigidbody>();
                        rig.useGravity = false;
                        rig.constraints = RigidbodyConstraints.FreezeAll;
                        var col = mesh.gameObject.GetComponent<MeshCollider>();
                        col.enabled = true;
                    }
                }

            } // End of Section 6
            var endingPlatform = Instantiate<GameObject>(endingPlatformSample, GeneratedObjects).transform; // Section 7 - Comment: Instantiate endingPlatform directly, because LevelCreatorScene is opened, and set its position. Also, add it to generatedLevelSteps by copying RotationSpeed of previous step. -1 StepIndex means, this step is a endingPlatform
            endingPlatform.position = new Vector3(0, generatedLevelSteps[generatedLevelSteps.Count - 1].step.position.y - 0.8f, 0);
            generatedLevelSteps.Add(new StepReferenceToGame(generatedLevelSteps[generatedLevelSteps.Count - 1].rotationSpeed, -1, endingPlatform)); // End of Section 7
            return; // The remaining of the method isn't needed, because this scene is LevelCreator scene.
        }

        var levelObject = Resources.Load<TextAsset>("Levels/" + levelNumber); // Get level text from resources in assets.
        if (levelObject == null) // Section 8 - Comment: If the level doesn't exist, log error message and revert this method.
        {
            if(isLiveAutoGeneratorActive) TriggerLiveAutoGenerator();
            else Debug.LogError("The level that will be loaded is not exist");
            return;
        } // End of Section 8
        var levelContent = levelObject.text; // Read the level text file. 
        levelObject = null; // Release levelObject file. This is for releasing unnecessary memory on RAM.
        var stepsByOrderAsStrings = levelContent.Split('\n'); // Return line contents as array of level content. Each line means one step.

        // Level file content structure => speed / ang / index  /      color      /     color2      / scaleX / scaleZ /   00000
        //                                 float  float  int        int_int_int       int_int_int     float    float   int sequence

        for (int j = 0; j < stepsByOrderAsStrings.Length; j++) // For loop does this for each step.
        {
            var stepsStrings = stepsByOrderAsStrings[j].Split('/'); // Gets step contents as string array (speed, ang, index, etc.)
            float stepRotationSpeed = float.Parse(stepsStrings[0], CultureInfo.InvariantCulture); // Parse string to float. 0 index is speed.
            float stepAngle = float.Parse(stepsStrings[1], CultureInfo.InvariantCulture); // Parse string to float. 1 index is offset angle relative to upper step.
            int stepId = int.Parse(stepsStrings[2]); // Parse string to int. 2 index is step index.
            var stepColorStrings = stepsStrings[3].Split('_'); // Split string to string array by '_' character. This returns color by r,g and b as string array.
            Color stepColor = new Color(float.Parse(stepColorStrings[0], CultureInfo.InvariantCulture) / 255f, float.Parse(stepColorStrings[1], CultureInfo.InvariantCulture) / 255f, float.Parse(stepColorStrings[2], CultureInfo.InvariantCulture) / 255f); // Parsing 0 1 2 indexes to float. This returns each color component in 0-255 range, then divides it with 255 to make it in range 0f-1f.
            var obstacleColorStrings = stepsStrings[4].Split('_'); // Split string to string array by '_' character. This returns color by r,g and b as string array.
            Color obstacleColor = new Color(float.Parse(obstacleColorStrings[0], CultureInfo.InvariantCulture) / 255f, float.Parse(obstacleColorStrings[1], CultureInfo.InvariantCulture) / 255f, float.Parse(obstacleColorStrings[2], CultureInfo.InvariantCulture) / 255f); // Parsing 0 1 2 indexes to float. This returns each color component in 0-255 range, then divides it with 255 to make it in range 0f-1f.
            float scaleX = float.Parse(stepsStrings[5], CultureInfo.InvariantCulture); // Parse string to float. 5 index is scaleX of the step.
            float scaleZ = float.Parse(stepsStrings[6], CultureInfo.InvariantCulture); // Parse string to float. 6 index is scaleZ of the step.
            bool[] isObstacleArray = new bool[stepsStrings[7].Length]; // Create a bool array that's length is equal to int sequence (like 00000 => length is 5) 0 means => normal platform, 1 means => obstacle platform.
            for (int i = 0; i < stepsStrings[7].Length; i++) // Section 9 - Comment: If char is '1' of int sequence, it is an obstacle platform and isObstacleArray[i] value becomes true. For loop does this for each char of the int sequence.
            {
                isObstacleArray[i] = stepsStrings[7][i] == '1';
            }// End of Section 9
            levelEnding += platformSamples[stepId].height; // Accumulate level ending to calculate level height. This is needed for placing ending platform.
            stepGenerationQueue.Enqueue(new StepGeneration(stepRotationSpeed, stepId, stepAngle, stepColor, obstacleColor, scaleX, scaleZ, isObstacleArray)); // Add this step to stepGenerationQueue with using datas. stepGenerationQueue is used for generating steps on time. If all steps are generated immediately, the game will be slow on mobile phones, even on computers.
        }
        levelIndicatorSource.text = levelNumber.ToString(); // Set levelIndicatorSource text to level number. levelIndicatorSource is left side of the progressing indicator on the top of screen. 
        levelIndicatorTarget.text = Resources.Load<TextAsset>("Levels/" + (levelNumber + 1)) != null || isLiveAutoGeneratorActive ? (levelNumber + 1).ToString() : "END"; // Set levelIndicatorTarget text to level number+1. If there is no level with level number+1, set text to "END".
        GenerateStepsAsBulk(stepGenerationCountOnInitializing); // Generate steps as bulk
    }
    void GenerateStepsAsBulk(int count) // Generates steps as bulk
    {
        float stepYOffset = 0; // Position offset on the y axis.
        for (int i = 0; i < count; i++) // For loop does this for each step
        {
            StepGeneration stepInfo = stepGenerationQueue.Dequeue(); // Get next step from stepGenerationQueue. I used queue for storing steps, because steps can be only adding with 0 index and remove with last index.
            var generatedStep = new GameObject("Step").transform; // Generate the step.
            generatedStep.SetParent(GeneratedObjects); // Set parent of new step as GeneratedObjects. With this way all steps will under the GeneratedObjects gameobject.
            generatedStep.position = new Vector3(0, stepYOffset, 0); // Set step position.
            float lastStepAngle = generatedLevelSteps.Count > 0 ? generatedLevelSteps[generatedLevelSteps.Count - 1].step.eulerAngles.y : 0; // Get previous step angle to calculate relative rotate angle. If the index is equal to 0, that means, this step is first step of the level and relative angle must be 0.
            generatedStep.eulerAngles = new Vector3(0, lastStepAngle + stepInfo.stepAngle, 0); //  Set rotation of the step by calculating relative rotate angle.
            generatedLevelSteps.Add(new StepReferenceToGame(stepInfo.rotationSpeed, stepInfo.stepIndex, generatedStep)); // Add this step to generatedLevelSteps. generatedLevelSteps is used for rotating and deleting steps with StepUpdate method.
            Step step = platformSamples[stepInfo.stepIndex]; // This is used for getting obstaclePlatformSample, normalPlatformSample, count and height informations.
            for (int j = 0; j < stepInfo.isObstacleArray.Length; j++) // For loop does this for each platform
            {
                var generatedPlatform = Instantiate<GameObject>(stepInfo.isObstacleArray[j] ? step.obstaclePlatformSample : step.normalPlatformSample, generatedStep).transform; // Generate platform with getting the information from isObstacleArray[j].
                generatedPlatform.name = "Platform_" + (stepInfo.isObstacleArray[j] ? "1" : "0"); // Name the platform. I added _0 or _1 information end of the name. _0 means => normal platform, _1 means => obstacle platform. _ character is used for splitting two pieces easily with Split method of string.
                generatedPlatform.localEulerAngles = new Vector3(0, (360f / (float)step.count) * j, 0); // Set rotate of the platform. Rotate difference amount is 360/count.

                if (i == 0)
                {
                    var mesh = generatedPlatform.GetChild(0);
                    var rig = mesh.gameObject.AddComponent<Rigidbody>();
                    rig.useGravity = false;
                    rig.constraints = RigidbodyConstraints.FreezeAll;
                    var col = mesh.gameObject.GetComponent<MeshCollider>();
                    col.enabled = true;
                }

                if (generatedPlatform.GetChild(0).GetComponent<Renderer>().material.shader.name == "Custom/PlatformShader") generatedPlatform.GetChild(0).GetComponent<Renderer>().material.color = stepInfo.isObstacleArray[j] ? stepInfo.obstacleColor : stepInfo.stepColor; // If platform sample has Custom/PlatformShader, set its color to stepInfo.obstacleColor or stepInfo.stepColor. With this way, we can use textured platforms with other materials.
            }
            stepYOffset -= step.height; // Iterate the offset.
            generatedStep.localScale = new Vector3(stepInfo.scaleX, 1, stepInfo.scaleZ); // Set step size with information of stepInfo.
            if (stepGenerationQueue.Count == 0) break; // If there is no remaining steps in stepGenerationQueue, break loop. If count is greater than stepGenerationQueue count, this line happens.
        }
    }
    void GenerateStep() // Generates a step
    {
        if (stepGenerationQueue.Count == 0) return; // If there is no remaining steps in stepGenerationQueue, break loop.
        StepGeneration stepInfo = stepGenerationQueue.Dequeue(); // Get next step from stepGenerationQueue. I used queue for storing steps, because steps can be only adding with 0 index and remove with last index.
        var generatedStep = new GameObject("Step").transform; // Generate the step.
        generatedStep.SetParent(GeneratedObjects); // Set parent of new step as GeneratedObjects. With this way all steps will under the GeneratedObjects gameobject.
        generatedStep.position = new Vector3(0, generatedLevelSteps[generatedLevelSteps.Count - 1].step.position.y - platformSamples[stepInfo.stepIndex].height, 0); // Set step position. The position of this step is relative to previous step.
        float lastStepAngle = generatedLevelSteps[generatedLevelSteps.Count - 1].step.eulerAngles.y; // Previous step rotation angle. This will be used for calculating relative rotation angle.
        generatedStep.eulerAngles = new Vector3(0, lastStepAngle + stepInfo.stepAngle, 0); // Set rotation of the step by calculating relative rotate angle.
        generatedLevelSteps.Add(new StepReferenceToGame(stepInfo.rotationSpeed, stepInfo.stepIndex, generatedStep)); // Add this step to generatedLevelSteps. generatedLevelSteps is used for rotating and deleting steps with StepUpdate method.
        for (int j = 0; j < stepInfo.isObstacleArray.Length; j++) // For loop does this for each platform
        {
            Step step = platformSamples[stepInfo.stepIndex]; // This is used for getting obstaclePlatformSample, normalPlatformSample, count informations.
            var generatedPlatform = Instantiate<GameObject>(stepInfo.isObstacleArray[j] ? step.obstaclePlatformSample : step.normalPlatformSample, generatedStep).transform; // Generate platform with getting the information from isObstacleArray[j].
            generatedPlatform.name = "Platform_" + (stepInfo.isObstacleArray[j] ? "1" : "0"); // Name the platform. I added _0 or _1 information end of the name. _0 means => normal platform, _1 means => obstacle platform. _ character is used for splitting two pieces easily with Split method of string.
            generatedPlatform.localEulerAngles = new Vector3(0, (360f / (float)step.count) * j, 0); // Set rotate of the platform. Rotate difference amount is 360/count.
            if (generatedPlatform.GetChild(0).GetComponent<Renderer>().material.shader.name == "Custom/PlatformShader") generatedPlatform.GetChild(0).GetComponent<Renderer>().material.color = stepInfo.isObstacleArray[j] ? stepInfo.obstacleColor : stepInfo.stepColor; // If platform sample has Custom/PlatformShader, set its color to stepInfo.obstacleColor or stepInfo.stepColor. With this way, we can use textured platforms with other materials.
        }
        generatedStep.localScale = new Vector3(stepInfo.scaleX, 1, stepInfo.scaleZ); // Set step size with information of stepInfo.
        if (stepGenerationQueue.Count == 0) // If there is no remaining steps instantiate ending platform.
        {
            var endingPlatform = Instantiate<GameObject>(endingPlatformSample, GeneratedObjects).transform; // Instantiate ending platform with parent of GeneratedObjects. 
            endingPlatform.position = new Vector3(0, generatedStep.position.y - 0.8f, 0); // Set position of the ending platform. Ending platform half height is 0.8.
            generatedLevelSteps.Add(new StepReferenceToGame(stepInfo.rotationSpeed, -1, endingPlatform)); // Add ending platform to generatedLevelSteps. -1 means, indicating this is an ending platform. Also, copies rotation speed of previous step for the ending platform.
            isEndingPlatformGenerated = true; // Indicates ending platform is generated.
        }
    }
    public void StepUpdate() // Checks steps and rotates
    {
        for (int i = 0; i < generatedLevelSteps.Count; i++) // For loop does this for each generated step in the game scene.
        {
            if (generatedLevelSteps[i] == null || generatedLevelSteps[i].step.parent != GeneratedObjects) // This condition means if selected step is destroyed or its parent isn't GeneratedObjects.
            {
                generatedLevelSteps.RemoveAt(i); // Just remove step reference from generatedLevelSteps list array.
                i--; // This is needed, because when 1 item is deleted from list, the all indexes after "i" index are decreases by 1.
                continue; // Don't do under codes of this and continue.
            }
            generatedLevelSteps[i].step.eulerAngles += new Vector3(0, generatedLevelSteps[i].rotationSpeed, 0); // Iterate angle of rotation of each step by the rotationSpeed that comes from generatedLevelSteps information.
        }
    }
    public void PlayerUpdate()
    {
        float normalizedBreakingBonus = bonusBreakingTime / breakingBonusMax; // Normalizes bonusBreakingTime to 0-1 range.
        if (!isGameEnded && generatedLevelSteps.Count == 1) // This if block is only for making game end more precise. Otherwise some errors may encounter.
        {
            audioSource.pitch = 1; // Set pitch to 1, because the pitch of the audioSource may be modified.
            audioSource.PlayOneShot(successSoundClip); // Play success sound clip. Because game is ended with complete or passed all levels action, both are success.
            if (levelIndicatorTarget.text != "END") // If right level indicator's text is NOT "END", that means there is more levels after this level.
            {
                endingType = EndingTypes.COMPLETE; // Set ending action.
                GameEnd(); // Call game end to end. (like showing end UI)
            }
            else // If right level indicator's text is "END", that means there is no more levels after this level.
            {
                endingType = EndingTypes.PASSEDALLLEVELS; // The player passed all levels, so ending action is passed all levels.
                GameEnd(); // Call game end to end. (like showing end UI)
            }
        }
        if (!isGameStartMenuDisabled)
        {
            NotControledPlayerByTouching(normalizedBreakingBonus);
            if (isPlayerTouching)
            {
                isGameStartMenuDisabled = true;
                GameStartUI.SetActive(false);
                gameStateSlider.transform.parent.gameObject.SetActive(true);
                scoreValueText.gameObject.SetActive(true);
            }
            return;
        }
        if (isGameEnded || !Input.GetMouseButton(0) || isToleranceEffectActivated) // If game is ended or the player does not touch to screen, do this block.
        {
            NotControledPlayerByTouching(normalizedBreakingBonus); // Call NotControledPlayerByTouching (does free ball bouncing etc.)
        }
        else
        {
            ControledPlayerByTouching(normalizedBreakingBonus); // Call ControledPlayerByTouching (does ball breaking action etc.)
        }
    }
    public void PlayerClickDown()
    {
        isPlayerTouching = true;
    }
    public void PlayerClickUp()
    {
        isPlayerTouching = false;
    }
    public void NotControledPlayerByTouching(float normalizedBreakingBonus) // Control free ball bouncing, splash effect and some animations.
    {
        if (playerTimeCounter == -1) // If playerTimeCounter = -1, that means the player has just released the screen.
        {
            for (float i = 1f; i > 0f; i -= 0.02f) // For loop checks PlayerJumpCurve inversely.
            {
                if (((Player.transform.position.y - 0.20f) - PlayerJumpCurve.Evaluate(i)) < 0.01f) // If distance between player position and PlayerJumpCurve value is lower than 0.01, do this block.
                {
                    playerTimeCounter = i; // The correct location of the player position on the PlayerJumpCurve has found and set the time counter.
                    //GeneratedObjects.position = new Vector3(0, -GeneratedObjects.GetChild(0).localPosition.y, 0); // All steps' position must be set to 0, if the player does not touch the screen. So, assigning first steps' negative Y axis local position to GeneratedObjects Y axis position sets all steps' positions and by this way, first step's global position is set to 0 on Y axis.
                    Player.GetChild(1).GetComponent<ParticleSystem>().Stop(); // Stop the meteor effect. It may emit.
                    cameraShakeAmount = Vector3.zero; // Reset camera shake amount vector for reusing properly.
                    break; // The correct position is found and the loop must be broken.
                }
            }
        }
        if (toleratedStep != null && !isToleranceEffectActivated)
        {
            isToleranceEffectActivated = true;
            StartCoroutine(ToleranceEffect());
        }
        GeneratedObjects.position = new Vector3(0, Mathf.MoveTowards(GeneratedObjects.position.y, -GeneratedObjects.GetChild(0).localPosition.y, 0.2f), 0); // All steps' position must be set to 0, if the player does not touch the screen. So, assigning first steps' negative Y axis local position to GeneratedObjects Y axis position sets all steps' positions and by this way, first step's global position is set to 0 on Y axis.
        Player.GetChild(0).GetComponent<LineRenderer>().enabled = false;
        Player.transform.position = new Vector3(0, PlayerJumpCurve.Evaluate(playerTimeCounter) + 0.20f, playerPositionOnZAxis); // Simple bouncing animation controlled by the curve.
        Player.transform.localScale = new Vector3(Mathf.MoveTowards(Player.transform.localScale.x, PlayerBounceScaleXCurve.Evaluate(playerTimeCounter), 0.05f), PlayerBounceScaleYCurve.Evaluate(playerTimeCounter), 1); // Simple scale animation controlled by the curves. For x scale I used MoveTowards method, because when the player touches the screen, ball expands too much on the X axis, if this method is not used, the scale animation on X axis would be discrete.
        Camera.localRotation = Quaternion.Slerp(Camera.localRotation, Quaternion.Euler(25f, 0f, 0f), 0.3f); // Reset camera rotation. Quaternion is used, because euler angle method has gimbal lock.
        playerTimeCounter += Time.deltaTime * PlayerAnimationSpeed; // Increase time counter.
        if (playerTimeCounter > 1f) // If time counter is greater than 1, that means the animation curve ended and it is needed to reset. I did not use PingPong effect of animation curve, because to make splash and bounce sound effects, a single time operation must be launched and this method achieves that easily.
        {
            playerTimeCounter = 0f; // Reset counter.
            audioSource.pitch = 1f; // Set pitch to 1. It may be modified.
            audioSource.PlayOneShot(bounceSoundClip); // Play bounce sound effect, because jump curve has 0 value at 0 time, and this is starting of the bounce.
            var splash = new GameObject("splash").transform; // Generate a game object to use as splash parent.
            splash.parent = generatedLevelSteps.Count != 1 ? generatedLevelSteps[0].step.GetChild(0).GetChild(0) : generatedLevelSteps[0].step; // If the first step is normal step, the parent is generatedLevelSteps[0].step.GetChild(0).GetChild(0) (This location is under of the platform mesh object). If the generatedLevelSteps = 1, that means this step is an ending platform and the parent is needed to assign differently. The parent must be generatedLevelSteps[0].step.
            splash.SetAsLastSibling(); // Set the sibling index as last, because other siblings can be used by other codes by using child indexing.
            var splashSprite = splash.gameObject.AddComponent<SpriteRenderer>(); // Add sprite renderer and store it. Because the splash sprite can be renderer by sprite renderer.
            splashSprite.sprite = splashSpriteSamples[Random.Range(0, splashSpriteSamples.Length)]; // Pick a random splash sprite from splashSpriteSamples.
            splashSprite.color = Player.GetChild(0).GetComponent<Renderer>().material.color; // Set splash color to player's material color.
            splash.eulerAngles = new Vector3(90, 0, 0); // Splash effects must be facing up.
            splash.localScale = Vector3.one * splashSpriteScale; // Set scale of the splash sprite.
            splash.position = Player.position; // Set position to Player's position. This is initial position.
            splash.localPosition = new Vector3(splash.localPosition.x, generatedLevelSteps.Count != 1 ? generatedLevelSteps[0].step.GetChild(0).Find("splashOrigin").localPosition.y : generatedLevelSteps[0].step.Find("splashOrigin").localPosition.y, splash.localPosition.z); // Get splashOrigin object from platform or ending platform where it is located and set y axis of the splash sprite to splashOrigin y axis position.
            splash.gameObject.AddComponent<SplashDestroyer>(); // Add SplashDestroyer, this destroys splash sprite when it is rotated 1.5 complete turn. (540 degree)
#if UNITY_ANDROID && !UNITY_EDITOR
        if(isVibrationEnabled && !isToleranceEffectActivated)
        {
            Vibration.Vibrate(3);
        }
#endif
        }
        if (bonusBreakingTime > 0f)
            bonusBreakingTime -= Time.deltaTime * breakingBonusDecreasingSpeed; // If the player does not the screen and bonus breaking time greater than zero, decrease it.
        else
        {
            if (breakBonusWarning.activeSelf) breakBonusWarning.SetActive(false); // If breakBonusWarning is active, deactive it. This is needed because the ball may exit from meteor mode.
            bonusBreakingTime = 0f; // Set bonus breaking time to 0 for exact values.
            isBreakingBonusConsuming = false; // If the ball exited from meteor mode, isBreakingBonusConsuming must be false.
        }

        breakBonus.fillAmount = normalizedBreakingBonus; // Set break bonus indicator filling amount.
        sceneLight.intensity = sceneLightCurve.Evaluate(normalizedBreakingBonus); // Set scene light intensity by evaluating curve.
        breakBonus.color = breakBonusColor.Evaluate(normalizedBreakingBonus); // Set break bonus indicator color by evaluating gradient.
        Camera.GetComponent<CameraControl>().overrideMultiplier = normalizedBreakingBonus; // Override multiplier is equal to normalized breaking bonus value. It is needed to control amount of overriding meteor background color by using animation curves.
        overrideBottom = backgroundBottomColorTransient.Evaluate(normalizedBreakingBonus); // Meteor background bottom color
        overrideTop = backgroundTopColorTransient.Evaluate(normalizedBreakingBonus); // Meteor background top color
        isNotFirstMovingSteps = false; // It is used for increasing position precise of breaking effect. If the player does not touch the screen, it must be resetted.
        CameraOrigin.eulerAngles = new Vector3(Mathf.LerpAngle(CameraOrigin.eulerAngles.x, 0, 0.15f), 0, 0); // Reset camera origin rotation.
        scoreValueText.transform.localScale = Vector3.MoveTowards(scoreValueText.transform.localScale, Vector3.one, 0.15f); // Reset scoreValueText scale, it may be scaled in meteor mode.
        scoreValueText.color = scoreTextColorGradient.Evaluate(normalizedBreakingBonus); // Reset color of scoreValueText, it may be changed its color in meteor mode.
    }
    public void ControledPlayerByTouching(float normalizedBreakingBonus)
    {
        Player.transform.position = Vector3.MoveTowards(Player.transform.position, new Vector3(0, 0.2f, playerPositionOnZAxis), 0.25f); // The ball must be (0,0.2,-2.4) position to break platforms properly.
        Player.transform.localScale = Vector3.MoveTowards(Player.transform.localScale, playerScaleWhenItIsBreakingPlatforms, 0.1f); // Sets the ball scale.
        playerTimeCounter = -1; // This indicates the player touches to screen and the proper location must be found in NotControledPlayerByTouching method.
        if (Player.transform.position.y == 0.2f) // That means the ball started to break platforms.
        {
            Player.GetChild(0).GetComponent<LineRenderer>().enabled = true;
            if (!isBreakingBonusConsuming) // If the meteor mode is not activated.
            {
                scoreValueText.transform.localScale = Vector3.MoveTowards(scoreValueText.transform.localScale, Vector3.one, 0.1f); // scoreValueText scale is (1,1,1) when meteor mode is not activated.
                scoreValueText.color = scoreTextColorGradient.Evaluate(normalizedBreakingBonus); // Sets scoreValueText color by evaluating gradient.
                if (bonusBreakingTime < breakingBonusMax) bonusBreakingTime += Time.deltaTime * breakingBonusIncreasingSpeed; // Increase bonus breaking time counter.
                else // If bonusBreakingTime is greater than breakingBonusMax and the meteor mode is activated. The effects of meteor effect will be able to appear in next frame. (in else block)
                {
                    bonusBreakingTime = breakingBonusMax; // Sets bonusBreakingTime to breakingBonusMax, this is needed for exact values.
                    isBreakingBonusConsuming = true; // Meteor mode is activated.
                    breakBonusWarning.SetActive(true); // Warning indicator is activated and it has play on awake animation component so it will play fading in-out animation once it is activated.
                    Player.GetChild(1).GetComponent<ParticleSystem>().Play(); // Meteor effect is started.
                }
            }
            else // If the meteor mode is activated. Meteor effects can appear here.
            {
                AttemptToGenerateMeteor(); // Attemp to generate background meteor figures.
                CameraOrigin.eulerAngles = new Vector3(Mathf.LerpAngle(CameraOrigin.eulerAngles.x, CameraRotateDegreeWhenPlayerIsMeteor, 0.15f), 0, 0); // Set camera origin rotation.
                scoreValueText.transform.localScale = Vector3.MoveTowards(scoreValueText.transform.localScale, Vector3.one * scoreScaleWhenMeteorModeActive, 0.11f); // Set scoreValueText scale.

                if (cameraShakeAmount == Vector3.zero) // If camera shake amount is 0, this effect is for meteor mode camera shaking. The logic is; set camera rotation in one frame by randomly and recover camera the rotation in next frame.
                {
                    cameraShakeAmount = Random.insideUnitSphere * CameraShakeMultiplier; // Set shake amount randomly.
                    Camera.eulerAngles += cameraShakeAmount; // Apply shake amount.
                }
                else // If cameraShakeAmount is not (0,0,0), the camera is just shaked.
                {
                    Camera.eulerAngles -= cameraShakeAmount; // Recover camera rotation. 
                    cameraShakeAmount = Vector3.zero; // Reset the shake amount.
                }

                if (bonusBreakingTime > 0f) bonusBreakingTime -= Time.deltaTime * breakingBonusDecreasingSpeedWhenMeteor; // Decrease bonus breaking time, because the meteor mode consumes bonus breaking time.
                else // If bonusBreakingTime < 0
                {
                    bonusBreakingTime = 0; // Set bonus breaking time to 0 for exact values.
                    isBreakingBonusConsuming = false; // The meteor mode is deactivated.
                    breakBonusWarning.SetActive(false); // The meteor mode is deactivated.
                    Player.GetChild(1).GetComponent<ParticleSystem>().Stop(); // Stop meteor effect of the ball.
                }
            }
            breakBonus.fillAmount = normalizedBreakingBonus; // Set break bonus indicator filling amount.
            breakBonus.color = breakBonusColor.Evaluate(normalizedBreakingBonus); // Set break bonus indicator color by evalueating gradient.
            float offsetTime = (isBreakingBonusConsuming ? 0.6f : 0f); // If meteor mode is activated (isBreakingBonusConsuming = true), set 0.6 offset time.
            sceneLight.intensity = sceneLightCurve.Evaluate(normalizedBreakingBonus + offsetTime); // Set scene light by evalueating curve.
            Camera.GetComponent<CameraControl>().overrideMultiplier = normalizedBreakingBonus + offsetTime; // Set override multiplier value
            overrideBottom = backgroundBottomColorTransient.Evaluate(normalizedBreakingBonus + offsetTime); // Set background bottom color 
            overrideTop = backgroundTopColorTransient.Evaluate(normalizedBreakingBonus + offsetTime); // Set background top color 
            PlatformBreakAndChecking(); // Check and break platform if needed.
        }
    }
    void AttemptToGenerateMeteor() // Generates meteor if random time is elapsed.
    {
        if (meteorTimeCounter > meteorRandomValue) // If time counter greater than meteorRandomValue, generate meteor. 
        {
            meteorRandomValue = Random.Range(0f, MeteorGenerateTimeCounterRandomMax); // Set new random value.
            meteorTimeCounter = 0; // Reset time counter
            var t = Instantiate<GameObject>(MeteorSample, GarbageObjects).transform; // Generate meteor object and set its parent to GarbageObjects.
            t.position = new Vector3(Random.Range(3f, 5f) * (Random.value > 0.5f ? 1 : -1), 10f, Random.Range(5f, 7f) * (Random.value > 0.6f ? 1f : -1f)); // Set meteor position randomly.
            t.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(MeteorXAxisVelocityMin, MeteorXAxisVelocityMax) * (t.position.x < 0 ? 1 : -1), Random.Range(MeteorYAxisVelocityMin, MeteorYAxisVelocityMax), 0); // Set meteor velocity randomly.
            Destroy(t.gameObject, MeteorDestroyTime); // Destroy meteor after MeteorDestroyTime time.
        }
        meteorTimeCounter += Time.deltaTime; // Increase time counter.
    }
    void ShuffleStars() // Generates Star Parent and triggers generating stars. 
    {
        if (StarParent != null) // If star parent is not null
        {
            Destroy(StarParent.gameObject); // Destroy it
        }
        StarParent = new GameObject("BackgroundStars").transform; // Generate new star parent object.
        StarParent.position = StarGenerationPoint; // Set star parent position to StarGenerationPoint.
        for (int i = 0; i < Random.Range(minStarCount, maxStarCount); i++) // Generates stars with random amount.
        {
            GenerateAStar(true); // Generates a star randomly.
        }
    }
    void GenerateAStar(bool isFirstTime) // Generates one star. If isFirstTime is true, generates complete randomly on y axis, otherwise generates on Random.Range(-10,-5) y axis. This is needed for making stars moving continuously.
    {
        var star = Instantiate<GameObject>(StarSample, StarParent).transform; // Generate star object
        star.localScale = Random.Range(starSizeMin, starSizeMax) * Vector3.one; // Set scale of star randomly.
        var spriteRenderer = star.GetComponent<SpriteRenderer>(); // Get sprite renderer component.
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Random.Range(starAlphaTransparentMin, starAlphaTransparentMax)); // Set alpha of star sprite renderer randomly.
        star.localPosition = StarGenerationPoint + new Vector3(Random.Range(-StarGenerationSize.x / 2, StarGenerationSize.x / 2), isFirstTime ? Random.Range(-StarGenerationSize.y / 2, StarGenerationSize.y / 2) : Random.Range(-10, -5), Random.Range(-StarGenerationSize.z / 2, StarGenerationSize.z / 2)); // Set position of star randomly.
    }
    void MovingStars() //Moves the stars
    {
        for (int i = 0; i < StarParent.childCount; i++) // For loop does for each star.
        {
            var t = StarParent.GetChild(i); // Get star by i index.
            t.position += new Vector3(0, isBreakingBonusConsuming ? whenPlayerMeteorStarSpeed : normalStarSpeed, 0); // Moves up the stars.
            if (t.position.y > 10f) // If the start position on y axis is greater than 10.
            {
                Destroy(t.gameObject); // Destroy this star.
                GenerateAStar(false); // Generate a new star to replace with destroyed star.
            }
            t.LookAt(Camera); // Stars must look to camera to view perfectly circle.
        }
    }
    void PlatformBreakAndChecking() // Break platform and checks, also triggers break effect for steps.
    {
        var touchedStepsDictionary = Player.GetComponent<PlayerController>().touchingPlatforms; // Touched steps information that comes from PlayerController component on Player. This information is collected by using Physic Engine. 
        var touchedStepKeys = touchedStepsDictionary.Keys.ToArray<Transform>(); // touchedStepsDictionary is stored as Dictionary, so this line gets all keys as transform array.
        bool isObstacleHit = false; // This holds the Player is touching any obstacle platform. False is the initial value.
        for (int i = 0; i < touchedStepsDictionary.Count; i++) // For loop does this for each touch steps.
        {
            if (touchedStepKeys[i] == null) // If touched step key is null, that means the touched step is destroyed before. (The player can touch two or more platform of one step, this block prevents it)
            {
                touchedStepsDictionary.Remove(touchedStepKeys[i]); // Remove it
                i--; // When remove an item from dictionary, it's length is changed and after i index, the all indexes decreased by one.
                continue; // This platform must not concern to delete or not.
            }
            if (touchedStepKeys[i].tag == "WillBeDeletedStep") continue; // If a step marked as "WillBeDeletedStep", it will be destroyed and deleted next frame due touching of the player another platform of this step and skip this iteration with continue.
            touchedStepsDictionary.TryGetValue(touchedStepKeys[i], out isObstacleHit); // Gets value of key-value pair of touchedStepsDictionary. (Key is step transform, value is the platform is an obstacle or not)
            if (isToleranceEnabled && isObstacleHit && !isBreakingBonusConsuming && touchedStepKeys[i].position.y < hittingToleranceAmount)
            {
                toleratedStep = touchedStepKeys[i];
                isObstacleHit = false;
                isNotFirstMovingSteps = true;
                continue;
            }
            PlatformBreakEffect(touchedStepKeys[i]); // Triggers platform break effect for the step.
            touchedStepKeys[i].tag = "WillBeDeletedStep"; // Marks as WillBeDeletedStep the step.
            if (!isObstacleHit || isBreakingBonusConsuming)
            {
                scoreValue += isBreakingBonusConsuming ? scoreIncreaseAmountWhenPlayerIsMeteor : scoreIncreaseAmount; // Increase the score with scoreIncreaseAmountWhenPlayerIsMeteor or scoreIncreaseAmount. If the meteor mode active, breaking steps give more point logically.
                scoreValueText.text = ((int)scoreValue).ToString(); // Set scoreValueText text with updated scoreValue.
            }
            touchedStepsDictionary.Remove(touchedStepKeys[i]); // Remove the step from touchedStepsDictionary, because this step will be deleted soon.
            GenerateStep(); // One step is deleted from top and one step must be generated from bottom if there is more step in stepGenerationQueue queue.
            if (!isNotFirstMovingSteps) isNotFirstMovingSteps = true; // If a step is deleted, that means the all steps must be moving up.
        }
        if (isNotFirstMovingSteps) // Steps can move up and can play sound effects etc.
        {
            if (isObstacleHit && !isBreakingBonusConsuming) // Did the player hit any obstacle and is meteor mode deactivated?
            {
                audioSource.pitch = 0.6f; // Set pitch of the audioSource, this is for making sound effect deeper.
                audioSource.PlayOneShot(playerBreakSoundClip); // Play sound effect.
                endingType = EndingTypes.GAMEOVER; // The player hit to an obstacle and the game has finished.
                GameEnd(); // Call game end to end. (like showing end UI)
            }
            if (isEndingPlatformGenerated && generatedLevelSteps.Last<StepReferenceToGame>().step.position.y < -5f) Camera.position += new Vector3(0, 0.132f * 0.75f * 0.25f, 0);
            if (generatedLevelSteps.Count != 1) GeneratedObjects.position += new Vector3(0, 0.132f * 1.5f, 0);
            gameStateSlider.value = gameStateSlider.minValue + (GeneratedObjects.position.y / levelEnding) * (gameStateSlider.maxValue - gameStateSlider.minValue);
            if (!isEndingPlatformGenerated && CenterCylinder.transform.position.y < 0.5f) CenterCylinder.transform.position += new Vector3(0, 0.132f * 1.5f, 0);
            if (isEndingPlatformGenerated && generatedLevelSteps.Count < 25) CenterCylinder.position = new Vector3(0, CenterCylinder.localScale.y + generatedLevelSteps.Last<StepReferenceToGame>().step.position.y, 0);
            if (generatedLevelSteps.Count == 1) // generatedLevelSteps count is equal to 1, that means there is no normal step, there is only ending platform. End game finished with complete or passed all levels action.
            {
                audioSource.pitch = 1; // Set pitch to 1, because the pitch of the audioSource may be modified.
                audioSource.PlayOneShot(successSoundClip); // Play success sound clip. Because game is ended with complete or passed all levels action, both are success.
                if (levelIndicatorTarget.text != "END") // If right level indicator's text is NOT "END", that means there is more levels after this level.
                {
                    endingType = EndingTypes.COMPLETE; // Set ending action.
                    GameEnd(); // Call game end to end. (like showing end UI)
                }
                else // If right level indicator's text is "END", that means there is no more levels after this level.
                {
                    endingType = EndingTypes.PASSEDALLLEVELS; // The player passed all levels, so ending action is passed all levels.
                    GameEnd(); // Call game end to end. (like showing end UI)
                }
            }
        }
    }
    void PlatformBreakEffect(Transform step) // Breaks a step with effect.
    {
        if (isVibrationEnabled)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Vibration.Vibrate(15);
#endif
        }
        step.parent = GarbageObjects; // This step is a garbage for now, it is useless and it will be destroyed and deleted soon. So its parent is set as GarbageObjects.

        if (GeneratedObjects.GetChild(0) && GeneratedObjects.childCount > 1)
        {
            var newFirstStep = GeneratedObjects.GetChild(0);
            for (int i = 0; i < newFirstStep.childCount; i++)
            {
                var mesh = newFirstStep.GetChild(i).GetChild(0);
                var rig = mesh.gameObject.AddComponent<Rigidbody>();
                rig.useGravity = false;
                rig.constraints = RigidbodyConstraints.FreezeAll;
                var col = mesh.gameObject.GetComponent<MeshCollider>();
                col.enabled = true;
            }
        }

        audioSource.pitch = 1 + (isBreakingBonusConsuming ? 0 : bonusBreakingTime / breakingBonusMax) * stepBreakingSoundEffectPitchMultiplier; // Set pitch of audioSource according to bonusBreakingTime value, with this way, while bonusBreakingTime is increasing, the breaking sound effect becomes thinner. But isBreakingBonusConsuming is true, the meteor sound effect must not be pinched, so if isBreakingBonusConsuming is true, this value becomes 1 + 0.
        audioSource.PlayOneShot(isBreakingBonusConsuming ? meteorBreakSoundClip : breakSoundClip); // Play meteor or break sound effect according to isBreakingBonusConsuming.
        Destroy(step.gameObject, stepDestroyTime); // Destroy this step after stepDestroyTime time.

        for (int i = 0; i < step.childCount; i++) // For loop does this for each platform of this step.
        {
            var rig = step.GetChild(i).GetChild(0).gameObject.GetComponent<Rigidbody>(); // Gets rigidbody of the platform mesh object.
            Destroy(step.GetChild(i).GetChild(0).GetComponent<MeshCollider>());
            rig.constraints = RigidbodyConstraints.None; // No restriction about moving or rotating rigidbody in breaking process.
            rig.angularVelocity = Vector3.zero; // Set platform to no rotating. This is needed because steps can hit each other.
            rig.velocity = Vector3.zero; // Set platform to no moving. This is needed because steps can hit each other.
            step.GetChild(i).GetChild(0).GetComponent<MeshCollider>().enabled = false; // Disables the mesh collider of the platform mesh object.
            rig.AddForce((step.GetChild(i).GetChild(0).eulerAngles.y < 95 || step.GetChild(i).GetChild(0).eulerAngles.y > 265f) ? -800f * Mathf.Clamp(2f / bonusBreakingTime * stepXAxisForceBreakingTimeMultiplier, 0.2f, 2) * stepXAxisForceMultiplier : 800f * Mathf.Clamp(2f / bonusBreakingTime * stepXAxisForceBreakingTimeMultiplier, 0.2f, 2) * stepXAxisForceMultiplier, 3500f * stepYAxisForceMultiplier, 0f); // Applies the force to platform to make break effect. If the platform is on the left side, the force is applied to left with random magnitude, otherwise the force is applied to right with random magnitude.
            rig.AddRelativeTorque(Vector3.right * -300f * Random.Range(0.2f, 10f)); // Applies torque to platform with random magnitude, this is needed for making the break effect more realistic.
        }
    }
    void GameEnd() // Shows end game UI, updates total score and score values on the end game UI and resets some visual components.
    {
        isGameEnded = true; // If there is no steps else endingPlatform, game ended.
#if UNITY_EDITOR // If the game is playing on the Editor.
        if (SceneManager.sceneCount != 1) // If opened scene count is not 1, that means the LevelCreatorScene is opened.
        {
            Invoke("LevelEditorDelayedStop", 1); // Invokes LevelEditorDelayedStop to stop play mode as delayed.
        }
#endif
        if (SceneManager.sceneCount == 1) GetComponent<ADS>().ShowAd(); // If game is playing in game scene, trigger AD.
        if (secondChanceCoroutine != null) StopCoroutine(secondChanceCoroutine); // If second chance counter is working, stop it.
        bool isSecondChanceAsked = false;
#if UNITY_ADS
        if (isSecondChanceActive && secondChanceLimitCounter > 0 && generatedLevelSteps.Count > 1 && UnityEngine.Advertisements.Advertisement.IsReady())
        {
            isSecondChanceAsked = true;
            secondChanceCoroutine = StartCoroutine(SecondChangeCounter());
            secondChanceLimitCounter--;
        }
#endif
        gameEndUI.GameEndUIParent.gameObject.SetActive(true); // Activates game end UI.
        gameEndUI.passedInfoText.gameObject.SetActive(false); // This line fixes second chance bug.
        gameEndUI.passAllLevelsInfoText.gameObject.SetActive(false); // This line fixes second chance bug.
        gameEndUI.gameOverInfoText.gameObject.SetActive(false); // This line fixes second chance bug.
        breakBonus.fillAmount = 0; // Set breakBonus filling amount to 0, this is needed for when game ended, the break bonus indicator is not shown on background.
        breakBonusWarning.SetActive(false); // Disable warning indicator.
        gameEndUI.currentScoreValue.text = scoreValueText.text; // Update current score text on game end UI.
        int newScoreIfpassed = (int)scoreValue + int.Parse(gameEndUI.totalScoreValue.text); // If the level has been passed, the new score is needed to be calculated.
        switch (endingType)
        {
            case EndingTypes.COMPLETE: // On complete level action.
                gameEndUI.touchToContinue.gameObject.SetActive(true); // Activates touchToContinue text.
                gameEndUI.passedInfoText.gameObject.SetActive(true); // Activates passedInfotext text.
                gameEndUI.totalScoreValue.text = newScoreIfpassed.ToString(); // Updates totalScoreValue text with new total score.
                if (SceneManager.sceneCount == 1)
                {
                    PlayerPrefs.SetInt("passed_levels", currentLevel + 1); // Stores new passed levels number to "passed_levels" key.
                    PlayerPrefs.SetInt("total_score", newScoreIfpassed); // Stores new total score to "total_score" key.
                    PlayerPrefs.Save(); // Saves all changes on the PlayerPrefs.
                }
                break;
            case EndingTypes.PASSEDALLLEVELS: // On passed all levels action.
                gameEndUI.touchToRestartText.gameObject.SetActive(true); // Activates touchToRestartText text.
                gameEndUI.passAllLevelsInfoText.gameObject.SetActive(true); // Activates passAllLevelsInfoText text.
                gameEndUI.totalScoreValue.text = newScoreIfpassed.ToString(); // Updates totalScoreValue text with new total score.
                if (SceneManager.sceneCount == 1)
                {
                    PlayerPrefs.SetInt("total_score", newScoreIfpassed); // Stores new total score to "total_score" key.
                    PlayerPrefs.Save(); // Saves all changes on the PlayerPrefs.
                }
                break;
            case EndingTypes.GAMEOVER: // On gameover level action.
                Destroy(Player.gameObject); // Destroy the player, because it hit to obstacle.
                GeneratedObjects.position = new Vector3(0,-GeneratedObjects.GetChild(0).localPosition.y,0); // All steps' position must be set to 0, if the player does not touch the screen. So, assigning first steps' negative Y axis local position to GeneratedObjects Y axis position sets all steps' positions and by this way, first step's global position is set to 0 on Y axis.
                CrackedPlayer = Instantiate<GameObject>(CrackedPlayerSample).transform; // Generate cracked player. Cracked player has multiple separated meshes, so its meshes can move independently.
                CrackedPlayer.position = Player.position; // Set cracked player position as player position.
                if (isVibrationEnabled)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                Vibration.Vibrate(new long[] { 600, 300, 600, 300 }, 1);
                StartCoroutine(CancelVibration(3f));
#elif UNITY_IOS && !UNITY_EDITOR
                Handheld.Vibrate();
#endif
                }
                for (int i = 0; i < CrackedPlayer.childCount; i++) // For loop does this for each cracked player mesh inside cracked player.
                {
                    var rig = CrackedPlayer.GetChild(i).GetComponent<Rigidbody>(); // Get rigidbody of piece of cracked player.
                    rig.AddForce(CrackedPlayer.GetChild(i).localPosition * Random.Range(350f, 550f)); // Apply force with random magnitude to this rigidbody. CrackedPlayer.GetChild(i).localPosition means the force vector direction is to outside, so this can be used as explosion effect.
                    rig.AddTorque(Random.onUnitSphere * Random.Range(50f, 150f)); // Apply some torque to this rigidbody with random magnitude, with this way, the effect will be more realistic.
                }
                if (!isSecondChanceAsked) gameEndUI.touchToRestartText.gameObject.SetActive(true); // Activates touchToRestartText text.
                gameEndUI.gameOverInfoText.gameObject.SetActive(true); // Activates gameOverInfoText text.
                break;
        }
        if (endingType == EndingTypes.COMPLETE || endingType == EndingTypes.PASSEDALLLEVELS)
        {
            if (isVibrationEnabled)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Vibration.Vibrate(new long[] { 500, 100, 150, 400, 100, 150, 400, 100 }, 1);
                StartCoroutine(CancelVibration(4));
#elif UNITY_IOS && !UNITY_EDITOR
                Handheld.Vibrate();
#endif
            }
            var endingPlatform = generatedLevelSteps[0].step;
            endingPlatform.Find("ConfettiGeneratorLeft").GetComponent<ParticleSystem>().Play();
            endingPlatform.Find("ConfettiGeneratorRight").GetComponent<ParticleSystem>().Play();
        }
    }
    IEnumerator SecondChangeCounter() // A counter for second chance
    {
        int remainingTime = secondChanceTimeCounterMax - 1; // Counting range will be 0-secondChanceTimeCounterMax-1 range.
        gameEndUI.secondChanceCounterText.text = secondChanceTimeCounterMax.ToString(); // Sets counter text as max value.
        gameEndUI.secondChanceCounterText.gameObject.SetActive(true); // Section 10 - Comment: Arranges second chance UI.
        gameEndUI.secondChanceInfoText.gameObject.SetActive(true);
        gameEndUI.secondChanceWatchAdText.gameObject.SetActive(true);
        gameEndUI.totalScoreValue.gameObject.SetActive(false);
        gameEndUI.totalScoreText.gameObject.SetActive(false);
        gameEndUI.currentScoreText.gameObject.SetActive(false);
        gameEndUI.currentScoreValue.gameObject.SetActive(false); // End of Section 10
        yield return new WaitForSeconds(0.4f);
        while (remainingTime > -1)
        {
            gameEndUI.secondChanceCounterText.GetComponent<Animation>().Play("SecondChanceDecreaseAnim");
            yield return new WaitForSeconds(0.33f);
            gameEndUI.secondChanceCounterText.text = remainingTime.ToString();
            remainingTime--;
            yield return new WaitForSeconds(0.66f);
        }

        gameEndUI.secondChanceCounterText.gameObject.SetActive(false); // Section 10 - Comment: Arranges game end UI after waiting for second chance.
        gameEndUI.secondChanceInfoText.gameObject.SetActive(false);
        gameEndUI.secondChanceWatchAdText.gameObject.SetActive(false);
        gameEndUI.totalScoreValue.gameObject.SetActive(true);
        gameEndUI.totalScoreText.gameObject.SetActive(true);
        gameEndUI.currentScoreText.gameObject.SetActive(true);
        gameEndUI.currentScoreValue.gameObject.SetActive(true);
        gameEndUI.touchToRestartText.gameObject.SetActive(true);// End of Section 10

        secondChanceCoroutine = null; // Indicates counting is finished with no response, also indicates this coroutine is finished.
    }
    IEnumerator ToleranceEffect()
    {
        var currentScale = toleratedStep.localScale.x;
        var animatingScale = currentScale;
        if (isVibrationEnabled)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
                Vibration.Vibrate(new long[] { 10, 40, 40, 10 }, 1);
                StartCoroutine(CancelVibration(0.5f));
#elif UNITY_IOS && !UNITY_EDITOR
                Handheld.Vibrate();
#endif
        }    
        while (currentScale * 1.5f - animatingScale > 0.1f)
        {
            if(toleratedStep == null)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Vibration.Cancel();
#endif
                isToleranceEffectActivated = false;
                yield break;
            }
            animatingScale = Mathf.MoveTowards(animatingScale,currentScale * 1.5f,0.07f);
            toleratedStep.localScale = new Vector3(animatingScale,1,animatingScale);
            yield return null;
        }
        while (animatingScale - currentScale > 0.06f)
        {
            if (toleratedStep == null)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                Vibration.Cancel();
#endif
                isToleranceEffectActivated = false;
                yield break;
            }
            animatingScale = Mathf.MoveTowards(animatingScale, currentScale, 0.04f);
            toleratedStep.localScale = new Vector3(animatingScale, 1, animatingScale);
            yield return null;
        }
        toleratedStep.localScale = new Vector3(currentScale, 1, currentScale);

        toleratedStep = null;
        isToleranceEffectActivated = false;
    }
    IEnumerator CancelVibration(float time)
    {
        yield return new WaitForSeconds(time);
        Vibration.Cancel();
    }
    public void SetVibration(string options)
    {
        var isInitialize = options.Split('_')[0] == "1";
        var isButtonFromGameEndUI = options.Split('_')[1] == "1";
        var vibrationButton = isButtonFromGameEndUI ? VibrationButtonGameEndUI : VibrationButton;
        if (isInitialize)
        {
            var vibrationStatus = PlayerPrefs.GetInt("vibration",1);
            if (vibrationStatus == 0)
            {
                VibrationButton.GetComponent<Animation>().Play("VibrationOff");
                VibrationButtonGameEndUI.GetComponent<Animation>().Play("VibrationOff");
                isVibrationEnabled = false;
            }
            else
            {
                vibrationButton.GetComponent<Animation>().Play("VibrationOn");
                VibrationButtonGameEndUI.GetComponent<Animation>().Play("VibrationOn");
                isVibrationEnabled = true;
            }
            return;
        }
        if (isVibrationEnabled)
        {
            isVibrationEnabled = false;
            vibrationButton.GetComponent<Animation>().Play("VibrationOff");
#if UNITY_ANDROID && !UNITY_EDITOR
                Vibration.Cancel();
#endif
            PlayerPrefs.SetInt("vibration", 0);
        }
        else
        {
            isVibrationEnabled = true;
            vibrationButton.GetComponent<Animation>().Play("VibrationOn");
#if UNITY_ANDROID && !UNITY_EDITOR
            Vibration.Vibrate(new long[] {100,100,100,100},1);
            StartCoroutine(CancelVibration(0.8f));
#elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#endif

            PlayerPrefs.SetInt("vibration", 1);
        }
    }
    public void SetMusicOption(string options)
    {
        var isInitialize = options.Split('_')[0] == "1";
        var isButtonFromGameEndUI = options.Split('_')[1] == "1";
        var musicButton = isButtonFromGameEndUI ? MusicButtonGameEndUI : MusicButton;
        if (isInitialize)
        {
            var musicStatus = PlayerPrefs.GetInt("music", 1);
            if (musicStatus == 0)
            {
                MusicButton.GetComponent<Image>().sprite = musicOffSprite;
                MusicButtonGameEndUI.GetComponent<Image>().sprite = musicOffSprite;
                isMusicEnabled = false;
            }
            else
            {
                MusicButton.GetComponent<Image>().sprite = musicOnSprite;
                MusicButtonGameEndUI.GetComponent<Image>().sprite = musicOnSprite;
                musicAudioSource.Play();
                isMusicEnabled = true;
            }
            return;
        }
        if (isMusicEnabled)
        {
            isMusicEnabled = false;
            musicButton.GetComponent<Animation>().Play("MusicOff");
            StartCoroutine(SetMusicButtonSprite(false,musicButton));
            musicAudioSource.Stop();
            PlayerPrefs.SetInt("music", 0);
        }
        else
        {
            isMusicEnabled = true;
            musicButton.GetComponent<Animation>().Play("MusicOn");
            StartCoroutine(SetMusicButtonSprite(true, musicButton));
            musicAudioSource.Play();
            PlayerPrefs.SetInt("music", 1);
        }
    }
    IEnumerator SetMusicButtonSprite(bool isMusicOn, Button musicButton)
    {
        for (int i=0;i<20;i++)
        {
            yield return null;
        }
        if (isMusicOn)
        {
            musicButton.GetComponent<Image>().sprite = musicOnSprite;
            yield break;
        }
        musicButton.GetComponent<Image>().sprite = musicOffSprite;
    }
#if UNITY_EDITOR
    void LevelEditorDelayedStop()
    { // This method is used when the game is playing in Editor.
        UnityEditor.EditorApplication.isPlaying = false; // After one second (one second comes from GameEnd method Invoke method delay), the playing mode will be stopped in editor.
    }
#endif
    public void RestartGame()
    {
        if (SceneManager.sceneCount != 1 || creditOpenedBlocker) return; // If the game is playing in the editor, this method is not needed.
        bool isSecondChanceWanted = false;
        if (secondChanceCoroutine != null) // If this condition is true, the player wanted second chance
        {
            if (generatedLevelSteps.Count > 1) isSecondChanceWanted = true; // If there is step else ending platform and the player wanted second chance, the second chance can be given.
            StopCoroutine(secondChanceCoroutine); // Stops second chance counter.
            secondChanceCoroutine = null; // Indicates second chance counter is not working.
        }
        gameEndUI.GameEndUIParent.gameObject.SetActive(false); // Deactive game end UI, because game will restart.
        gameEndUI.secondChanceCounterText.gameObject.SetActive(false); // Section 11 - Comment: Arranges game end UI after waiting for second chance. This is needed because if the player wants second chance and starts game again until ending second chance lives, then when the game ends, the second chance UI is shown. This codes fix this.
        gameEndUI.secondChanceInfoText.gameObject.SetActive(false);
        gameEndUI.secondChanceWatchAdText.gameObject.SetActive(false);
        gameEndUI.totalScoreValue.gameObject.SetActive(true);
        gameEndUI.totalScoreText.gameObject.SetActive(true);
        gameEndUI.currentScoreText.gameObject.SetActive(true);
        gameEndUI.currentScoreValue.gameObject.SetActive(true); // End of Section 11

        if (isSecondChanceWanted) // If second chance has been given
        {
#if UNITY_ADS
            UnityEngine.Advertisements.Advertisement.Show(); // Show an AD.
#endif
            if (CrackedPlayer != null) Destroy(CrackedPlayer.gameObject); // Destroy cracked player to start game again.
            CrackedPlayer = null; // Indicates there is no cracked player.
            Player = Instantiate<GameObject>(PlayerSample).transform; // There is no player to start game again, so this line instantiates player again.
            isGameEnded = false; // Marks the game is not ended.
            endingType = EndingTypes.NONE; // This line is to not do under this line and also resets game ending indicator.
        }

        switch (endingType)
        {
            case EndingTypes.COMPLETE: // On complete level action.
                gameEndUI.touchToContinue.gameObject.SetActive(false); // Deactivates touchToContinue text.
                gameEndUI.passedInfoText.gameObject.SetActive(false); // Deactivates passedInfoText text.
                if (!isSecondChanceWanted) LoadLevel(currentLevel + 1); // Loads next level.
                break;
            case EndingTypes.PASSEDALLLEVELS: // On passed all levels action.
                gameEndUI.touchToRestartText.gameObject.SetActive(false); // Deactivates touchToRestartText text.
                gameEndUI.passAllLevelsInfoText.gameObject.SetActive(false); // Deactivates passAllLevelsInfoText text.
                if (!isSecondChanceWanted) LoadLevel(currentLevel); // Loads same level.
                break;
            case EndingTypes.GAMEOVER: // On gameover level action.
                gameEndUI.touchToRestartText.gameObject.SetActive(false); // Deactivates touchToRestartText text.
                gameEndUI.gameOverInfoText.gameObject.SetActive(false); // Deactivates gameOverInfoText text.
                if (!isSecondChanceWanted) LoadLevel(currentLevel); // Loads same level.
                break;
        }
    }
    void OnApplicationPause(bool isPaused) // If the game is paused, Unity framework invokes this method automatically.
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if(isPaused) 
        Vibration.Cancel(); // Stops the vibration
#endif
    }
    public void ButtonOpenLink(string link)
    {
        Application.OpenURL(link);
    }
    public void CreditOpened()
    {
        creditOpenedBlocker = true;
    }
    public void CreditClosed()
    {
        creditOpenedBlocker = false;
    }
    void TriggerLiveAutoGenerator() // Triggers live auto generator
    {
        Random.State backupRandom = Random.state; // Backups the random state to set random state in end of the block.
        Random.InitState(autoGeneratorSeed+currentLevel); // Sets the seed of the random method, autoGeneratorSeed+currentLevel will be same for the all players, so random generated levels will be same for the all players.
        Random.InitState(Random.Range(-currentLevel * 12345, currentLevel * 12345)); // Sets again the seed, this will be set to more complicated seed.
        var stepCount = (int)Random.Range(totalStepMinCurve.Evaluate((float)currentLevel / (float)maxHardnessLevelNumber) * 100, totalStepMaxCurve.Evaluate((float)currentLevel / (float)maxHardnessLevelNumber) * 100 + 1); // Picks step count between two curves randomly.
        var levelContent = ""; // Empty level content as string.
        var colorpairs = AutoGeneratorColorPairs;
        var colorPair = colorpairs[Random.Range(0, colorpairs.Count)]; // Picks the color pair randomly.
        var patternIndex = Random.Range(0, platformSamples.Length); // Picks the step pattern randomly.
        var pattern = platformSamples[patternIndex]; // Gets a reference of selected step pattern.
        var snapAngle = 360f / (float)pattern.count; // Sets the snap angle according to step platform count.
        var lastAngle = Random.Range(0f, 360f); // Picks lastAngle randomly between 0-360.
        var stepIsObstacleArray = new int[pattern.count]; // Creates an integer array for storing "is platform obstacle or not". 0 is normal platform, 1 is obstacle platform.
        var speed = (Random.Range(0f, autoGeneratorHardnessCurve.Evaluate(((float)currentLevel) / (maxHardnessLevelNumber + 1))) + 1.5f) * (Random.Range(0f, 1f) < 0.5f ? -1f : 1f); // Sets initial speed randomly.
        var scaleCurve = AutoGenerateScales[Random.Range(0, AutoGenerateScales.Count)]; // Picks scale curve from array of curves randomly.
        bool isAnyPlatformNormal = false; // Prevents all steps obstacle. With this way, one of the steps must normal platform at least. This is needed to make game playable.
        for (int j = 0; j < pattern.count; j++) // Sets platforms are obstacle or normal randomly.
        {
            stepIsObstacleArray[j] = Random.Range(0, 2);
            if (stepIsObstacleArray[j] == 0) isAnyPlatformNormal = true; // If one of them is normal platform, disable the next if block.
        }
        if (!isAnyPlatformNormal) // If all platforms are obstacle
        {
            stepIsObstacleArray[Random.Range(0, pattern.count)] = 0; // Set one of the obstacle to normal platform randomly.
        }
        for (int stepIndex = 0; stepIndex < stepCount; stepIndex++) // This for loop does this for each step.
        {
            var stepContent = ""; // Empty step content as string.
            bool isTrapped = isTrapOccured(currentLevel); // Call isTrapOccurred and store it to isTrapped variable.
            var angleDifference = speed * speedToAngle; // Sets angle difference.
            if (isTrapped) // If it is trapped.
            {
                isAnyPlatformNormal = false; // Prevents all steps obstacle. With this way, one of the steps must normal platform at least. This is needed to make game playable.
                for (int j = 0; j < pattern.count; j++) // Sets platforms are obstacle or normal randomly again because it is trapped.
                {
                    stepIsObstacleArray[j] = Random.Range(0, 2); 
                    if (stepIsObstacleArray[j] == 0) isAnyPlatformNormal = true; // If one of them is normal platform, disable the next if block.
                }
                if (!isAnyPlatformNormal) // If all platforms are obstacle
                {
                    stepIsObstacleArray[Random.Range(0, pattern.count)] = 0; // Set one of the obstacle to normal platform randomly.
                }
                angleDifference += (Random.Range(0f, 1f) < 0.5f ? -1f : 1f) * snapAngle * (float)Random.Range(1, pattern.count); // Sets angle difference randomly based on the snap angle. The snap angle is needed, because if it is not used, the steps rotations may be looking weird.
                if (angleDifference >= 360) // This if - else if block sets the angle difference value between 0-360
                {
                    angleDifference = 360 - angleDifference;
                }
                else if (angleDifference < 0)
                {
                    angleDifference += 360;
                }
            }
            var normalPlatformColor = colorPair.normalPlatformGradientColor.Evaluate((float)stepIndex / (float)stepCount); // Gets normal platform color from the graident by evaluating.
            var obstaclePlatformColor = colorPair.obstaclePlatformGradientColor.Evaluate((float)stepIndex / (float)stepCount); // Gets obstacle platform color from the graident by evaluating.

            var n_c_r = (int)(normalPlatformColor.r * 255f); // This part converts color components from range of 0-1 to 0-255 as integer.
            var n_c_g = (int)(normalPlatformColor.g * 255f);
            var n_c_b = (int)(normalPlatformColor.b * 255f);

            var o_c_r = (int)(obstaclePlatformColor.r * 255f);
            var o_c_g = (int)(obstaclePlatformColor.g * 255f);
            var o_c_b = (int)(obstaclePlatformColor.b * 255f); // End of part

            var scaleValue = scaleCurve.Evaluate((float)stepIndex / (float)stepCount); // Gets scale value of the step from selected scale curve.
            stepContent = speed.ToString(CultureInfo.InvariantCulture) + "/" + (stepIndex == 0 ? lastAngle : angleDifference).ToString(CultureInfo.InvariantCulture) + "/" + patternIndex.ToString(CultureInfo.InvariantCulture) + "/" + n_c_r.ToString(CultureInfo.InvariantCulture) + "_" + n_c_g.ToString(CultureInfo.InvariantCulture) + "_" + n_c_b.ToString(CultureInfo.InvariantCulture) + "/" + o_c_r.ToString(CultureInfo.InvariantCulture) + "_" + o_c_g.ToString(CultureInfo.InvariantCulture) + "_" + o_c_b.ToString(CultureInfo.InvariantCulture) + "/" + scaleValue.ToString(CultureInfo.InvariantCulture) + "/" + scaleValue.ToString(CultureInfo.InvariantCulture) + "/"; // Sets step content with obtained values.
            for (int p_index = 0; p_index < pattern.count; p_index++) // Sets is obstacle or not array. This is the integer sequence of the step content at the end. (for example: 010011)
            {
                stepContent += stepIsObstacleArray[p_index].ToString(CultureInfo.InvariantCulture);
            }
            stepContent += "/"; // Add slash character between two steps.
            if (stepIndex != stepCount - 1) // If this step is not the last step, add enter character (\n).
            {
                stepContent += "\n";
            }
            if (isTrapOccuredForSpeed(currentLevel)) // If speed trap is occurred.
            {
                speed = (Random.Range(0f, autoGeneratorHardnessCurve.Evaluate(((float)currentLevel) / (maxHardnessLevelNumber + 1))) + 1.5f) * (Random.Range(0f, 1f) < 0.5f ? -1f : 1f); // Set speed randomly if it is speed trapped.
            }
            levelContent += stepContent; // Write this step to level content.
        }

        Random.state = backupRandom; // Sets the random state to old one.

        var stepsByOrderAsStrings = levelContent.Split('\n'); // Return line contents as array of level content. Each line means one step.
        // Level file content structure => speed / ang / index  /      color      /     color2      / scaleX / scaleZ /   00000
        //                                 float  float  int        int_int_int       int_int_int     float    float   int sequence
        for (int j = 0; j < stepsByOrderAsStrings.Length; j++) // For loop does this for each step.
        {
            var stepsStrings = stepsByOrderAsStrings[j].Split('/'); // Gets step contents as string array (speed, ang, index, etc.)
            float stepRotationSpeed = float.Parse(stepsStrings[0], CultureInfo.InvariantCulture); // Parse string to float. 0 index is speed.
            float stepAngle = float.Parse(stepsStrings[1], CultureInfo.InvariantCulture); // Parse string to float. 1 index is offset angle relative to upper step.
            int stepId = int.Parse(stepsStrings[2]); // Parse string to int. 2 index is step index.
            var stepColorStrings = stepsStrings[3].Split('_'); // Split string to string array by '_' character. This returns color by r,g and b as string array.
            Color stepColor = new Color(float.Parse(stepColorStrings[0], CultureInfo.InvariantCulture) / 255f, float.Parse(stepColorStrings[1], CultureInfo.InvariantCulture) / 255f, float.Parse(stepColorStrings[2], CultureInfo.InvariantCulture) / 255f); // Parsing 0 1 2 indexes to float. This returns each color component in 0-255 range, then divides it with 255 to make it in range 0f-1f.
            var obstacleColorStrings = stepsStrings[4].Split('_'); // Split string to string array by '_' character. This returns color by r,g and b as string array.
            Color obstacleColor = new Color(float.Parse(obstacleColorStrings[0], CultureInfo.InvariantCulture) / 255f, float.Parse(obstacleColorStrings[1], CultureInfo.InvariantCulture) / 255f, float.Parse(obstacleColorStrings[2], CultureInfo.InvariantCulture) / 255f); // Parsing 0 1 2 indexes to float. This returns each color component in 0-255 range, then divides it with 255 to make it in range 0f-1f.
            float scaleX = float.Parse(stepsStrings[5], CultureInfo.InvariantCulture); // Parse string to float. 5 index is scaleX of the step.
            float scaleZ = float.Parse(stepsStrings[6], CultureInfo.InvariantCulture); // Parse string to float. 6 index is scaleZ of the step.
            bool[] isObstacleArray = new bool[stepsStrings[7].Length]; // Create a bool array that's length is equal to int sequence (like 00000 => length is 5) 0 means => normal platform, 1 means => obstacle platform.
            for (int i = 0; i < stepsStrings[7].Length; i++) // Section 9 - Comment: If char is '1' of int sequence, it is an obstacle platform and isObstacleArray[i] value becomes true. For loop does this for each char of the int sequence.
            {
                isObstacleArray[i] = stepsStrings[7][i] == '1';
            }// End of Section 9
            levelEnding += platformSamples[stepId].height; // Accumulate level ending to calculate level height. This is needed for placing ending platform.
            stepGenerationQueue.Enqueue(new StepGeneration(stepRotationSpeed, stepId, stepAngle, stepColor, obstacleColor, scaleX, scaleZ, isObstacleArray)); // Add this step to stepGenerationQueue with using datas. stepGenerationQueue is used for generating steps on time. If all steps are generated immediately, the game will be slow on mobile phones, even on computers.
        }
        levelIndicatorSource.text = currentLevel.ToString(); // Set levelIndicatorSource text to level number. levelIndicatorSource is left side of the progressing indicator on the top of screen. 
        levelIndicatorTarget.text = (currentLevel + 1).ToString(); // Set levelIndicatorTarget text to level number+1. If there is no level with level number+1, set text to "END".
        GenerateStepsAsBulk(stepGenerationCountOnInitializing); // Generate steps as bulk
    }
    bool isTrapOccured(float levelIndex)
    {
        var y_value = autoGeneratorHardnessCurve.Evaluate((levelIndex) / (maxHardnessLevelNumber + 1));
        return Random.Range(0f, 1f + trapOccurenceDecreaser) < y_value;
    }
    bool isTrapOccuredForSpeed(float levelIndex)
    {
        var y_value = autoGeneratorHardnessCurve.Evaluate((levelIndex) / (maxHardnessLevelNumber + 1));
        return Random.Range(0f, 1f + speedTrapOccurenceDecreaser) < y_value;
    }
    [System.Serializable]
    public class Step
    {
        public int count;
        public float height;
        public GameObject normalPlatformSample, obstaclePlatformSample;
    }
    public class StepReferenceToGame
    {
        public float rotationSpeed;
        public int index;
        public Transform step;
        public StepReferenceToGame(float rotationSpeed, int index, Transform step)
        {
            this.rotationSpeed = rotationSpeed;
            this.index = index;
            this.step = step;
        }
    }
    public class StepGeneration
    {
        public float rotationSpeed;
        public int stepIndex;
        public float stepAngle;
        public float scaleX, scaleZ;
        public Color stepColor, obstacleColor;
        public bool[] isObstacleArray;
        public StepGeneration(float rotationSpeed, int stepIndex, float stepAngle, Color stepColor, Color obstacleColor, float scaleX, float scaleZ, bool[] isObstacleArray)
        {
            this.rotationSpeed = rotationSpeed;
            this.stepIndex = stepIndex;
            this.stepAngle = stepAngle;
            this.stepColor = stepColor;
            this.obstacleColor = obstacleColor;
            this.scaleX = scaleX;
            this.scaleZ = scaleZ;
            this.isObstacleArray = isObstacleArray;
        }
    }
    [System.Serializable]
    public struct GameEndUI
    {
        public Transform GameEndUIParent;
        public Image backgroundPanel;
        public Text totalScoreValue, totalScoreText, currentScoreText, currentScoreValue, touchToRestartText, passAllLevelsInfoText, touchToContinue, gameOverInfoText, passedInfoText, secondChanceCounterText, secondChanceInfoText, secondChanceWatchAdText;
    }
    [System.Serializable]
    public struct BackgroundColorPair
    {
        public Gradient BackgroundColorTop, BackgroundColorBottom;
    }
    [System.Serializable]
    public struct SocialMediaPair
    {
        public Button button;
        public string link;
    }
    public static class Vibration
    {

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#endif

        public static void Vibrate()
        {
            if (isAndroid())
                vibrator.Call("vibrate");
            else
                Handheld.Vibrate();
        }


        public static void Vibrate(long milliseconds)
        {
            if (isAndroid())
                vibrator.Call("vibrate", milliseconds);
            else
                Handheld.Vibrate();
        }

        public static void Vibrate(long[] pattern, int repeat)
        {
            if (isAndroid())
                vibrator.Call("vibrate", pattern, repeat);
            else
                Handheld.Vibrate();
        }

        public static bool HasVibrator()
        {
            return isAndroid();
        }

        public static void Cancel()
        {
            if (isAndroid())
                vibrator.Call("cancel");
        }

        private static bool isAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
	return true;
#else
            return false;
#endif
        }
    }
}