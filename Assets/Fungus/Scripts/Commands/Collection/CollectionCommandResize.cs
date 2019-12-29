using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                 "Resize",
                     "Resize will grow the collection to be the given size, will not remove items to shrink")]
    [AddComponentMenu("")]
    public class CollectionCommandResize : CollectionBaseIntCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Resize(integer.Value);
        }
    }
}