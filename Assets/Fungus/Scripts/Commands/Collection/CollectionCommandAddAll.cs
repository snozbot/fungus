// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Add all items in given rhs collection to target collection
    /// </summary>
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