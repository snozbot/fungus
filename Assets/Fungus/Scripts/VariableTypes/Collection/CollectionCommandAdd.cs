using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Add",
                    "Add an item to a collection")]
    [AddComponentMenu("")]
    public class CollectionCommandAdd : CollectionBaseCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Add(variableToUse);
        }
    }
}