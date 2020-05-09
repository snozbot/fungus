// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Store Input.GetKey in a variable. Supports an optional Negative key input. A negative value will be overridden by a positive one, they do not add.
    /// </summary>
    [CommandInfo("Input",
                 "GetKey",
                 "Store Input.GetKey in a variable. Supports an optional Negative key input. A negative value will be overridden by a positive one, they do not add.")]
    [AddComponentMenu("")]
    public class GetKey : Command
    {
        [SerializeField]
        protected KeyCode keyCode = KeyCode.None;

        [Tooltip("Optional, secondary or negative keycode. For booleans will also set to true, for int and float will set to -1.")]
        [SerializeField]
        protected KeyCode keyCodeNegative = KeyCode.None;

        [SerializeField]
        [Tooltip("Only used if KeyCode is KeyCode.None, expects a name of the key to use.")]
        protected StringData keyCodeName = new StringData(string.Empty);

        [SerializeField]
        [Tooltip("Optional, secondary or negative keycode. For booleans will also set to true, for int and float will set to -1." +
            "Only used if KeyCode is KeyCode.None, expects a name of the key to use.")]
        protected StringData keyCodeNameNegative = new StringData(string.Empty);

        public enum InputKeyQueryType
        {
            Down,
            Up,
            State
        }

        [Tooltip("Do we want an Input.GetKeyDown, GetKeyUp or GetKey")]
        [SerializeField]
        protected InputKeyQueryType keyQueryType = InputKeyQueryType.State;

        [Tooltip("Will store true or false or 0 or 1 depending on type. Sets true or -1 for negative key values.")]
        [SerializeField]
        [VariableProperty(typeof(FloatVariable), typeof(BooleanVariable), typeof(IntegerVariable))]
        protected Variable outValue;

        public override void OnEnter()
        {
            FillOutValue(0);

            if (keyCodeNegative != KeyCode.None)
            {
                DoKeyCode(keyCodeNegative, -1);
            }
            else if (!string.IsNullOrEmpty(keyCodeNameNegative))
            {
                DoKeyName(keyCodeNameNegative, -1);
            }


            if (keyCode != KeyCode.None)
            {
                DoKeyCode(keyCode, 1);
            }
            else if (!string.IsNullOrEmpty(keyCodeName))
            {
                DoKeyName(keyCodeName, 1);
            }

            Continue();
        }

        private void DoKeyCode(KeyCode key, int trueVal)
        {
            switch (keyQueryType)
            {
                case InputKeyQueryType.Down:
                    if (Input.GetKeyDown(key))
                    {
                        FillOutValue(trueVal);
                    }
                    break;
                case InputKeyQueryType.Up:
                    if (Input.GetKeyUp(key))
                    {
                        FillOutValue(trueVal);
                    }
                    break;
                case InputKeyQueryType.State:
                    if (Input.GetKey(key))
                    {
                        FillOutValue(trueVal);
                    }
                    break;
                default:
                    break;
            }
        }

        private void DoKeyName(string key, int trueVal)
        {
            switch (keyQueryType)
            {
                case InputKeyQueryType.Down:
                    if (Input.GetKeyDown(key))
                    {
                        FillOutValue(trueVal);
                    }
                    break;
                case InputKeyQueryType.Up:
                    if (Input.GetKeyUp(key))
                    {
                        FillOutValue(trueVal);
                    }
                    break;
                case InputKeyQueryType.State:
                    if (Input.GetKey(key))
                    {
                        FillOutValue(trueVal);
                    }
                    break;
                default:
                    break;
            }
        }

        private void FillOutValue(int v)
        {
            FloatVariable fvar = outValue as FloatVariable;
            if (fvar != null)
            {
                fvar.Value = v;
                return;
            }

            BooleanVariable bvar = outValue as BooleanVariable;
            if (bvar != null)
            {
                bvar.Value = v == 0 ? false : true;
                return;
            }

            IntegerVariable ivar = outValue as IntegerVariable;
            if (ivar != null)
            {
                ivar.Value = v;
                return;
            }
        }

        public override string GetSummary()
        {
            if (outValue == null)
            {
                return "Error: no outvalue set";
            }

            return (keyCode != KeyCode.None ? keyCode.ToString() : keyCodeName) + " in " + outValue.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (keyCodeName.stringRef == variable || outValue == variable || keyCodeNameNegative.stringRef == variable)
                return true;

            return false;
        }

    }
}