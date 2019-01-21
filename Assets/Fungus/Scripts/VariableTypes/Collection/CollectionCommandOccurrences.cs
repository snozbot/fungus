using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Occurrences",
                    "How many occurrences of a given variable exist in a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandOccurrences : CollectionBaseVarIntCommand
    {
        protected override void OnEnterInner()
        {
            integer.Value = collection.Value.Occurrences(variableToUse);
        }
    }
}