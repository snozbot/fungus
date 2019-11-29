using UnityEngine;

namespace Fungus
{
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