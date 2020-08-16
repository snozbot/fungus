// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// </summary>
    [System.Serializable]
    public class ValueTypeCollectionData
    {
        public List<FlowchartData.StringPair> nameToContentsPairs = new List<FlowchartData.StringPair>();

        /// <summary>
        /// </summary>
        public static ValueTypeCollectionData Encode(Collection[] collections)
        {
            var vtcd = new ValueTypeCollectionData();

            foreach (var item in collections)
            {
                if (item.IsSerializable)
                {
                    vtcd.nameToContentsPairs.Add(new FlowchartData.StringPair()
                    {
                        key = item.name,
                        val = item.GetStringifiedValue()
                    });
                }
            }

            return vtcd;
        }

        /// <summary>
        /// Recreate global vars from previous serialise and restore value.
        /// </summary>
        public void Decode(Collection[] collections)
        {
            foreach (var item in nameToContentsPairs)
            {
                var v = collections.First(x => x.Name == item.key);
                v.RestoreFromStringifiedValue(item.val);
            }
        }
    }
}