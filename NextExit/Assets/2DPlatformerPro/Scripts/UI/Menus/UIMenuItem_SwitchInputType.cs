using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// An item in a menu.
	/// </summary>
	public class UIMenuItem_SwitchInputType : UIMenuItem
	{
		
		/// <summary>
		/// A list of default inputs.
		/// </summary>
		[Tooltip ("Associate a human readable name to a pre-defined input resource file (XML).")]
		public List<NameAndPref> inputs;

		/// <summary>
		/// The input being configured.
		/// </summary>
		[Tooltip ("The input being configured.")]
		public Input input;

		/// <summary>
		/// Current position in the inputs array or -1 for custom.
		/// </summary>
		protected int currentListPosition;

		/// <summary>
		/// The name of the currently loaded input in player prefs.
		/// </summary>
		protected const string StaticCurrentInputPlayerPrefName = "PP.CurrentInput";

		virtual protected string CurrentInputPlayerPrefName
		{
			get
			{
				if (input is StandardInput) return StaticCurrentInputPlayerPrefName + ((StandardInput)input).dataToLoad;
				return StaticCurrentInputPlayerPrefName;
			}
		}

		/// <summary>
		/// Unity Awake() hook.
		/// </summary>
		void Awake() 
		{
			currentListPosition = -1;
			if (inputs == null || inputs.Count < 1)
			{
				Debug.LogWarning("You are using an input switcher but there are no pre-configured inputs to switch between.");
			}
			else
			{
				string currentlyLoadedItemName = PlayerPrefs.GetString(CurrentInputPlayerPrefName, inputs[0].name);
				for (int i= 0; i < inputs.Count; i++) if (inputs[i].name == currentlyLoadedItemName) currentListPosition = i;
			}
		}

		/// <summary>
		/// Gets the title.
		/// </summary>
		override public string Title
		{
			get
			{ 
				if (currentListPosition == -1) 
				{
					if (input is StandardInput) 
					{
						if (((StandardInput)input).enableController) return "Custom (Controller)";
						if (((StandardInput)input).enableKeyboard) return "Custom (Keyboard)";
					}
					return "Custom";
				}
				return inputs[currentListPosition].name;
			}
		}

		/// <summary>
		/// Hitting the action key does nothing for this menu item type.
		/// </summary>
		override public void DoAction()
		{
			// Do nothing
		}

		/// <summary>
		/// Do the action for when the user presses right.
		/// </summary>
		override public void DoRightAction()
		{
			currentListPosition++;
			if (currentListPosition >= inputs.Count) currentListPosition = 0;
			LoadInput(inputs[currentListPosition]);
			UIBasicMenu menu = gameObject.GetComponentInParent<UIBasicMenu>();
			if (menu != null) menu.Refresh();
		}

		/// <summary>
		/// Do the action for when the user presses left.
		/// </summary>
		override public void DoLeftAction()
		{
			currentListPosition--;
			if (currentListPosition < 0) currentListPosition = inputs.Count -1;
			LoadInput(inputs[currentListPosition]);
			UIBasicMenu menu = gameObject.GetComponentInParent<UIBasicMenu>();
			if (menu != null) menu.Refresh();
		}

		/// <summary>
		/// Set contorls to custom as the user has changed them.
		/// </summary>
		public void SetToCustom()
		{
			currentListPosition = -1;
			PlayerPrefs.SetString(CurrentInputPlayerPrefName, Title);
			UIBasicMenu menu = gameObject.GetComponentInParent<UIBasicMenu>();
			if (menu != null) menu.Refresh();
		}

		/// <summary>
		/// Loads the given input file form a Unity resource.
		/// </summary>
		/// <param name="nameAndPref">Menu item anme and resource name.</param>
		protected void LoadInput(NameAndPref nameAndPref)
		{
			StandardInputData data = StandardInputData.LoadFromResource (nameAndPref.resourceName);
			if (data != null)
			{
				input.LoadInputData (data);
				input.SaveInputData ();
				PlayerPrefs.SetString(CurrentInputPlayerPrefName, nameAndPref.name);
			} 
			else
			{
				Debug.LogError("Tried to load input from a resource but LoadFromResource() returned null");
			}
			UIBasicMenu menu = gameObject.GetComponentInParent<UIBasicMenu>();
			if (menu != null) menu.Refresh();
		}
	}

	/// <summary>
	/// Associates a human readable name to an input XML resource name.
	/// </summary>
	[System.Serializable]
	public class NameAndPref
	{
		/// <summary>
		/// Human readable name of the input.
		/// </summary>
		public string name;

		/// <summary>
		/// The input component.
		/// </summary>
		public string resourceName;
	}

}