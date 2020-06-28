// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;

namespace Fungus 
{
    /// <summary>
    /// 
    /// </summary>
    [EventHandlerInfo("Input",
                      "Input Action",
                      "")]
    [AddComponentMenu("")]
    public class InputActionEventHandler : EventHandler
    {   
        //have to use a reference as unity is preventing us from using the InputAction mapping directly
        public InputActionReference inputAction;

        public virtual void OnEnable()
        {
            inputAction.action.performed += InputAction_performed;
        }

        public virtual void OnDisable()
        {
            inputAction.action.performed -= InputAction_performed;
        }

        private void InputAction_performed(InputAction.CallbackContext obj)
        {
            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (inputAction == null)
            {
                return "Error: no InputAction set";
            }

            return inputAction.action.name;
        }
    }
}
#endif