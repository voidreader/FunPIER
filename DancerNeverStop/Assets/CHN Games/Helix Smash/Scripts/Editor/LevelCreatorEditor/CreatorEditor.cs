using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;
using System.IO;
using System.Globalization;
[CustomEditor(typeof(CreatorScript)), CanEditMultipleObjects]
public class CreatorEditor : Editor
{
    SerializedProperty addCount, selectedStepPatternIndex, levelIndex, speedToAngle, speedTrapOccurenceDecreaser, trapOccurenceDecreaser;
    SerializedProperty addingSteps, stepModifiers, autoGenerator;
    SerializedProperty StepPatternsProperty, LevelStepsProperty;
    string[] StepPatternNames;
    List<CreatorScript.StepPattern> StepPatterns;
    SerializedProperty obstacleOrNotToggles;
    SerializedProperty AutoGeneratorColorPairs, AutoGenerateScales, autoGeneratorHardnessCurve;
    static Gradient addStepsNormalColorGradient = new Gradient(), addStepsObstacleColorGradient = new Gradient();
    static AnimationCurve addStepsScaleXCurve = AnimationCurve.Constant(0, 1, 1), addStepsScaleZCurve = AnimationCurve.Constant(0, 1, 1), addStepsAngleCurve = AnimationCurve.Constant(0, 1, 0), addStepsSpeedCurve = AnimationCurve.Constant(0, 1, 0);
    static Gradient modifierStepsNormalColorGradient = new Gradient(), modifierStepsObstacleColorGradient = new Gradient();
    static AnimationCurve modifierStepsScaleXCurve = AnimationCurve.Constant(0, 1, 1), modifierStepsScaleZCurve = AnimationCurve.Constant(0, 1, 1), modifierStepsAngleCurve = AnimationCurve.Constant(0, 1, 0), modifierStepsSpeedCurve = AnimationCurve.Constant(0, 1, 0);
    SerializedProperty StepIndexMin, StepIndexMax;
    Transform CreatedLevel;
    Color oldColor;
    int totalStepCountMin, totalStepCountMax;
    int levelIndexStart, levelIndexEnd;
    public void OnEnable()
    {
        if (EditorSceneManager.sceneCount == 1) EditorSceneManager.OpenScene("Assets/CHN Games/Helix Smash/Scenes/GameScene.unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);
        StepPatternsProperty = serializedObject.FindProperty("StepPatterns");
        var creatorScript = (target as CreatorScript);
        StepPatterns = creatorScript.StepPatterns;
        CreatedLevel = GameObject.Find("GeneratedObjects").transform;

        addCount = serializedObject.FindProperty("addCount");
        selectedStepPatternIndex = serializedObject.FindProperty("selectedStepPatternIndex");
        addingSteps = serializedObject.FindProperty("addingSteps");
        stepModifiers = serializedObject.FindProperty("stepModifiers");
        autoGenerator = serializedObject.FindProperty("autoGenerator");
        obstacleOrNotToggles = serializedObject.FindProperty("obstacleOrNotToggles");
        StepIndexMin = serializedObject.FindProperty("StepIndexMin");
        StepIndexMax = serializedObject.FindProperty("StepIndexMax");
        levelIndex = serializedObject.FindProperty("levelIndex");
        AutoGeneratorColorPairs = serializedObject.FindProperty("AutoGeneratorColorPairs");
        AutoGenerateScales = serializedObject.FindProperty("AutoGenerateScales");
        autoGeneratorHardnessCurve = serializedObject.FindProperty("autoGeneratorHardnessCurve");
        speedToAngle = serializedObject.FindProperty("speedToAngle");
        trapOccurenceDecreaser = serializedObject.FindProperty("trapOccurenceDecreaser");
        speedTrapOccurenceDecreaser = serializedObject.FindProperty("speedTrapOccurenceDecreaser");

        addStepsNormalColorGradient = creatorScript.addStepsNormalColorGradient;
        addStepsObstacleColorGradient = creatorScript.addStepsObstacleColorGradient;
        addStepsScaleXCurve = creatorScript.addStepsScaleXCurve;
        addStepsScaleZCurve = creatorScript.addStepsScaleZCurve;
        addStepsAngleCurve = creatorScript.addStepsAngleCurve;
        addStepsSpeedCurve = creatorScript.addStepsSpeedCurve;
        modifierStepsNormalColorGradient = creatorScript.modifierStepsNormalColorGradient;
        modifierStepsObstacleColorGradient = creatorScript.modifierStepsObstacleColorGradient;
        modifierStepsScaleXCurve = creatorScript.modifierStepsScaleXCurve;
        modifierStepsScaleZCurve = creatorScript.modifierStepsScaleZCurve;
        modifierStepsAngleCurve = creatorScript.modifierStepsAngleCurve;
        modifierStepsSpeedCurve = creatorScript.modifierStepsSpeedCurve;

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (CreatedLevel == null) CreatedLevel = GameObject.Find("GeneratedObjects").transform;
        StepPatternNames = (target as CreatorScript).StepPatterns.Where((stepPatterns) => { return (stepPatterns.StepPatternName != "" && stepPatterns.count > 0 && stepPatterns.height > 0 && stepPatterns.PlatformSample != null && stepPatterns.ObstacleSample != null); }).Select((stepPatterns, s) => { return stepPatterns.StepPatternName; }).ToArray<string>();
        EditorGUILayout.PropertyField(StepPatternsProperty, true);
        EditorGUILayout.Space(); EditorGUILayout.Space();

        autoGenerator.boolValue = EditorGUILayout.Foldout(autoGenerator.boolValue, "Auto Generator");

        if (autoGenerator.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            levelIndexStart = EditorGUILayout.IntField("Level Index Start", levelIndexStart);
            levelIndexEnd = EditorGUILayout.IntField("End", levelIndexEnd);
            EditorGUILayout.EndHorizontal();
            if (levelIndexStart < 1)
            {
                levelIndexStart = 1;
            }
            if (levelIndexStart > levelIndexEnd)
            {
                levelIndexEnd = levelIndexStart;
            }
            EditorGUILayout.BeginHorizontal();
            totalStepCountMin = EditorGUILayout.IntField("Total Step Count Min", totalStepCountMin);
            totalStepCountMax = EditorGUILayout.IntField("Max", totalStepCountMax);
            EditorGUILayout.EndHorizontal();
            speedToAngle.floatValue = EditorGUILayout.FloatField("Speed To Angle", speedToAngle.floatValue);
            trapOccurenceDecreaser.floatValue = EditorGUILayout.FloatField("Trap Occurence Decrease Amount", trapOccurenceDecreaser.floatValue);
            speedTrapOccurenceDecreaser.floatValue = EditorGUILayout.FloatField("Speed Trap Occurence Decrease Amount", speedTrapOccurenceDecreaser.floatValue);

            if (totalStepCountMin < 1)
            {
                totalStepCountMin = 1;
            }
            if (totalStepCountMin > totalStepCountMax)
            {
                totalStepCountMax = totalStepCountMin;
            }
            EditorGUILayout.PropertyField(autoGeneratorHardnessCurve, new GUIContent("Hardness"), false);
            EditorGUILayout.PropertyField(AutoGeneratorColorPairs, new GUIContent("Color Pairs"), true);
            EditorGUILayout.PropertyField(AutoGenerateScales, new GUIContent("Scales"), true);

            if (GUILayout.Button("Generate"))
            {
                TriggerAutoGenerate();
            }
            GUILayout.Space(50);
        }

        addingSteps.boolValue = EditorGUILayout.Foldout(addingSteps.boolValue, "Add Steps");
        if (addingSteps.boolValue)
        {
            if (StepPatternNames.Length == 1) selectedStepPatternIndex.intValue = 0;
            if (StepPatternNames.Length > 0)
            {
                selectedStepPatternIndex.intValue = EditorGUILayout.Popup("Select Step Pattern", selectedStepPatternIndex.intValue, StepPatternNames);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Obstacle or Not Toggles");
                EditorGUILayout.BeginHorizontal();

                if (obstacleOrNotToggles.arraySize != StepPatterns[selectedStepPatternIndex.intValue].count)
                {
                    obstacleOrNotToggles.arraySize = StepPatterns[selectedStepPatternIndex.intValue].count;
                }
                for (int i = 0; i < StepPatterns[selectedStepPatternIndex.intValue].count; i++)
                {
                    obstacleOrNotToggles.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Toggle(obstacleOrNotToggles.GetArrayElementAtIndex(i).boolValue);
                }
                EditorGUILayout.EndHorizontal();

                addStepsNormalColorGradient = EditorGUILayout.GradientField("Adding Steps Normal Color Gradient", addStepsNormalColorGradient);
                addStepsObstacleColorGradient = EditorGUILayout.GradientField("Adding Steps Obstacle Color Gradient", addStepsObstacleColorGradient);

                addStepsScaleXCurve = EditorGUILayout.CurveField("Adding Steps Scale X Curve", addStepsScaleXCurve);
                addStepsScaleZCurve = EditorGUILayout.CurveField("Adding Steps Scale Z Curve", addStepsScaleZCurve);

                addStepsAngleCurve = EditorGUILayout.CurveField("Adding Steps Angle Curve", addStepsAngleCurve);

                addStepsSpeedCurve = EditorGUILayout.CurveField("Adding Steps Speed Curve", addStepsSpeedCurve);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("addCount"), new GUIContent("Adding Step Count")); addCount.intValue = Mathf.Clamp(addCount.intValue, 0, addCount.intValue);
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.cyan;
                if (GUILayout.Button("Add Step") && EditorUtility.DisplayDialog("Are you sure?", "Do you want to add " + addCount.intValue + " " + StepPatternNames[selectedStepPatternIndex.intValue] + " steps?", "Yes", "Cancel"))
                {
                    AddStep();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.Space(); EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.LabelField("You need to add Step Patterns to add new steps!");
                EditorGUILayout.LabelField("A step must match these conditions => StepPatternName, PlatformSample and ObstacleSample must be assigned, Height > 0, Count > 0");
            }
        }
        stepModifiers.boolValue = EditorGUILayout.Foldout(stepModifiers.boolValue, "Step Modifiers");
        if (stepModifiers.boolValue)
        {
            if (CreatedLevel.childCount == 0)
            {
                EditorGUILayout.LabelField("You need to add Level Steps to apply modifiers!");
            }
            else
            {
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                StepIndexMin.intValue = EditorGUILayout.IntField("Step Index Min", StepIndexMin.intValue);
                StepIndexMax.intValue = EditorGUILayout.IntField("Step Index Max", StepIndexMax.intValue);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.BeginHorizontal();
                modifierStepsNormalColorGradient = EditorGUILayout.GradientField("Modifier Normal Color Gradient", modifierStepsNormalColorGradient);
                if (GUILayout.Button("Do in Range")) DoModifier(ModifierMode.NORMALCOLOR);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                modifierStepsObstacleColorGradient = EditorGUILayout.GradientField("Modifier Obstacle Color Gradient", modifierStepsObstacleColorGradient);
                if (GUILayout.Button("Do in Range")) DoModifier(ModifierMode.OBSTACLECOLOR);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                modifierStepsScaleXCurve = EditorGUILayout.CurveField("Modifier Scale X Curve", modifierStepsScaleXCurve);
                if (GUILayout.Button("Do in Range")) DoModifier(ModifierMode.SCALEX);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                modifierStepsScaleZCurve = EditorGUILayout.CurveField("Modifier Scale Z Curve", modifierStepsScaleZCurve);
                if (GUILayout.Button("Do in Range")) DoModifier(ModifierMode.SCALEZ);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                modifierStepsAngleCurve = EditorGUILayout.CurveField("Modifier Angle Curve", modifierStepsAngleCurve);
                if (GUILayout.Button("Do in Range")) DoModifier(ModifierMode.ANGLE);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                modifierStepsSpeedCurve = EditorGUILayout.CurveField("Modifier Speed Curve", modifierStepsSpeedCurve);
                if (GUILayout.Button("Do in Range")) DoModifier(ModifierMode.SPEED);
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Delete Steps in Range")) DeleteSteps();
                if (GUILayout.Button("Reorder Steps")) ReorderSteps();
            }
        }
        GUILayout.Space(40);
        GUILayout.Label("Level Extractor");
        EditorGUILayout.BeginHorizontal();
        levelIndex.intValue = EditorGUILayout.IntField("Level Index", levelIndex.intValue);
        if (GUILayout.Button("+")) levelIndex.intValue++;
        if (GUILayout.Button("-")) levelIndex.intValue--;
        EditorGUILayout.EndHorizontal();
        levelIndex.intValue = Mathf.Clamp(levelIndex.intValue, 1, levelIndex.intValue);
        oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Extract this level at " + levelIndex.intValue + " level index"))
        {
            LevelExtractor();
        }
        GUI.backgroundColor = oldColor;
        var lastRect = GUILayoutUtility.GetLastRect();
        GUILayout.Space(100);
        oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        if (GUI.Button(new Rect(lastRect.x, lastRect.y + lastRect.height, lastRect.width, 100), "SAVE STEP PATTERNS\n\nIf you add, change or modify a new step pattern, you need to click here"))
        {
            Object gameManagerTarget = GameObject.Find("GameManager").GetComponent<GameManager>();
            var platformSamples = (gameManagerTarget as GameManager).platformSamples;
            SerializedObject serializedGameManager = new SerializedObject(gameManagerTarget);
            serializedGameManager.Update();
            SerializedProperty platformSamplesProperty = serializedGameManager.FindProperty("platformSamples");
            platformSamplesProperty.arraySize = StepPatterns.Count;
            serializedGameManager.ApplyModifiedProperties();
            for (int i = 0; i < StepPatterns.Count; i++)
            {
                platformSamples[i] = new GameManager.Step();
                AddPlatformComponents(StepPatterns[i].PlatformSample);
                AddPlatformComponents(StepPatterns[i].ObstacleSample);
                platformSamples[i].normalPlatformSample = StepPatterns[i].PlatformSample;
                platformSamples[i].obstaclePlatformSample = StepPatterns[i].ObstacleSample;
                platformSamples[i].height = StepPatterns[i].height;
                platformSamples[i].count = StepPatterns[i].count;
            }
            serializedGameManager.ApplyModifiedProperties();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                EditorSceneManager.SaveScene(EditorSceneManager.GetSceneAt(i));
        }
        GUI.backgroundColor = oldColor;
        serializedObject.ApplyModifiedProperties();
    }
    enum ModifierMode { NORMALCOLOR, OBSTACLECOLOR, SCALEX, SCALEZ, ANGLE, SPEED }
    void DoModifier(ModifierMode mode)
    {
        int RealStepIndexMin = ((StepIndexMin.intValue > 0) && (StepIndexMin.intValue < StepIndexMax.intValue)) ? StepIndexMin.intValue : 999999;
        int RealStepIndexMax = ((StepIndexMax.intValue > StepIndexMin.intValue) && (StepIndexMax.intValue < CreatedLevel.childCount + 1)) ? StepIndexMax.intValue : 999999;
        if (RealStepIndexMin == 999999 || RealStepIndexMax == 999999)
        {
            EditorUtility.DisplayDialog("Wrong Index", "The index range is not correct (" + StepIndexMin.intValue + "-" + StepIndexMax.intValue + ").", "Ok");
            return;
        }
        if (mode == ModifierMode.NORMALCOLOR || mode == ModifierMode.OBSTACLECOLOR)
        {
            for (int i = StepIndexMin.intValue - 1; i < RealStepIndexMax; i++)
            {
                var timeValue = ((float)i - RealStepIndexMin + 1) / ((float)(RealStepIndexMax - RealStepIndexMin));
                var color = (mode == ModifierMode.NORMALCOLOR ? modifierStepsNormalColorGradient : modifierStepsObstacleColorGradient).Evaluate(timeValue);
                var step = CreatedLevel.GetChild(i);
                for (int j = 0; j < step.childCount; j++)
                {
                    if (mode == ModifierMode.NORMALCOLOR)
                    {
                        if (step.GetChild(j).name.Split('_')[1] == "0")
                        {
                            step.GetChild(j).GetChild(0).GetComponent<Renderer>().sharedMaterial.color = color;
                        }
                    }
                    else
                    {
                        if (step.GetChild(j).name.Split('_')[1] == "1")
                        {
                            step.GetChild(j).GetChild(0).GetComponent<Renderer>().sharedMaterial.color = color;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = StepIndexMin.intValue - 1; i < RealStepIndexMax; i++)
            {
                var timeValue = ((float)i - RealStepIndexMin + 1) / ((float)(RealStepIndexMax - RealStepIndexMin));
                AnimationCurve activeCurve = null;
                var step = CreatedLevel.GetChild(i);
                switch (mode)
                {
                    case ModifierMode.SCALEX:
                        activeCurve = modifierStepsScaleXCurve;
                        step.localScale = new Vector3(activeCurve.Evaluate(timeValue), step.localScale.y, step.localScale.z);
                        break;
                    case ModifierMode.SCALEZ:
                        activeCurve = modifierStepsScaleZCurve;
                        step.localScale = new Vector3(step.localScale.x, step.localScale.y, activeCurve.Evaluate(timeValue));
                        break;
                    case ModifierMode.ANGLE:
                        activeCurve = modifierStepsAngleCurve;
                        step.localEulerAngles = new Vector3(0, activeCurve.Evaluate(timeValue) * 360f, 0);
                        break;
                    case ModifierMode.SPEED:
                        activeCurve = modifierStepsSpeedCurve;
                        step.name = "Vars_" + activeCurve.Evaluate(timeValue) + "_" + step.name.Split('_')[2];
                        break;
                }
            }
        }
    }
    void OnSceneGUI()
    {
        for (int i = 0; i < CreatedLevel.childCount; i++)
        {
            Handles.Label(CreatedLevel.GetChild(i).position + new Vector3(-6, 0.15f, 0), (i + 1).ToString());
            if (i % 5 == 4)
            {
                var oldColor = Handles.color;
                Handles.color = Color.red;
                Handles.DrawSolidDisc(CreatedLevel.GetChild(i).position + new Vector3(-6.3f, 0, 0), Vector3.forward, 0.2f);
                Handles.color = oldColor;
            }
        }
    }
    void AddStep()
    {
        for (int i = 0; i < addCount.intValue; i++)
        {
            var Step = new GameObject("Step").transform;
            Step.parent = CreatedLevel;
            Step.SetAsLastSibling();
            float timeValue = ((float)i) / ((float)addCount.intValue);
            for (int j = 0; j < StepPatterns[selectedStepPatternIndex.intValue].count; j++)
            {
                var platform = Instantiate<GameObject>(obstacleOrNotToggles.GetArrayElementAtIndex(j).boolValue ? StepPatterns[selectedStepPatternIndex.intValue].ObstacleSample : StepPatterns[selectedStepPatternIndex.intValue].PlatformSample, Step).transform;
                platform.eulerAngles = new Vector3(0, j * (360f / (float)StepPatterns[selectedStepPatternIndex.intValue].count), 0);
                if (platform.GetChild(0).GetComponent<Renderer>().sharedMaterial.shader.name == "Custom/PlatformShader")
                {
                    platform.GetChild(0).GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Custom/PlatformShader"));
                    platform.GetChild(0).GetComponent<Renderer>().sharedMaterial.color = (obstacleOrNotToggles.GetArrayElementAtIndex(j).boolValue ? addStepsObstacleColorGradient : addStepsNormalColorGradient).Evaluate(timeValue);
                }
                platform.name = "Platform_" + (obstacleOrNotToggles.GetArrayElementAtIndex(j).boolValue ? 1 : 0);
            }
            Step.position = new Vector3(0, (CreatedLevel.childCount > 1 ? CreatedLevel.GetChild(CreatedLevel.childCount - 2).position.y : StepPatterns[selectedStepPatternIndex.intValue].height) - StepPatterns[selectedStepPatternIndex.intValue].height, 0);
            Step.eulerAngles = new Vector3(0, addStepsAngleCurve.Evaluate(timeValue) * 360f, 0);
            Step.localScale = new Vector3(addStepsScaleXCurve.Evaluate(timeValue), 1, addStepsScaleZCurve.Evaluate(timeValue));
            Step.name = "Vars_" + addStepsSpeedCurve.Evaluate(timeValue) + "_" + selectedStepPatternIndex.intValue;
        }
        EditorUtility.DisplayDialog("Adding Steps", addCount.intValue + " " + StepPatternNames[selectedStepPatternIndex.intValue] + " Steps Added", "Ok");
    }
    void LevelExtractor()
    {
        if (File.Exists("Assets/CHN Games/Helix Smash/Resources/Levels/" + levelIndex.intValue + ".txt") && !EditorUtility.DisplayDialog("Are you sure", "This level id was generated before. Are you sure to regenerate it? It will be overwritten with new one.", "Yes", "Cancel"))
        {
            return;
        }
        string level = "";
        for (int i = 0; i < CreatedLevel.childCount; i++)
        {
            var step = CreatedLevel.GetChild(i);
            var index = step.name.Split('_');
            float relativeAngle = step.eulerAngles.y - (i > 1 ? CreatedLevel.GetChild(i - 1).eulerAngles.y : 0);
            string normalPlatformColor = "0_0_0";
            string obstaclePlatformColor = "0_0_0";
            string isObstacleSequence = "";
            Material normalPlatformMat = null, obstaclePlatformMat = null;
            for (int j = 0; j < step.childCount; j++)
            {
                string platformType = step.GetChild(j).name.Split('_')[1];
                if (normalPlatformMat == null && platformType == "0") normalPlatformMat = step.GetChild(j).GetChild(0).GetComponent<Renderer>().sharedMaterial;
                if (obstaclePlatformMat == null && platformType == "1") obstaclePlatformMat = step.GetChild(j).GetChild(0).GetComponent<Renderer>().sharedMaterial;
                isObstacleSequence += platformType;
            }
            if (normalPlatformMat.shader.name == "Custom/PlatformShader")
            {
                normalPlatformColor = (int)(normalPlatformMat.color.r * 255f) + "_" + (int)(normalPlatformMat.color.g * 255f) + "_" + (int)(normalPlatformMat.color.b * 255f);
            }
            if (obstaclePlatformMat.shader.name == "Custom/PlatformShader")
            {
                obstaclePlatformColor = (int)(obstaclePlatformMat.color.r * 255f) + "_" + (int)(obstaclePlatformMat.color.g * 255f) + "_" + (int)(obstaclePlatformMat.color.b * 255f);
            }
            level += index[1] + "/" + relativeAngle + "/" + index[2] + "/" + normalPlatformColor + "/" + obstaclePlatformColor + "/" + step.localScale.x + "/" + step.localScale.z + "/" + isObstacleSequence + "/";
            if (i < CreatedLevel.childCount - 1) level += "\n";
        }
        var file = File.Create("Assets/CHN Games/Helix Smash/Resources/Levels/" + levelIndex.intValue + ".txt");
        StreamWriter writer = new StreamWriter(file);
        writer.NewLine = level;
        writer.WriteLine();
        writer.Flush();
        file.Close();
        AssetDatabase.Refresh();
    }
    void DeleteSteps()
    {
        int RealStepIndexMin = ((StepIndexMin.intValue > 0) && (StepIndexMin.intValue < StepIndexMax.intValue)) ? StepIndexMin.intValue : 999999;
        int RealStepIndexMax = ((StepIndexMax.intValue > StepIndexMin.intValue) && (StepIndexMax.intValue < CreatedLevel.childCount + 1)) ? StepIndexMax.intValue : 999999;
        if (RealStepIndexMin == 999999 || RealStepIndexMax == 999999)
        {
            EditorUtility.DisplayDialog("Wrong Index", "The index range is not correct (" + StepIndexMin.intValue + "-" + StepIndexMax.intValue + ").", "Ok");
            return;
        }
        if (!EditorUtility.DisplayDialog("Are you sure?", "Do you want to delete " + (StepIndexMax.intValue - StepIndexMin.intValue + 1) + " steps in range " + StepIndexMin.intValue + "-" + StepIndexMax.intValue + "?", "Yes", "Cancel")) return;
        int realIndex = StepIndexMax.intValue - 1;
        for (int i = StepIndexMin.intValue - 1; i < StepIndexMax.intValue; i++)
        {
            GameObject.DestroyImmediate(CreatedLevel.GetChild(realIndex).gameObject);
            realIndex--;
        }
        EditorUtility.DisplayDialog("Steps are deleted", (StepIndexMax.intValue - StepIndexMin.intValue + 1) + "steps are deleted steps in range " + StepIndexMin.intValue + "-" + StepIndexMax.intValue + ".", "Ok");
    }
    void ReorderSteps()
    {
        for (int i = 0; i < CreatedLevel.childCount; i++)
        {
            var step = CreatedLevel.GetChild(i);
            var index = step.name.Split('_');
            int stepIndex = int.Parse(index[2]);
            float height = StepPatterns[stepIndex].height;
            step.position = new Vector3(0, (i > 0 ? CreatedLevel.GetChild(i - 1).position.y : height) - height, 0);
        }
    }
    void AddPlatformComponents(GameObject platform)
    {
        var platformObj = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(platform));
        var mesh = platform.transform.GetChild(0);
        bool isAnythingChanged = false;
        if (mesh.GetComponent<MeshCollider>() == null)
        {
            isAnythingChanged = true;
            var col = mesh.gameObject.AddComponent<MeshCollider>();
            col.enabled = false;
            col.convex = true;
            Debug.LogWarning("Mesh collider has been added to " + platform.name);
        }
        if (platformObj.transform.Find("splashOrigin") == null)
        {
            isAnythingChanged = true;
            var splashOrigin = new GameObject("splashOrigin").transform;
            splashOrigin.parent = platformObj.transform;
            splashOrigin.SetAsLastSibling();
            Debug.LogWarning("Set your splashOrigin position of " + platform.name + " from assets located at " + AssetDatabase.GetAssetPath(platform));
        }
        if (isAnythingChanged) PrefabUtility.SaveAsPrefabAsset(platformObj, AssetDatabase.GetAssetPath(platform));
    }
    void TriggerAutoGenerate()
    {
        if (!VerifyHardnessCurve())
        {
            EditorUtility.DisplayDialog("Error", "The hardness curve is not valid, process will be aborted\n Note: Starting point must be 0 in time and ending point must be 1 in time. Also, all keys must be positive or 0 and lower than 1", "Ok");
            return;
        }
        if (!VerifyScaleCurves())
        {
            EditorUtility.DisplayDialog("Error", "The scale curves are not valid, process will be aborted\n Note: Starting point must be 0 in time and ending point must be 1 in time.", "Ok");
            return;
        }
        for (int i = levelIndexStart; i <= levelIndexEnd; i++)
        {
            var stepCount = Random.Range(totalStepCountMin, totalStepCountMax + 1);
            var levelContent = "";
            var creatorInstance = (target as CreatorScript);
            var colorpairs = creatorInstance.AutoGeneratorColorPairs;
            var colorPair = colorpairs[Random.Range(0, colorpairs.Count)];
            var patternIndex = Random.Range(0, creatorInstance.StepPatterns.Count);
            var pattern = creatorInstance.StepPatterns[patternIndex];
            var snapAngle = 360f / (float)pattern.count;
            var lastAngle = Random.Range(0f,360f);
            var stepIsObstacleArray = new int[pattern.count];
            var speed = (Random.Range(0f, creatorInstance.autoGeneratorHardnessCurve.Evaluate(((float)i) / (levelIndexEnd - levelIndexStart + 1))) + 1.5f) * (Random.Range(0f, 1f) < 0.5f ? -1f : 1f);
            var scaleCurve = creatorInstance.AutoGenerateScales[Random.Range(0,creatorInstance.AutoGenerateScales.Count)];
            for (int j = 0; j < pattern.count; j++)
            {
                stepIsObstacleArray[j] = Random.Range(0, 2);
            }
            for (int stepIndex = 0; stepIndex < stepCount; stepIndex++)
            {
                var stepContent = "";
                bool isTrapped = isTrapOccured(i);
                var angleDifference = speed * speedToAngle.floatValue;
                if (isTrapped)
                {
                    bool isAnyPlatformNormal = false;
                    for (int j=0;j<pattern.count;j++)
                    {
                        stepIsObstacleArray[j] = Random.Range(0,2);
                        if (stepIsObstacleArray[j] == 0) isAnyPlatformNormal = true;
                    }
                    if (!isAnyPlatformNormal)
                    {
                        stepIsObstacleArray[Random.Range(0,pattern.count)] = 0;
                    }
                    angleDifference += (Random.Range(0f, 1f) < 0.5f ? -1f : 1f) * snapAngle * (float)Random.Range(1, pattern.count);
                    if (angleDifference >= 360)
                    {
                        angleDifference = 360 - angleDifference; 
                    }else if (angleDifference < 0)
                    {
                        angleDifference += 360;
                    }
                }
                var normalPlatformColor = colorPair.normalPlatformGradientColor.Evaluate((float)stepIndex / (float)stepCount);
                var obstaclePlatformColor = colorPair.obstaclePlatformGradientColor.Evaluate((float)stepIndex / (float)stepCount);

                var n_c_r = (int)(normalPlatformColor.r * 255f); // This part converts color components from range of 0-1 to 0-255 as integer.
                var n_c_g = (int)(normalPlatformColor.g * 255f);
                var n_c_b = (int)(normalPlatformColor.b * 255f);

                var o_c_r = (int)(obstaclePlatformColor.r * 255f);
                var o_c_g = (int)(obstaclePlatformColor.g * 255f);
                var o_c_b = (int)(obstaclePlatformColor.b * 255f); // End of part

                var scaleValue = scaleCurve.Evaluate((float)stepIndex / (float)stepCount);
                stepContent = speed.ToString(CultureInfo.InvariantCulture) + "/" + (stepIndex==0?lastAngle: angleDifference).ToString(CultureInfo.InvariantCulture) + "/" + patternIndex.ToString(CultureInfo.InvariantCulture) + "/" + n_c_r.ToString(CultureInfo.InvariantCulture) + "_" + n_c_g.ToString(CultureInfo.InvariantCulture) + "_" + n_c_b.ToString(CultureInfo.InvariantCulture) + "/" + o_c_r.ToString(CultureInfo.InvariantCulture) + "_" + o_c_g.ToString(CultureInfo.InvariantCulture) + "_" + o_c_b.ToString(CultureInfo.InvariantCulture) + "/" + scaleValue.ToString(CultureInfo.InvariantCulture) + "/" + scaleValue.ToString(CultureInfo.InvariantCulture) + "/";
                for (int p_index = 0;p_index < pattern.count;p_index++)
                {
                    stepContent += stepIsObstacleArray[p_index].ToString(CultureInfo.InvariantCulture);
                }
                stepContent += "/";
                if (stepIndex != stepCount-1)
                {
                    stepContent += "\n";
                }
                if (isTrapOccuredForSpeed(i))
                {
                    speed = (Random.Range(0f, creatorInstance.autoGeneratorHardnessCurve.Evaluate(((float)i) / (levelIndexEnd - levelIndexStart + 1))) + 1.5f) * (Random.Range(0f, 1f) < 0.5f ? -1f : 1f);
                }
                levelContent += stepContent;
            }
            AutoGenerateLevelExtractor(i,levelContent);
        }
    }
    bool VerifyHardnessCurve()
    {
        var autoGeneratorHardnessCurve = (target as CreatorScript).autoGeneratorHardnessCurve;
        if (autoGeneratorHardnessCurve.length == 0)
        {
            return false;
        }
        if (autoGeneratorHardnessCurve.keys[0].time != 0)
        {
            return false;
        }
        if (autoGeneratorHardnessCurve.keys[autoGeneratorHardnessCurve.length - 1].time != 1)
        {
            return false;
        }
        foreach (var key in autoGeneratorHardnessCurve.keys)
        {
            if (key.value < 0 || key.value > 1)
            {
                return false;
            }
        }
        return true;
    }
    bool VerifyScaleCurves()
    {
        foreach (var curve in (target as CreatorScript).AutoGenerateScales)
        {
            if (curve.length == 0)
            {
                return false;
            }
            if (curve.keys[0].time != 0)
            {
                return false;
            }
            if (curve.keys[curve.length - 1].time != 1)
            {
                return false;
            }
        }
        return true;
    }
    bool isTrapOccured(float levelIndex)
    {
        var y_value = (target as CreatorScript).autoGeneratorHardnessCurve.Evaluate((levelIndex - levelIndexStart) /(levelIndexEnd - levelIndexStart +1));
        return Random.Range(0f, 1f + trapOccurenceDecreaser.floatValue) < y_value;
    }
    bool isTrapOccuredForSpeed(float levelIndex)
    {
        var y_value = (target as CreatorScript).autoGeneratorHardnessCurve.Evaluate((levelIndex - levelIndexStart) / (levelIndexEnd - levelIndexStart + 1));
        return Random.Range(0f, 1f + speedTrapOccurenceDecreaser.floatValue) < y_value;
    }
    void AutoGenerateLevelExtractor(int levelIndex,string levelContent)
    {
        var file = File.Create("Assets/CHN Games/Helix Smash/Resources/Levels/" + levelIndex + ".txt");
        StreamWriter writer = new StreamWriter(file);
        writer.NewLine = levelContent;
        writer.WriteLine();
        writer.Flush();
        file.Close();
        AssetDatabase.Refresh();
    }
}