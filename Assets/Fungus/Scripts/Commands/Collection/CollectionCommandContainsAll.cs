// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Does target collection, contain all rhs collection items
    /// </summary>
    [CommandInfo("Collection",
                 "Contains All Of",
                     "Does target collection, contain all rhs collection items")]
    [AddComponentMenu("")]
    public class CollectionCommandContainsAll : CollectionBaseTwoCollectionCommand
    {
        [Tooltip("Do they have to be in the same order?")]
        [SerializeField]
        protected BooleanData inSameOrder = new BooleanData(false);

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
                if (inSameOrder.Value)
                {
                    result.Value = collection.Value.ContainsAllOfOrdered(rhsCollection.Value);
                }
                else
                {
                    result.Value = collection.Value.ContainsAllOf(rhsCollection.Value);
                }
            }
        }

        public override bool HasReference(Variable variable)
        {
            return result == variable || inSameOrder.booleanRef == variable || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            return base.GetSummary() + (inSameOrder.Value ? " Ordered" : "");
        }
    }
}