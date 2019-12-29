using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                 "Contains Any Of",
                     "Does target collection, contain any of the items in the rhs collection items")]
    [AddComponentMenu("")]
    public class CollectionCommandContainsAny : CollectionBaseTwoCollectionCommand
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
                result.Value = collection.Value.ContainsAnyOf(rhsCollection.Value);
            }
        }

        public override bool HasReference(Variable variable)
        {
            return result == variable || base.HasReference(variable);
        }
    }
}