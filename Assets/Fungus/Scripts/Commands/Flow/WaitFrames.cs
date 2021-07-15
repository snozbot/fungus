// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Waits for a number of frames before executing the next command in the block.
    /// </summary>
    [CommandInfo("Flow", 
                 "Wait Frames", 
                 "Waits for a number of frames before executing the next command in the block.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class WaitFrames : Command
    {
        [Tooltip("Number of frames to wait for")]
        [SerializeField] protected IntegerData frameCount = new IntegerData(1);

        protected virtual IEnumerator WaitForFrames()
        {
            int count = frameCount.Value;
            while (count > 0)
            {
                yield return new WaitForEndOfFrame();
                count--;
            }

            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            StartCoroutine(WaitForFrames());
        }

        public override string GetSummary()
        {
            return frameCount.Value.ToString() + " frames";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return frameCount.integerRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}