// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;

//TODO needs doco update

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes a list of game objects to be saved for each Save Point.
    /// It knows how to encode / decode concrete game classes like Flowchart and FlowchartData.
    /// To extend the save system to handle other data types, just modify or subclass this component.
    /// </summary>
    public class NarrativeLogSaveDataSerializer : SaveDataSerializer
    {
        protected const string NarrativeLogKey = "NarrativeLogData";

        public override string DataTypeKey => NarrativeLogKey;

        public override void Encode(SavePointData data)
        {
            var narLogJSON = FungusManager.Instance.NarrativeLog.GetJsonHistory();

            var narrativeLogItem = SaveDataItem.Create(NarrativeLogKey, narLogJSON);
            data.SaveDataItems.Add(narrativeLogItem);
        }

        public override void Decode(SavePointData data)
        {
            DecodeMatchingItem(data, ProcessItem);
        }

        protected virtual void ProcessItem(SaveDataItem item)
        {
            FungusManager.Instance.NarrativeLog.LoadHistory(item.Data);
        }
    }
}

#endif