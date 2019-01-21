// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Waits for period of time before executing the next command in the block.
    /// </summary>
    [CommandInfo("Flow", 
                 "Wait", 
                 "Waits for period of time before executing the next command in the block.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Wait : Command
    {
        [Tooltip("Duration to wait for")]
        [SerializeField] protected FloatData _duration = new FloatData(1);

        protected virtual void OnWaitComplete()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            Invoke ("OnWaitComplete", _duration.Value);
        }

        public override string GetSummary()
        {
            return _duration.Value.ToString() + " seconds";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _duration.floatRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

        protected virtual void OnEnable()
        {
            if (durationOLD != default(float))
            {
                _duration.Value = durationOLD;
                durationOLD = default(float);
            }
        }

        #endregion
    }
}