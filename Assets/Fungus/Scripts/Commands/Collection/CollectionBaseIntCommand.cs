// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for all FungusCollection commands that use an intvar
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CollectionBaseIntCommand : CollectionBaseCommand
    {
        [SerializeField]
        protected IntegerData integer;

        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                OnEnterInner();
            }

            Continue();
        }

        protected abstract void OnEnterInner();

        public override bool HasReference(Variable variable)
        {
            return variable == integer.integerRef || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            return integer.Value.ToString() + " on " + collection.Value.name;
        }
    }
}