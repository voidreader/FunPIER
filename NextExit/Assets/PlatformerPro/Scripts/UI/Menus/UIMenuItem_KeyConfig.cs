using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{

	/// <summary>
	/// Men uitem which is used for configuring a key.
	/// </summary>
	public class UIMenuItem_KeyConfig : UIMenuItem
	{
		/// <summary>
		/// The type of key to set.
		/// </summary>
		[Tooltip ("The type of key to set.")]
		public KeyType keyType;

		/// <summary>
		/// The index of the action key. Ignored if this is not configuration for an action key.
		/// </summary>
		[Tooltip ("The index of the action key. Ignored if this is not configuration for an action key.")]
		public int actionKeyNumber;

		/// <summary>
		/// THe input being configured.
		/// </summary>
		[Tooltip ("The input being configured.")]
		public Input input;

		/// <summary>
		/// True if we are accepting input.
		/// </summary>
		protected bool checkingForKey;

		/// <summary>
		/// Stores if up left was -ve or +ve.
		/// </summary>
		protected float upLeftValue;

		///<summary>
		/// Only check for key when this is zero or lower. Used to ensure the key that selects
		/// the menu item doesn't also trigger the key check.
		/// </summary>
		protected float checkForKeyDelayTimer;

		/// <summary>
		/// Cached menu reference.
		/// </summary>
		protected UIBasicMenu menu;

		/// <summary>
		/// Cache a copy of the switch input (or null if none found).
		/// </summary>
		protected UIMenuItem_SwitchInputType switchInputMenuItem;

		/// <summary>
		/// Cached reference to the escape handler.
		/// </summary>
		protected EscapeKeyHandler escHandler;

		///<summary>
		/// How long to wait before accepting input during check key.
		/// </summary>
		protected static float checkForKeyDelayTime = 0.1f;

		/// <summary>
		/// List of all joystick axes.
		/// </summary>
		protected static string[] axisList = new string[]{
						"Joystick1Axis1","Joystick1Axis2","Joystick1Axis3","Joystick1Axis4",
						"Joystick1Axis5","Joystick1Axis6","Joystick1Axis7","Joystick1Axis8",
						"Joystick2Axis1","Joystick2Axis2","Joystick2Axis3","Joystick2Axis4",
						"Joystick2Axis5","Joystick2Axis6","Joystick2Axis7","Joystick2Axis8"};

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			switchInputMenuItem = transform.parent.GetComponentInChildren<UIMenuItem_SwitchInputType> ();
			menu = gameObject.GetComponentInParent<UIBasicMenu>();
			if (input == null) input = (Input) GameObject.FindObjectOfType (typeof(Input));
			escHandler = GameObject.FindObjectOfType<EscapeKeyHandler>();
		}

		/// <summary>
		/// Gets the title.
		/// </summary>
		/// <value>The title.</value>
		override public string Title
		{
			get
			{
				if (checkingForKey && keyType == KeyType.HORIZONTAL_AXIS) return (upLeftValue == 0) ? " > Hold Left" : " > Hold Right";
				if (checkingForKey && keyType == KeyType.VERTICAL_AXIS) return (upLeftValue == 0) ? " > Hold Down" : " > Hold Up";
				return title;
			}
		}

		/// <summary>
		/// Gets additional info to display.
		/// </summary>
		/// <value>The extra info.</value>
		override public string ExtraInfo
		{
			get
			{
				if (action == UIMenuAction.RESTORE_DEFAULT_KEYS) return "";
				if (keyType == KeyType.HORIZONTAL_AXIS || keyType == KeyType.VERTICAL_AXIS ) 
				{
					if (checkingForKey) return "";
					return input.GetAxisForType(keyType);
				}
				if (checkingForKey) return "?";

				return input.GetKeyForType(keyType, actionKeyNumber).ToString();
			}
		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (checkingForKey) {
				// Escape should always stop us
				if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetKey(KeyCode.Escape))
				{
					checkingForKey = false;
					if (menu != null)
					{
						menu.Refresh();
						menu.AcceptingInput = true;
					}
					if (escHandler != null) escHandler.enabled = true;
				}
				else
				{
					if (checkForKeyDelayTimer <= 0) {
						if (keyType == KeyType.VERTICAL_AXIS || keyType == KeyType.HORIZONTAL_AXIS) CheckForAxis();
						else CheckForKey();
					}
					else checkForKeyDelayTimer -= Time.deltaTime;
				}
			}
		}
		
		/// <summary>
		/// Indicates if this menu item should be active.
		/// </summary>
		override public bool IsActive
		{
			get
			{
				if (input is StandardInput)
				{
					if ((keyType == KeyType.HORIZONTAL_AXIS || keyType == KeyType.VERTICAL_AXIS) &&
					    !((StandardInput)input).enableController) return false;
					if ((keyType == KeyType.UP || keyType == KeyType.DOWN || keyType == KeyType.LEFT  || keyType == KeyType.RIGHT ) &&
					    !((StandardInput)input).enableKeyboard) return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Does the action.
		/// </summary>
		override public void DoAction()
		{

			if (action == UIMenuAction.CONFIGURE_KEY)
			{
				upLeftValue = 0.0f;
				checkingForKey = true;
				checkForKeyDelayTimer = checkForKeyDelayTime;
				if (menu != null) menu.AcceptingInput = false;
				if (switchInputMenuItem != null) switchInputMenuItem.SetToCustom();

			}
			else if (action == UIMenuAction.RESTORE_DEFAULT_KEYS)
	        {
				input.LoadInputData(supportingString);
				input.SaveInputData();
				if (menu != null) menu.Refresh();
			}
			else
			{
				Debug.LogError ("UIMenuItem_KeyConfig can only support the CONFIGURE_KEY and RESTORE_DEFAULT_KEYS actions.");
			}
		}

		/// <summary>
		/// Checks for key presses and assigns them to type when found.
		/// </summary>
		virtual protected void CheckForKey()
		{
			foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
			{
				if (UnityEngine.Input.GetKeyDown(code))
				{
					if (input.SetKey(keyType, code, actionKeyNumber))
					{
						input.SaveInputData();
					}
					checkingForKey = false;
					if (menu != null)
					{
						menu.Refresh();
						menu.AcceptingInput = true;
					}
					if (escHandler != null) escHandler.enabled = true;
					break;
				}
			}
		}

		/// <summary>
		/// Checks for joystick axis' and assigns to input.
		/// </summary>
		virtual protected void CheckForAxis()
		{
			float thresholdValue = 1;
			if (input is StandardInput)
			{
				if (keyType == KeyType.HORIZONTAL_AXIS &&((StandardInput)input).digitalHorizontalThreshold < thresholdValue)
				{
					thresholdValue = ((StandardInput)input).digitalHorizontalThreshold;
				}
				else if (keyType == KeyType.VERTICAL_AXIS &&((StandardInput)input).digitalVerticalThreshold < thresholdValue)
				{
					thresholdValue = ((StandardInput)input).digitalVerticalThreshold;
				}
			}
			for (int i = 0; i < axisList.Length; i++)
			{
				float val = UnityEngine.Input.GetAxis (axisList[i]);
				if (upLeftValue == 0.0f)
				{
					if (val >= thresholdValue || val <= -thresholdValue)
					{
						upLeftValue = val;
						if (menu != null) menu.Refresh();
					}
				}
				else
				{
					// Ensure the opposite dir is pressed
					if ((upLeftValue > 0 && val <= -thresholdValue) || (upLeftValue < 0 && val >= thresholdValue))
					{
						checkingForKey = false;
						if (escHandler != null) escHandler.enabled = true;
						input.SetAxis(keyType, axisList[i], (val < 0));
						input.SaveInputData();
						if (menu != null)
						{
							menu.Refresh();
							menu.AcceptingInput = true;menu.Refresh();
						}
					}
				}
			}
		}
	}
}
