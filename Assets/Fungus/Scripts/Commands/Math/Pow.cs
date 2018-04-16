using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Raise a value to the power of another
    /// </summary>
    [CommandInfo("Math",
                 "Pow",
                 "Raise a value to the power of another.")]
    [AddComponentMenu("")]
    public class Pow : Command
    {
        [SerializeField]
        protected FloatData baseValue, exponentValue;

        [Tooltip("Where the result of the function is stored.")]
        [SerializeField]
        protected FloatData outValue;

        public override void OnEnter()
        {
            outValue.Value = Mathf.Pow(baseValue.Value, exponentValue.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return "Pow";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

    }
}