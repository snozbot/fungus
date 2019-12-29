using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                 "Clear",
                     "Clears a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandClear : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Clear();
            }

            Continue();
        }
    }
}