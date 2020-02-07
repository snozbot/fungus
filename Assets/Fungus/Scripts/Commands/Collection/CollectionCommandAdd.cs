// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Add an item to a collection
    /// </summary>
    [CommandInfo("Collection",
                 "Add",
                     "Add an item to a collection")]
    [AddComponentMenu("")]
    public class CollectionCommandAdd : CollectionBaseVarCommand
    {
        [Tooltip("Only add if the item does not already exist in the collection")]
        [SerializeField]
        protected BooleanData onlyIfUnique = new BooleanData(false);

        protected override void OnEnterInner()
        {
            if (onlyIfUnique.Value)
                collection.Value.AddUnique(variableToUse);
            else
                collection.Value.Add(variableToUse);
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