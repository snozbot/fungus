using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                 "Remove At",
                     "Remove item at given index")]
    [AddComponentMenu("")]
    public class CollectionCommandRemoveAt : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.RemoveAt(integer.Value);
        }
    }
}