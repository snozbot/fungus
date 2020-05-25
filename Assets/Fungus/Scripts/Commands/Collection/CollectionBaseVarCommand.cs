// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for all FungusCollection commands that require a compatible variable type
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CollectionBaseVarCommand : CollectionBaseCommand, ICollectionCompatible
    {
        [SerializeField]
        [VariableProperty(compatibleVariableName = "collection")]
        protected Variable variableToUse;

        public override void OnEnter()
        {
            if (collection.Value != null && variableToUse != null)
            {
                OnEnterInner();
            }

            Continue();
        }

        protected abstract void OnEnterInner();

        public override bool HasReference(Variable variable)
        {
            return variable == variableToUse || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            if (variableToUse == null)
                return "Error: no variable selected";

            return variableToUse.Key + " to " + collection.Value.name;
        }

        bool ICollectionCompatible.IsVarCompatibleWithCollection(Variable variable, string compatibleWith)
        {
            if (compatibleWith == "collection")
                return collection.Value == null ? false : collection.Value.IsElementCompatible(variable);
            else
                return true;
        }
    }
}