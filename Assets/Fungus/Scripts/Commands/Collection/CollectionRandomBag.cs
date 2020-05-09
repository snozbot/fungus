// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Use the collection as a source of random items and turn it into a random bag. Drawing the 
    /// next random item until out of items and then reshuffling them.
    /// </summary>
    [CommandInfo("Collection",
                 "RandomBag",
                     "Use the collection as a source of random items and turn it into a random bag. " +
                         "Drawing the next random item until out of items and then reshuffling them.")]
    [AddComponentMenu("")]
    public class CollectionRandomBag : CollectionBaseVarCommand
    {
        [SerializeField]
        [Tooltip("Will add this many copies to the bag. If you want 5 of everything, you want 4 copies.")]
        protected IntegerData duplicatesToPutInBag = new IntegerData(0);

        [SerializeField]
        protected IntegerData currentIndex = new IntegerData(int.MaxValue);

        protected bool isInit = false;

        protected override void OnEnterInner()
        {
            if (!isInit)
            {
                Init();
            }

            currentIndex.Value++;

            if (currentIndex.Value >= collection.Value.Count)
            {
                Reshuffle();
            }

            collection.Value.Get(currentIndex.Value, ref variableToUse);
        }

        protected void Init()
        {
            var startingCount = collection.Value.Count;
            for (int i = 0; i < duplicatesToPutInBag.Value; i++)
            {
                for (int j = 0; j < startingCount; j++)
                {
                    collection.Value.Add(collection.Value.Get(j));
                }
            }

            //force invalid index
            currentIndex.Value = collection.Value.Count;

            isInit = true;
        }

        protected void Reshuffle()
        {
            currentIndex.Value = 0;
            collection.Value.Shuffle();
        }

        public override bool HasReference(Variable variable)
        {
            return base.HasReference(variable) || duplicatesToPutInBag.integerRef == variable || currentIndex.integerRef;
        }

        public override string GetSummary()
        {
            return base.GetSummary() +
                (duplicatesToPutInBag.integerRef != null ? " " + duplicatesToPutInBag.integerRef.Key : "") +
            (currentIndex.integerRef != null ? " " + currentIndex.integerRef.Key : ""); ;
        }
    }
}