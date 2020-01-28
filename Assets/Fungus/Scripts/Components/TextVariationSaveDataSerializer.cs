// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;

//TODO needs doco update
//todo savedata and children need a refactor

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes a list of game objects to be saved for each Save Point.
    /// It knows how to encode / decode concrete game classes like Flowchart and FlowchartData.
    /// To extend the save system to handle other data types, just modify or subclass this component.
    /// </summary>
    public class TextVariationSaveDataSerializer : SaveDataSerializer
    {
        [System.Serializable]
        public class TextVariationData
        {
            public List<int> history = new List<int>();
        }

        protected const string TextVarKey = "TextVariationData";

        public override string DataTypeKey => TextVarKey;

        public override void Encode(SavePointData data)
        {
            var tvdata = new TextVariationData();
            tvdata.history = TextVariationHandler.GetSerialisedHistory();

            var tvDataItem = SaveDataItem.Create(TextVarKey, JsonUtility.ToJson(tvdata));
            data.SaveDataItems.Add(tvDataItem);
        }

        public override void Decode(SavePointData data)
        {
            DecodeMatchingItem(data, ProcessItem);
        }

        protected virtual void ProcessItem(SaveDataItem item)
        {
            var tvdata = JsonUtility.FromJson<TextVariationData>(item.Data);
            if(tvdata == null)
            {
                Debug.LogError("Failed to decode Text Variation save data item");
                return;
            }

            TextVariationHandler.RestoreFromSerialisedHistory(tvdata.history);
        }
    }
}

#endif