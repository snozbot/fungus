using UnityEngine;

namespace Fungus
{
    [CommandInfo("Collection",
                    "Sort",
                    "Sort a target collection")]
    [AddComponentMenu("")]
    public class CollectionCommandSort : CollectionBaseCommand
    {
        public override void OnEnter()
        {
            if (collection.Value != null)
            {
                collection.Value.Sort();
            }

            Continue();
        }
    }
}