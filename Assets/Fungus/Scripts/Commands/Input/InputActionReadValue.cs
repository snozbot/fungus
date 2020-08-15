// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if ENABLE_INPUT_SYSTEM

using UnityEngine;
using UnityEngine.InputSystem;

namespace Fungus
{
    // <summary>
    /// Store value of target InputAction in a variable
    /// </summary>
    [CommandInfo("Input",
                 "InputActionReadValue",
                 "Store value of target InputAction in a variable")]
    [AddComponentMenu("")]
    public class InputActionReadValue : Command
    {
        [SerializeField] protected InputActionReference inputAction;

        [Tooltip("Float to store the value of the GetAxis")]
        [SerializeField]
        [VariableProperty()]
        protected Variable outValue;

        public override void OnEnter()
        {
            if (outValue != null)
            {
                outValue.SetValue(inputAction.action.ReadValueAsObject());
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (inputAction == null)
                return "Error: no input action set";

            if (outValue == null)
                return "Error: no outValue set";

            return inputAction.name + " in " + outValue.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (outValue == variable)
                return true;

            return false;
        }
    }
}

#endif