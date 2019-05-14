﻿#region copyright
// ------------------------------------------------------------------------
//  Copyright (C) 2013-2019 Dmitriy Yukhanov - focus [http://codestage.net]
// ------------------------------------------------------------------------
#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
#define ACTK_ANDROID_DEVICE
#endif

#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
#define ACTK_DEBUG_ENABLED
#endif

namespace CodeStage.AntiCheat.Detectors
{
	using Common;

	using System;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	/// <summary>
	/// Allows to detect Cheat Engine's speed hack (and maybe some other speed hack tools) usage.
	/// </summary>
	/// Just add it to any GameObject as usual or through the "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit"
	/// menu to get started.<br/>
	/// You can use detector completely from inspector without writing any code except the actual reaction on cheating.
	/// 
	/// Avoid using detectors from code at the Awake phase.
	[AddComponentMenu(MenuPath + ComponentName)]
	[DisallowMultipleComponent]
	[HelpURL(ACTkConstants.DocsRootUrl + "class_code_stage_1_1_anti_cheat_1_1_detectors_1_1_speed_hack_detector.html")]
	public class SpeedHackDetector : ACTkDetectorBase
	{
		internal const string ComponentName = "Speed Hack Detector";
		internal const string LogPrefix = ACTkConstants.LogPrefix + ComponentName + ": ";

		private const long TicksPerSecond = TimeSpan.TicksPerMillisecond * 1000;
		private const int Threshold = 5000000; // == 500 ms, allowed time difference between genuine and vulnerable ticks
		private const float ThresholdFloat = 0.5f; // == 500 ms, allowed time difference between genuine ticks and vulnerable time

#if ACTK_ANDROID_DEVICE
		private const string RoutinesClassPath = "net.codestage.actk.androidnative.ACTkAndroidRoutines";
#endif

		private static int instancesInScene;

		#region public fields
		/// <summary> 
		/// Time (in seconds) between detector checks.
		/// </summary>
		[Tooltip("Time (in seconds) between detector checks.")]
		public float interval = 1f;

		/// <summary>
		/// Maximum false positives count allowed before registering speed hack.
		/// </summary>
		[Tooltip("Maximum false positives count allowed before registering speed hack.")]
		public byte maxFalsePositives = 3;

		/// <summary>
		/// Amount of sequential successful checks before clearing internal false positives counter.<br/>
		/// Set 0 to disable Cool Down feature.
		/// </summary>
		[Tooltip("Amount of sequential successful checks before clearing internal false positives counter.\nSet 0 to disable Cool Down feature.")]
		public int coolDown = 30;
		#endregion

		#region private variables
		private byte currentFalsePositives;
		private int currentCooldownShots;
		private long ticksOnStart;
		private long vulnerableTicksOnStart;
		private long previousTicks;
		private long previousIntervalTicks;
		private float vulnerableTimeOnStart;

#if ACTK_ANDROID_DEVICE
		private AndroidJavaClass routinesClass;
		private bool androidTimeReadAttemptWasMade;
#endif

		#endregion

		#region public static methods
		/// <summary>
		/// Creates new instance of the detector at scene if it doesn't exists. Make sure to call NOT from Awake phase.
		/// </summary>
		/// <returns>New or existing instance of the detector.</returns>
		public static SpeedHackDetector AddToSceneOrGetExisting()
		{
			return GetOrCreateInstance;
		}

		/// <summary>
		/// Starts speed hack detection for detector you have in scene.
		/// </summary>
		/// Make sure you have properly configured detector in scene with #autoStart disabled before using this method.
		public static void StartDetection()
		{
			if (Instance != null)
			{
				Instance.StartDetectionInternal(null, Instance.interval, Instance.maxFalsePositives, Instance.coolDown);
			}
			else
			{
				Debug.LogError(LogPrefix + "can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		/// <summary>
		/// Starts speed hack detection with specified callback.
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="callback">Method to call after detection.</param>
		public static void StartDetection(Action callback)
		{
			StartDetection(callback, GetOrCreateInstance.interval);
		}

		/// <summary>
		/// Starts speed hack detection with specified callback using passed interval.<br/>
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="interval">Time in seconds between speed hack checks. Overrides #interval property.</param>
		public static void StartDetection(Action callback, float interval)
		{
			StartDetection(callback, interval, GetOrCreateInstance.maxFalsePositives);
		}

		/// <summary>
		/// Starts speed hack detection with specified callback using passed interval and maxFalsePositives.<br/>
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="interval">Time in seconds between speed hack checks. Overrides #interval property.</param>
		/// <param name="maxFalsePositives">Amount of possible false positives. Overrides #maxFalsePositives property.</param>
		public static void StartDetection(Action callback, float interval, byte maxFalsePositives)
		{
			StartDetection(callback, interval, maxFalsePositives, GetOrCreateInstance.coolDown);
		}

		/// <summary>
		/// Starts speed hack detection with specified callback using passed interval, maxFalsePositives and coolDown. 
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="callback">Method to call after detection.</param>
		/// <param name="interval">Time in seconds between speed hack checks. Overrides #interval property.</param>
		/// <param name="maxFalsePositives">Amount of possible false positives. Overrides #maxFalsePositives property.</param>
		/// <param name="coolDown">Amount of sequential successful checks before resetting false positives counter. Overrides #coolDown property.</param>
		public static void StartDetection(Action callback, float interval, byte maxFalsePositives, int coolDown)
		{
			GetOrCreateInstance.StartDetectionInternal(callback, interval, maxFalsePositives, coolDown);
		}

		/// <summary>
		/// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			if (Instance != null) Instance.StopDetectionInternal();
		}

		/// <summary>
		/// Stops and completely disposes detector component.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		public static void Dispose()
		{
			if (Instance != null) Instance.DisposeInternal();
		}
		#endregion

		#region static instance
		/// <summary>
		/// Allows reaching public properties from code.
		/// Can be null if detector does not exist in scene or if accessed at Awake phase.
		/// </summary>
		public static SpeedHackDetector Instance { get; private set; }

		private static SpeedHackDetector GetOrCreateInstance
		{
			get
			{
				if (Instance != null)
					return Instance;

				if (detectorsContainer == null)
				{
					detectorsContainer = new GameObject(ContainerName);
				}
				Instance = detectorsContainer.AddComponent<SpeedHackDetector>();
				return Instance;
			}
		}
		#endregion

		private SpeedHackDetector() { } // prevents direct instantiation

		#region unity messages
#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Awake()
		{
			instancesInScene++;
			if (Init(Instance, ComponentName))
			{
				Instance = this;
			}

			SceneManager.sceneLoaded += OnLevelWasLoadedNew;
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		protected override void OnDestroy()
		{
			base.OnDestroy();
			instancesInScene--;
		}

		private void OnLevelWasLoadedNew(Scene scene, LoadSceneMode mode)
		{
			if (instancesInScene < 2)
			{
				if (!keepAlive)
				{
					DisposeInternal();
				}
			}
			else
			{
				if (!keepAlive && Instance != this)
				{
					DisposeInternal();
				}
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				ResetStartTicks();
			}
		}

#if ACTK_EXCLUDE_OBFUSCATION
		[System.Reflection.Obfuscation(Exclude = true)]
#endif
		private void Update()
		{
			if (!isRunning)
				return;

			var reliableTicks = GetReliableTicks();
			var ticksSpentSinceLastUpdate = reliableTicks - previousTicks;

			if (ticksSpentSinceLastUpdate < 0 || ticksSpentSinceLastUpdate > TicksPerSecond)
			{
				ResetStartTicks();
				return;
			}

			previousTicks = reliableTicks;

			var intervalTicks = (long)(interval * TicksPerSecond);

			// return if configured interval is not passed yet
			if (reliableTicks - previousIntervalTicks < intervalTicks) return;

			var reliableTicksFromStart = reliableTicks - ticksOnStart;

			var vulnerableTicks = Environment.TickCount * TimeSpan.TicksPerMillisecond;
			var vulnerableTicksFromStart = vulnerableTicks - vulnerableTicksOnStart;
			var ticksCheated = Math.Abs(vulnerableTicksFromStart - reliableTicksFromStart) > Threshold;

			var vulnerableTime = Time.realtimeSinceStartup;
			var vulnerableTimeFromStart = vulnerableTime - vulnerableTimeOnStart;
			var timeCheated = Math.Abs(reliableTicksFromStart / (float)TicksPerSecond - vulnerableTimeFromStart) > ThresholdFloat;

			if (ticksCheated || timeCheated)
			{

#if ACTK_DETECTION_BACKLOGS
				Debug.LogWarning(LogPrefix + "Detection backlog:\n" +
				                 "ticksCheated: " + ticksCheated + "\n" +
				                 "timeCheated: " + timeCheated + "\n" +
				                 "vulnerableTicks: " + vulnerableTicks + "\n" +
				                 "vulnerableTicksFromStart: " + vulnerableTicksFromStart + "\n" +
				                 "reliableTicks: " + reliableTicks + "\n" +
				                 "reliableTicksFromStart: " + reliableTicksFromStart + "\n" +
				                 "vulnerableTime: " + vulnerableTime);
#endif

				currentFalsePositives++;
				if (currentFalsePositives > maxFalsePositives)
				{
#if ACTK_DEBUG_ENABLED
					Debug.LogWarning(LogPrefix + "final detection!", this);
#endif
					OnCheatingDetected();
				}
				else
				{
#if ACTK_DEBUG_ENABLED
					Debug.LogWarning(LogPrefix + "detection! Allowed false positives left: " + (maxFalsePositives - currentFalsePositives), this);
#endif
					currentCooldownShots = 0;
					ResetStartTicks();
				}
			}
			else if (currentFalsePositives > 0 && coolDown > 0)
			{
#if ACTK_DEBUG_ENABLED
				Debug.Log(LogPrefix + "success shot! Shots till cool down: " + (coolDown - currentCooldownShots), this);
#endif
				currentCooldownShots++;
				if (currentCooldownShots >= coolDown)
				{
#if ACTK_DEBUG_ENABLED
					Debug.Log(LogPrefix + "cool down!", this);
#endif
					currentFalsePositives = 0;
				}
			}

			previousIntervalTicks = reliableTicks;
		}

#endregion

		private void StartDetectionInternal(Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			if (isRunning)
			{
				Debug.LogWarning(LogPrefix + "already running!", this);
				return;
			}

			if (!enabled)
			{
				Debug.LogWarning(LogPrefix + "disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}

			if (callback != null && detectionEventHasListener)
			{
				Debug.LogWarning(LogPrefix + "has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}

			if (callback == null && !detectionEventHasListener)
			{
				Debug.LogWarning(LogPrefix + "was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				enabled = false;
				return;
			}

			CheatDetected += callback;
			interval = checkInterval;
			maxFalsePositives = falsePositives;
			coolDown = shotsTillCooldown;

			ResetStartTicks();
			currentFalsePositives = 0;
			currentCooldownShots = 0;

			started = true;
			isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null, interval, maxFalsePositives, coolDown);
		}
		
		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (Instance == this) Instance = null;
#if ACTK_ANDROID_DEVICE
			ReleaseAndroidClass();
#endif
		}

		private void ResetStartTicks()
		{
			ticksOnStart = GetReliableTicks();
			vulnerableTicksOnStart = Environment.TickCount * TimeSpan.TicksPerMillisecond;
			previousTicks = ticksOnStart;
			previousIntervalTicks = ticksOnStart;

			vulnerableTimeOnStart = Time.realtimeSinceStartup;
		}

		private long GetReliableTicks()
		{
			long ticks = 0;
#if ACTK_ANDROID_DEVICE
			ticks = TryReadTicksFromAndroidRoutine();
#endif
			if (ticks == 0) ticks = DateTime.UtcNow.Ticks;
			return ticks;
		}

#if ACTK_ANDROID_DEVICE
		private long TryReadTicksFromAndroidRoutine()
		{
			long result = 0;

			if (!androidTimeReadAttemptWasMade)
			{
				androidTimeReadAttemptWasMade = true;

				try
				{
					routinesClass = new AndroidJavaClass(RoutinesClassPath);
				}
				catch (Exception e)
				{
					Debug.LogError(LogPrefix + "Couldn't create instance of the AndroidJavaClass: " + RoutinesClassPath + " !\n" + e);
				}
			}

			if (routinesClass == null) return result;

			try
			{
				// getting time in nanoseconds from the native Android timer
				// since some random fixed and JVM initialization point
				// (it even may be a future so value could be negative)
				result = routinesClass.CallStatic<long>("GetSystemNanoTime");
				result = result / 100;
			}
			catch (Exception e)
			{
				Debug.LogError(LogPrefix + "Couldn't call static method from the Android Routines Class!\n" + e);
			}

			return result;
		}

		private void ReleaseAndroidClass()
		{
			routinesClass.Dispose();
		}
#endif
	}
}