// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// How many occurrences of a given variable exist in a target collection
    /// </summary>
    [CommandInfo("Collection",
                 "Occurrences",
                     "How many occurrences of a given variable exist in a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandOccurrences : CollectionBaseVarAndIntCommand
    {
        protected override void OnEnterInner()
        {
            integer.Value = collection.Value.Occurrences(variableToUse);
        }
    }
}