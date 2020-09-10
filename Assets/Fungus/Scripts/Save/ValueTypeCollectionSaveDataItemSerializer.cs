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

        public SaveDataItem[] Encode()
        {
            var data = new ValueTypeCollectionData();

            foreach (var item in collectionsToSerialize)
            {
                if (item.IsSerializable)
                {
                    data.nameToContentsPairs.Add(new StringPair()
                    {
                        key = item.name,
                        val = item.GetStringifiedValue()
                    });
                }
            }

            return SaveDataItemUtility.CreateSingleElement(DataTypeKey, data);
        }

        public bool Decode(SaveDataItem sdi)
        {
            var vtcd = JsonUtility.FromJson<ValueTypeCollectionData>(sdi.Data);
            if (vtcd == null)
            {
                Debug.LogError("Failed to decode ValueTypeCollection save data item");
                return false;
            }

            foreach (var item in vtcd.nameToContentsPairs)
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