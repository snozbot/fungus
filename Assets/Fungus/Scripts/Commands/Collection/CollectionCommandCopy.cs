// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Clears target and then adds all of rhs to target.
    /// </summary>
    [CommandInfo("Collection",
                 "Copy",
                     "Clears target and then adds all of rhs to target.")]
    [AddComponentMenu("")]
    public class CollectionCommandCopy : CollectionBaseTwoCollectionCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.CopyFrom(rhsCollection.Value);
        }
    }
}