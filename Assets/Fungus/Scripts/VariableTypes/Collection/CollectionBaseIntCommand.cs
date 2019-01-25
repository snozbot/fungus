using UnityEngine;

namespace Fungus
{
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