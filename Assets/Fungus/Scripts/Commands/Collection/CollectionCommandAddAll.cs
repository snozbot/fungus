using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                 "Add All",
                     "Add all items in given rhs collection to target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandAddAll : CollectionBaseTwoCollectionCommand
    {
        [Tooltip("Only add if the item does not already exist in the collection")]
        [SerializeField]
        protected BooleanData onlyIfUnique = new BooleanData(false);

        protected override void OnEnterInner()
        {
            if (onlyIfUnique.Value)
                collection.Value.AddUnique(rhsCollection);
            else
                collection.Value.Add(rhsCollection);
        }

        public override bool HasReference(Variable variable)
        {
            return onlyIfUnique.booleanRef == variable || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            return base.GetSummary() + (onlyIfUnique.Value ? " Unique" : "");
        }
    }
}