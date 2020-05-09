// This code is part of the Fungus library (https://github.com/snozbot/fungus) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Loop over a fixed integer range, similar to a common for loop.
    /// </summary>
    [CommandInfo("Flow",
                 "Loop Range",
                 "Loop over a fixed integer range, similar to a common for loop.")]
    [CommandInfo("Flow",
                 "For",
                 "Loop over a fixed integer range, similar to a common for loop.")]
    [AddComponentMenu("")]
    public class LoopRange : Condition
    {
        [Tooltip("Starting value for the counter variable")]
        [SerializeField]
        protected IntegerData startingValue;

        [Tooltip("End value for the counter variable, exclusive")]
        [SerializeField]
        protected IntegerData endValue;

        [Tooltip("Optional int var to hold the current loop counter.")]
        [SerializeField]
        protected IntegerData counter;

        [Tooltip("Step size for the counter, how much does it go up by each loop. Default 1")]
        [SerializeField]
        protected IntegerData step = new IntegerData(1);

        #region Public members

        public override bool IsLooping { get { return true; } }

        protected override void PreEvaluate()
        {
            //if we came from the end then we are already looping, if not this is first loop so prep
            if (ParentBlock.PreviousActiveCommandIndex != endCommand.CommandIndex)
            {
                counter.Value = startingValue.Value;
            }
            else
            {
                counter.Value += (startingValue.Value <= endValue.Value ? step.Value : -step.Value);
            }
        }

        protected override bool EvaluateCondition()
        {
            return (startingValue.Value <= endValue.Value ?
                   counter.Value < endValue.Value :
                   counter.Value > endValue.Value);
        }

        protected override void OnFalse()
        {
            MoveToEnd();
        }

        public override void OnValidate()
        {
            // no infinite loops
            if (step.Value == 0)
                step.Value = 1;

            //no negative steps, we do that automatically
            step.Value = Mathf.Abs(step.Value);
        }

        public override bool HasReference(Variable variable)
        {
            return startingValue.integerRef == variable || endValue.integerRef == variable ||
                counter.integerRef == variable || step.integerRef == variable ||
                base.HasReference(variable);
        }

        #endregion
    }
}