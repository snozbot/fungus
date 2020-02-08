// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Get or Set, an element in a collection
    /// </summary>
    [CommandInfo("Collection",
                 "Element",
                     "Get or Set, an element in a collection")]
    [AddComponentMenu("")]
    public class CollectionCommandElement : CollectionBaseVarCommand
    {
        public enum GetSet
        {
            Get,
            Set,
        }

        [SerializeField]
        protected IntegerData index;

        [SerializeField]
        protected GetSet getset = GetSet.Get;

        protected override void OnEnterInner()
        {
            if (index.Value >= 0 && index.Value < collection.Value.Count)
            {
                if (getset == GetSet.Get)
                {
                    collection.Value.Get(index.Value, ref variableToUse);
                }
                else
                {
                    collection.Value.Set(index.Value, variableToUse);
                }
            }
            else
            {
                throw new System.ArgumentOutOfRangeException();
            }
        }

        public override string GetSummary()
        {
            return base.GetSummary() + " " + getset.ToString();
        }
    }
}