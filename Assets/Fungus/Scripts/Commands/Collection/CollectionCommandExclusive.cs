using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Exclusive",
                    "Remove all items from collection that are also in RHS and add all the items in RHS that are not already in target. " +
        "Similar to an xor")]
    [AddComponentMenu("")]
    public class CollectionCommandExclusive : CollectionBase2ColCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Exclusive(rhsCollection);
        }
    }
}