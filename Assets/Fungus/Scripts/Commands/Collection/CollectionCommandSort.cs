// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sort a target collection
    /// </summary>
    [CommandInfo("Collection",
                 "Sort",
                     "Sort a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandSort : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Sort();
            }

            Continue();
        }
    }
}