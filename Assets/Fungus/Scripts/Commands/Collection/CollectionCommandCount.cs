// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Number of items in the collection
    /// </summary>
    [CommandInfo("Collection",
                 "Count",
                     "Number of items in the collection")]
    [CommandInfo("Collection",
                 "Length",
                     "Number of items in the collection")]
    [AddComponentMenu("")]
    public class CollectionCommandCount : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            integer.Value = collection.Value.Count;
        }
    }
}