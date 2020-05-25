// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Use the collection as a source of random selection. Picking a random item each run.
    /// </summary>
    [CommandInfo("Collection",
                 "RandomItem",
                     "Use the collection as a source of random selection. Picking a random item each run.")]
    [AddComponentMenu("")]
    public class CollectionRandom : CollectionBaseVarCommand
    {
        protected override void OnEnterInner()
        {
            collection.Value.Get(Random.Range(0, collection.Value.Count - 1), ref variableToUse);
        }
    }
}