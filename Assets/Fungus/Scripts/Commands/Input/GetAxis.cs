using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Store Input.GetAxis in a variable
    /// </summary>
    [CommandInfo("Input",
                 "GetAxis",
                 "Store Input.GetAxis in a variable")]
    [AddComponentMenu("")]
    public class GetAxis : Command
    {
        [SerializeField]
        protected StringData axisName;

        [Tooltip("If true, calls GetAxisRaw instead of GetAxis")]
        [SerializeField]
        protected bool axisRaw = false;

        [Tooltip("Float to store the value of the GetAxis")]
        [SerializeField]
        protected FloatData outValue;

        public override void OnEnter()
        {
            if (axisRaw)
            {
                outValue.Value = Input.GetAxisRaw(axisName.Value);
            }
            else
            {
                outValue.Value = Input.GetAxis(axisName.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            return axisName + (axisRaw ? " Raw" : "");
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (axisName.stringRef == variable || outValue.floatRef == variable)
                return true;

            return false;
        }

    }
}