using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Reverse",
                    "Reverse the current order of a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandReverse : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Reverse();
            }

            Continue();
        }
    }
}