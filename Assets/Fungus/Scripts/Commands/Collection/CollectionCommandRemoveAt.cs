// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Remove item at given index
    /// </summary>
    [CommandInfo("Collection",
                 "Remove At",
                     "Remove item at given index")]
    [AddComponentMenu("")]
    public class CollectionCommandRemoveAt : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.RemoveAt(integer.Value);
        }
    }
}