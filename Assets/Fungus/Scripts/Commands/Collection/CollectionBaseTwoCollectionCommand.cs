// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for all FungusCollection commands that require a second collection of the same type
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CollectionBaseTwoCollectionCommand : CollectionBaseCommand
    {
        [SerializeField]
        protected CollectionData rhsCollection;

        public override void OnEnter()
        {
            if (collection.Value != null && rhsCollection.Value != null)
            {
                OnEnterInner();
            }

            Continue();
        }

        protected abstract void OnEnterInner();

        public override bool HasReference(Variable variable)
        {
            return variable == rhsCollection.collectionRef || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            if (rhsCollection.Value == null)
                return "Error: no variable selected";

            if (collection.Value.ContainedType() != rhsCollection.Value.ContainedType())
            {
                return "Error: Collection types do not match. " + collection.Value.ContainedType().Name + " != " + rhsCollection.Value.ContainedType().Name;
            }

            return collection.Value.name + " , " + rhsCollection.Value.name;
        }
    }
}