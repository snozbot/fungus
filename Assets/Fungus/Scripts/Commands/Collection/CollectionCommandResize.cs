// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Resize will grow the collection to be the given size, will not remove items to shrink
    /// </summary>
    [CommandInfo("Collection",
                 "Resize",
                     "Resize will grow the collection to be the given size, will not remove items to shrink")]
    [AddComponentMenu("")]
    public class CollectionCommandResize : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Resize(integer.Value);
        }
    }
}