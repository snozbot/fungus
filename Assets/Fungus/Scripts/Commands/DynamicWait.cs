// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using IEnumerator = System.Collections.IEnumerator;

namespace Fungus
{
    /// <summary>
    /// Waits for period of time before executing the next command in the block.
    /// </summary>
    [CommandInfo("Flow", 
                 "Wait (Dynamic)", 
                 "Waits for period of time (in seconds) before executing the next command in the block. Can be interrupted based on player input.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class DynamicWait : Command
    {
        [Tooltip("Duration to wait for (in seconds)")]
        [SerializeField] protected FloatData _duration = new FloatData(1);

        [Tooltip("Whether or not the wait can be cancelled")]
        [SerializeField] protected BooleanData allowWaitCancelling;

        [Tooltip("These can cancel the wait if allowWaitCancelling is true")]
        [SerializeField] protected StringData[] inputAxes;

        protected virtual void OnWaitComplete()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            if (allowWaitCancelling)
            {
                UpdateFrameInfo();
                StartCoroutine(ApplyInterruptableWait());
            }
            else
                Invoke (waitCompleteFuncName, _duration.Value);
        }

        protected static string waitCompleteFuncName = "OnWaitComplete";

        protected virtual void UpdateFrameInfo()
        {
            // When we want to allow cancelling, we need to apply the wait on a frame-based basis
            frameRate = (int)(1f / Time.deltaTime);
            framesToWait = (int)(frameRate * _duration);
            framesPassed = 0;
        }

        int frameRate, framesToWait, framesPassed;

        protected virtual IEnumerator ApplyInterruptableWait()
        {
            while (framesPassed < framesToWait)
            {
                if (this.ShouldCancelWait())
                {
                    break;
                }

                yield return null;
                framesPassed++;
            }

            OnWaitComplete();
        }

        protected virtual bool ShouldCancelWait()
        {
            // This assumes allowWaitCancelling is true
            foreach (string inputAxis in inputAxes)
            {
                if (Input.GetAxis(inputAxis) != 0)
                    return true;
            }

            return false;
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