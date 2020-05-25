// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Randomly reorders all elements of a target collection
    /// </summary>
    [CommandInfo("Collection",
                 "Shuffle",
                     "Randomly reorders all elements of a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandShuffle : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Shuffle();
            }

            Continue();
        }
    }
}