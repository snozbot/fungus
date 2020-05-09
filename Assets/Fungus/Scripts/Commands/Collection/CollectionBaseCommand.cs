// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for all FungusCollection commands
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CollectionBaseCommand : Command
    {
        [SerializeField]
        protected CollectionData collection;

        public override Color GetButtonColor()
        {
            return new Color32(191, 217, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == collection.collectionRef;
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            return collection.Value.name;
        }
    }
}