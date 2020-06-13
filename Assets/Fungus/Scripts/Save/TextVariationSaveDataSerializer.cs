// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the state of the TextVariation system.
    /// </summary>
    public class TextVariationSaveDataSerializer : SaveDataSerializer
    {
        [System.Serializable]
        public class TextVariationData
        {
            public List<int> history = new List<int>();
        }

        protected const string TextVarKey = "TextVariationData";
        protected const int TextVarDataPriority = 1000;

        public override string DataTypeKey => TextVarKey;
        public override int Order => TextVarDataPriority;

        public override void Encode(SavePointData data)
        {
            var tvdata = new TextVariationData();
            tvdata.history = TextVariationHandler.GetSerialisedHistory();

            var tvDataItem = SaveDataItem.Create(TextVarKey, JsonUtility.ToJson(tvdata));
            data.SaveDataItems.Add(tvDataItem);
        }

        public override void Decode(SavePointData data)
        {
            TextVariationHandler.ClearHistory();
            base.Decode(data);
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            var tvdata = JsonUtility.FromJson<TextVariationData>(item.Data);
            if (tvdata == null)
            {
                Debug.LogError("Failed to decode Text Variation save data item");
                return;
            }

            TextVariationHandler.RestoreFromSerialisedHistory(tvdata.history);
        }
    }
}