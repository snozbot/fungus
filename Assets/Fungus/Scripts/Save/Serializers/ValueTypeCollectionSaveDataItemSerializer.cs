// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    [System.Serializable]
    /// <summary>
    /// This component encodes and decodes value type Fungus Collections, such as int, float and string.
    /// </summary>
    public class ValueTypeCollectionSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string VTCollectionKey = "VTCollectionData";
        protected const int VTCollectionDataPriority = 1500;

        public string DataTypeKey => VTCollectionKey;

        public int Order => VTCollectionDataPriority;

        public List<Collection> collectionsToSerialize = new List<Collection>();

        public StringPair[] Encode()
        {
            var data = new StringPairList();

            foreach (var item in collectionsToSerialize)
            {
                if (item.IsSerializable)
                {
                    data.Add(item.name, item.GetStringifiedValue());
                }
            }

            return SaveDataItemUtils.CreateSingleElement(DataTypeKey, data);
        }

        public bool Decode(StringPair sdi)
        {
            var vtcd = JsonUtility.FromJson<StringPairList>(sdi.val);
            if (vtcd == null)
            {
                Debug.LogError("Failed to decode ValueTypeCollection save data item");
                return false;
            }

            var col = vtcd.AsReadOnly();

            foreach (var item in col)
            {
                var v = collectionsToSerialize.First(x => x.Name == item.key);
                v.RestoreFromStringifiedValue(item.val);
            }

            return true;
        }

        public void PreDecode()
        {
        }

        public void PostDecode()
        {
        }

        [System.Serializable]
        public class ValueTypeCollectionData
        {
            [SerializeField]
            public List<StringPair> nameToContentsPairs = new List<StringPair>();
        }
    }
}
