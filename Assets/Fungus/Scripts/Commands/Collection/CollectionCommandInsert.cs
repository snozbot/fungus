// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Add at a specific location in the collection
    /// </summary>
    [CommandInfo("Collection",
                 "Insert",
                     "Add at a specific location in the collection")]
    [AddComponentMenu("")]
    public class CollectionCommandInsert : CollectionBaseVarAndIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Insert(integer.Value, variableToUse);
        }
    }
}