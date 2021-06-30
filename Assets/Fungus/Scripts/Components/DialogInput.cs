// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
    /// <summary>
    /// Supported modes for clicking through a Say Dialog.
    /// </summary>
    public enum ClickMode
    {
        /// <summary> Clicking disabled. </summary>
        Disabled,
        /// <summary> Click anywhere on screen to advance. </summary>
        ClickAnywhere,
        /// <summary> Click anywhere on Say Dialog to advance. </summary>
        ClickOnDialog,
        /// <summary> Click on continue button to advance. </summary>
        ClickOnButton
    }

    /// <summary>
    /// Input handler for say dialogs.
    /// </summary>
    public class DialogInput : MonoBehaviour
    {
        [Tooltip("Click to advance story")]
        [SerializeField] protected ClickMode clickMode;

        [Tooltip("Delay between consecutive clicks. Useful to prevent accidentally clicking through story.")]
        [SerializeField] protected float nextClickDelay = 0f;

        [Tooltip("Allow holding Cancel to fast forward text")]
        [SerializeField] protected bool cancelEnabled = true;

        [Tooltip("Ignore input if a Menu dialog is currently active")]
        [SerializeField] protected bool ignoreMenuClicks = true;

        protected bool dialogClickedFlag;

        protected bool nextLineInputFlag;

        protected float ignoreClickTimer;

#if ENABLE_INPUT_SYSTEM
        protected UnityEngine.InputSystem.UI.InputSystemUIInputModule inputSystemUIInputModule;
#else
        protected StandaloneInputModule currentStandaloneInputModule;
#endif

        protected Writer writer;

        protected virtual void Awake()
        {
            writer = GetComponent<Writer>();

            CheckEventSystem();
        }

        // There must be an Event System in the scene for Say and Menu input to work.
        // This method will automatically instantiate one if none exists.
        protected virtual void CheckEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                // Auto spawn an Event System from the prefab
                GameObject prefab = Resources.Load<GameObject>(FungusConstants.EventSystemPrefabName);
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    go.name = "EventSystem";
                }
            }
        }
            
        protected virtual void Update()
        {
            if (EventSystem.current == null)
            {
                return;
            }

#if ENABLE_INPUT_SYSTEM
            if(inputSystemUIInputModule == null)
            {
                inputSystemUIInputModule = FindObjectOfType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
            
            if (writer != null && writer.IsWriting)
            {
                if (inputSystemUIInputModule.submit.action.triggered ||
                    (cancelEnabled && inputSystemUIInputModule.cancel.action.triggered))
                {
                    SetNextLineFlag();
                }
            }
#else
            if (currentStandaloneInputModule == null)
            {
                currentStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            }

            if (writer != null)
            {
                if (Input.GetButtonDown(currentStandaloneInputModule.submitButton) ||
                    (cancelEnabled && Input.GetButton(currentStandaloneInputModule.cancelButton)))
                {
                    SetNextLineFlag();
                }
            }
#endif

            switch (clickMode)
            {
            case ClickMode.Disabled:
                break;
            case ClickMode.ClickAnywhere:
#if ENABLE_INPUT_SYSTEM
                if ((UnityEngine.InputSystem.Mouse.current?.leftButton.wasPressedThisFrame ?? false) ||
                    (UnityEngine.InputSystem.Touchscreen.current?.primaryTouch?.press.wasPressedThisFrame ?? false))
#else
                if (Input.GetMouseButtonDown(0))
#endif
                {
                    SetClickAnywhereClickedFlag();
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

            if (ignoreMenuClicks)
            {
                // Ignore input events if a Menu is being displayed
                if (MenuDialog.ActiveMenuDialog != null && 
                    MenuDialog.ActiveMenuDialog.IsActive() &&
                    MenuDialog.ActiveMenuDialog.DisplayedOptionsCount > 0)
                {
                    dialogClickedFlag = false;
                    nextLineInputFlag = false;
                }
            }

            // Tell any listeners to move to the next line
            if (nextLineInputFlag)
            {
                var inputListeners = gameObject.GetComponentsInChildren<IDialogInputListener>();
                for (int i = 0; i < inputListeners.Length; i++)
                {
                    var inputListener = inputListeners[i];
                    inputListener.OnNextLineEvent();
                }
                nextLineInputFlag = false;
            }
        }

#region Public members

        /// <summary>
        /// Trigger next line input event from script.
        /// </summary>
        public virtual void SetNextLineFlag()
        {
            if(writer.IsWaitingForInput || writer.IsWriting)
            {
                nextLineInputFlag = true;
            }
        }
        /// <summary>
        /// Set the ClickAnywhere click flag.
        /// </summary>
        public virtual void SetClickAnywhereClickedFlag()
        {
            if (ignoreClickTimer > 0f)
            {
                return;
            }
            ignoreClickTimer = nextClickDelay;

            // Only applies if ClickedAnywhere is selected
            if (clickMode == ClickMode.ClickAnywhere)
            {
                SetNextLineFlag();
            }
        }
        /// <summary>
        /// Set the dialog clicked flag (usually from an Event Trigger component in the dialog UI).
        /// </summary>
        public virtual void SetDialogClickedFlag()
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

        /// <summary>
        /// Sets the button clicked flag.
        /// </summary>
        public virtual void SetButtonClickedFlag()
        {
            // Only applies if clicking is not disabled
            if (clickMode != ClickMode.Disabled)
            {
                SetNextLineFlag();
            }
        }

#endregion
    }
}
