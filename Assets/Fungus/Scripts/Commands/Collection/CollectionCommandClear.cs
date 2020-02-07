// This code is part of the Fungus library (http://fungusgames.com)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Clears a target collection
    /// </summary>
    [CommandInfo("Collection",
                 "Clear",
                     "Clears a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandClear : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Clear();
            }

            Continue();
        }
    }
}