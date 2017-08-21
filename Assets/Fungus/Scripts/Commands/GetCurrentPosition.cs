using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Gets the current Vector3 position of game object attached and return it to a variable
    /// </summary>
    [CommandInfo("Scripting", "Get Current Position", "Gets the current Vector3 position of attached gameObject and assigns it to a variable")]
    public class GetCurrentPosition : Command
    {
        #region Public members

        [VariableProperty(typeof(Vector3Variable))]
        [SerializeField]
        protected Variable currentPosition;

        public override void OnEnter()
        {
            if (currentPosition == null)
            {
                return;
            }

            (currentPosition as Vector3Variable).Value = this.gameObject.transform.position;

            Continue();
        }

        public override string GetSummary()
        {
            if (currentPosition == null)
            {
                return "Error: There is no variable currently selected";
            }

            return "Returning current position into the variable " + currentPosition.name;
        }
        #endregion
    }
}