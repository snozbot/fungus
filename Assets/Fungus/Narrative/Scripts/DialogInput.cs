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
			Disabled,
			ClickAnywhere,
			ClickOnDialog
		}

		public enum KeyPressMode
		{
			Disabled,
			AnyKey,
			KeyPressed
		}

		public ClickMode clickMode;

		public KeyPressMode keyPressMode;

		public float nextClickDelay = 0.2f;

		[Tooltip("Keycode of the key to activate on")]
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
					if (Input.GetKeyDown(keyCode))
					{
						SetNextLineFlag();
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
	