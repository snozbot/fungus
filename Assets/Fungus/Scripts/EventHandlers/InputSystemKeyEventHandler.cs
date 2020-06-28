// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

//TODO use Keyboard.current, see GetKey for more details

namespace Fungus
{
    /// <summary>
    /// 
    /// </summary>
    [EventHandlerInfo("Input",
                      "InputSystem Key",
                      "")]
    [AddComponentMenu("")]
    public class InputSystemKeyEventHandler : EventHandler
    {
        [Tooltip("Keycode of the key to activate on")]
        [SerializeField] protected Key keyCode;

        [Tooltip("True is Pressed, false is Released")]
        [SerializeField] protected bool isPressedOrReleased = true;

        protected virtual void Update()
        {
            switch (isPressedOrReleased)
            {
            case true:
                if (Keyboard.current[keyCode].wasPressedThisFrame)
                {
                    ExecuteBlock();
                }
                break;
            case false:
                if (Keyboard.current[keyCode].wasReleasedThisFrame)
                {
                    ExecuteBlock();
                }
                break;
            }
        }

        public override string GetSummary()
        {
            return keyCode.ToString() + (isPressedOrReleased ? " Pressed" : " Released");
        }
    }
}
#endif