// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Remove all items from collection that are also in RHS and add all the items in RHS that are not already 
    /// in target. Similar to a xor
    /// </summary>
    [CommandInfo("Collection",
                 "Exclusive",
                     "Remove all items from collection that are also in RHS and add all the items in RHS that are not already in target. " +
        "Similar to a xor")]
    [AddComponentMenu("")]
    public class CollectionCommandExclusive : CollectionBaseTwoCollectionCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Exclusive(rhsCollection.Value);
        }
    }
}