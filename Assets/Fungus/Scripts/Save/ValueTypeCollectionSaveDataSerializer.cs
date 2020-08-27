// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes value type Fungus Collections, such as int, float and string.
    /// </summary>
    public class ValueTypeCollectionSaveDataSerializer : SaveDataSerializer
    {
        protected const string VTCollectionKey = "VTCollectionData";
        protected const int VTCollectionDataPriority = 1500;

        public override string DataTypeKey => VTCollectionKey;

        public override int Order => VTCollectionDataPriority;

        [SerializeField] protected Collection[] collectionsToSerialize;

        public override void Encode(SavePointData data)
        {
            var block = ValueTypeCollectionData.Encode(collectionsToSerialize);
            var saveDataItem = new SaveDataItem()
            {
                DataType = VTCollectionKey,
                Data = JsonUtility.ToJson(block)
            };
            data.SaveDataItems.Add(saveDataItem);
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            var block = JsonUtility.FromJson<ValueTypeCollectionData>(item.Data);
            if (block == null)
            {
                Debug.LogError("Failed to decode ValueTypeCollection save data item");
                return;
            }

            block.Decode(collectionsToSerialize);
        }
    }
}