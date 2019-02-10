using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Capacity",
                    "Capacity of the collection, different from its currnet count")]
    [AddComponentMenu("")]
    public class CollectionCommandCapacity : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.RemoveAt(integer.Value);
        }
    }
}