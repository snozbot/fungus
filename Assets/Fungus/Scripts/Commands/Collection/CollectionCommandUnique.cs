// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Removes all duplicates.
    /// </summary>
    [CommandInfo("Collection",
                 "Unique",
                     "Removes all duplicates.")]
    [AddComponentMenu("")]
    public class CollectionCommandUnique : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Unique();
            }

            Continue();
        }
    }
}