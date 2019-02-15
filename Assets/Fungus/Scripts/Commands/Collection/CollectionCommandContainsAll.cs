using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Contains All",
                    "Does target collection, contain all rhs collection items")]
    [AddComponentMenu("")]
    public class CollectionCommandContainsAll : CollectionBase2ColCommand
    {
        [Tooltip("Do they have to be in the same order?")]
        [SerializeField]
        protected BooleanData inSameOrder = new BooleanData(false);

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

                if (inSameOrder.Value)
                {
                    result.Value = collection.Value.ContainsAllOfOrdered(rhsCollection);
                }
                else
                {
                    result.Value = collection.Value.ContainsAllOf(rhsCollection);
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