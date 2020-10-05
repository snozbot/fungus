// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace Fungus
{
    /// <summary>
    /// Execute a block when a targeted InputAction is performed. Optionally reads the value from the action.
    /// </summary>
    [EventHandlerInfo("Input",
                      "Input Action",
                      "Execute a block when a targeted InputAction is performed. Optionally reads the value from the action.")]
    [AddComponentMenu("")]
    public class InputActionEventHandler : EventHandler
    {
        [SerializeField] protected InputActionReference inputAction;

        [VariableProperty()]
        [SerializeField] protected Variable variable;

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
            if (variable != null)
            {
                variable.SetValue(inputAction.action.ReadValueAsObject());
            }

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