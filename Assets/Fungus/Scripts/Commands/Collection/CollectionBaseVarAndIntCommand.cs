// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for all FungusCollection commands that require a compatible variable and an integer
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CollectionBaseVarAndIntCommand : CollectionBaseVarCommand
    {
        [SerializeField]
        [VariableProperty(typeof(IntegerVariable))]
        protected IntegerVariable integer;

        public override void OnEnter()
        {
            if (collection.Value != null && variableToUse != null && integer != null)
            {
                OnEnterInner();
            }

            Continue();
        }

        public override bool HasReference(Variable variable)
        {
            return variable == integer || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            if (variableToUse == null)
                return "Error: no variable selected";

            if (integer == null)
                return "Error: no integer selected";

            return integer.Key + " on " + variableToUse.Key + " in " + collection.Value.name;
        }
    }
}