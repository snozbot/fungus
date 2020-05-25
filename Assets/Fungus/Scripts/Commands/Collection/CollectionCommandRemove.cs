// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Remove an item to a collection
    /// </summary>
    [CommandInfo("Collection",
                 "Remove",
                     "Remove an item to a collection")]
    [AddComponentMenu("")]
    public class CollectionCommandRemove : CollectionBaseVarCommand
    {
        [Tooltip("Should it remove ALL occurances of variable")]
        [SerializeField]
        protected BooleanData allOccurances = new BooleanData(false);

        protected override void OnEnterInner()
        {
            if (allOccurances.Value)
                collection.Value.RemoveAll(variableToUse);
            else
                collection.Value.Remove(variableToUse);
        }

        public override bool HasReference(Variable variable)
        {
            return allOccurances.booleanRef == variable || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            return base.GetSummary() + (allOccurances.Value ? " ALL" : "");
        }
    }
}