using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Contains Any",
                    "Does target collection, contain any of the items in the rhs collection items")]
    [AddComponentMenu("")]
    public class CollectionCommandContainsAny : CollectionBase2ColCommand
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
                result.Value = collection.Value.ContainsAnyOf(rhsCollection);
            }
        }

        public override bool HasReference(Variable variable)
        {
            return result == variable || base.HasReference(variable);
        }
    }
}