// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Remove all items from collection that aren't also in RHS, similar to an overlap.
    /// </summary>
    [CommandInfo("Collection",
                 "Intersection",
                     "Remove all items from collection that aren't also in RHS, similar to an overlap.")]
    [AddComponentMenu("")]
    public class CollectionCommandIntersection : CollectionBaseTwoCollectionCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Intersection(rhsCollection.Value);
        }
    }
}