using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Reserve",
                    "Reserve space for given number of items in the collection")]
    [AddComponentMenu("")]
    public class CollectionCommandReserve : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Reserve(integer.Value);
        }
    }
}