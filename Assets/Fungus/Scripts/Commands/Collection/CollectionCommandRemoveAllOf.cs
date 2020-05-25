// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Remove all items in given rhs collection to target collection
    /// </summary>
    [CommandInfo("Collection",
                 "Remove All Of",
                     "Remove all items in given rhs collection to target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandRemoveAllOf : CollectionBaseTwoCollectionCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.RemoveAll(rhsCollection);
        }
    }
}