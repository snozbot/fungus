using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                 "Contains",
                     "Does the collection contain the given variable")]
    [AddComponentMenu("")]
    public class CollectionCommandContains : CollectionBaseVarCommand
    {
        [VariableProperty(typeof(BooleanVariable))]
        protected BooleanVariable result;

        protected override void OnEnterInner()
        {
            if (result == null)
            {
                Debug.LogWarning("No result var set");
            }
            else
            {
                result.Value = collection.Value.Contains(variableToUse);
            }
        }

        public override bool HasReference(Variable variable)
        {
            return result == variable || base.HasReference(variable);
        }
    }
}