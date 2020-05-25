// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Find an item in a collection
    /// </summary>
    [CommandInfo("Collection",
                 "Find",
                     "Find an item in a collection")]
    [CommandInfo("Collection",
                 "IndexOf",
                     "Find an item in a collection")]
    [AddComponentMenu("")]
    public class CollectionCommandFind : CollectionBaseVarAndIntCommand
    {
        [Tooltip("If true, will find the last occurance rather than first occurance.")]
        [SerializeField]
        protected BooleanData lastInsteadOfFirst = new BooleanData(false);

        protected override void OnEnterInner()
        {
            integer.Value = !lastInsteadOfFirst.Value ?
                collection.Value.IndexOf(variableToUse)
                : collection.Value.LastIndexOf(variableToUse);
        }

        public override bool HasReference(Variable variable)
        {
            return lastInsteadOfFirst.booleanRef == variable || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            return base.GetSummary() + (lastInsteadOfFirst.Value ? " Last" : "");
        }
    }
}