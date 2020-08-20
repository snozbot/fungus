// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Does target collection, contain any of the items in the rhs collection items
    /// </summary>
    [CommandInfo("Collection",
                 "Contains Any Of",
                     "Does target collection, contain any of the items in the rhs collection items")]
    [AddComponentMenu("")]
    public class CollectionCommandContainsAny : CollectionBaseTwoCollectionCommand
    {
        [VariableProperty(typeof(BooleanVariable))]
        [SerializeField] protected BooleanVariable result;

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