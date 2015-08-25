using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Fungus
{
	public interface IDialogInputListener
	{
		void OnNextLineEvent();
	}
	
	public class DialogInput : MonoBehaviour
	{
		public enum ClickMode
		{
			Disabled,			// Clicking disabled
			ClickAnywhere,		// Click anywhere on screen to advance
			ClickOnDialog,		// Click anywhere on Say Dialog to advance
			ClickOnButton		// Click on continue button to advance
		}

		public enum KeyPressMode
		{
			Disabled,		// Key pressing disabled
			AnyKey,			// Press any key to continue
			KeyPressed		// Press one of specified keys to advance
		}

		[Tooltip("Click to advance story")]
		public ClickMode clickMode;

		[Tooltip("Press a key to advance story")]
		public KeyPressMode keyPressMode;

		[Tooltip("Hold down shift while pressing a key to advance though story instantly")]
		public bool shiftKeyEnabled = true;

		[Tooltip("Delay between consecutive clicks. Useful to prevent accidentally clicking through story.")]
		public float nextClickDelay = 0f;

		[Tooltip("Keycodes to check for key presses")]
		public KeyCode[] keyList;

		protected bool dialogClickedFlag;

		protected bool nextLineInputFlag;

		protected float ignoreClickTimer;

		/**
		 * Trigger next line input event from script.
		 */
		public void SetNextLineFlag()
		{
			nextLineInputFlag = true;
		}

		/**
		 * Set the dialog clicked flag (usually from an Event Trigger component in the dialog UI)
		 */
		public void SetDialogClickedFlag()
		{
			// Ignore repeat clicks for a short time to prevent accidentally clicking through the character dialogue
			if (ignoreClickTimer > 0f)
			{
				return;
			}
			ignoreClickTimer = nextClickDelay;

			// Only applies in Click On Dialog mode
			if (clickMode == ClickMode.ClickOnDialog)
			{
				dialogClickedFlag = true;
			}
		}

		public void SetButtonClickedFlag()
		{
			// Only applies if clicking is not disabled
			if (clickMode != ClickMode.Disabled)
			{
				SetNextLineFlag();
			}
		}

		protected virtual void Update()
		{
			switch (keyPressMode)
			{
			case KeyPressMode.Disabled:
				break;
			case KeyPressMode.AnyKey:
				if (Input.anyKeyDown)
				{
					SetNextLineFlag();
				}
				break;
			case KeyPressMode.KeyPressed:
				foreach (KeyCode keyCode in keyList)
				{
					if (shiftKeyEnabled && 
					    (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
					{
						if (Input.GetKey(keyCode))
						{
							SetNextLineFlag();
						}
					}
					else
					{
						if (Input.GetKeyDown(keyCode))
						{
							SetNextLineFlag();
						}
					}
				}
				break;
			}

			switch (clickMode)
			{
			case ClickMode.Disabled:
				break;
			case ClickMode.ClickAnywhere:
				if (Input.GetMouseButtonDown(0))
				{
					SetNextLineFlag();
				}
				break;
			case ClickMode.ClickOnDialog:
				if (dialogClickedFlag)
				{
					SetNextLineFlag();
					dialogClickedFlag = false;
				}
				break;
			}

			if (ignoreClickTimer > 0f)
			{
				ignoreClickTimer = Mathf.Max (ignoreClickTimer - Time.deltaTime, 0f);
			}

			// Ignore input events if a Menu is being displayed
			if (MenuDialog.activeMenuDialog != null)
			{
				if (MenuDialog.activeMenuDialog.gameObject.activeSelf)
				{
					dialogClickedFlag = false;
					nextLineInputFlag = false;
				}
			}

			// Tell any listeners to move to the next line
			if (nextLineInputFlag)
			{
				IDialogInputListener[] inputListeners = gameObject.GetComponentsInChildren<IDialogInputListener>();
				foreach (IDialogInputListener inputListener in inputListeners)
				{
					inputListener.OnNextLineEvent();
				}
				nextLineInputFlag = false;
			}
		}
	}

}
	