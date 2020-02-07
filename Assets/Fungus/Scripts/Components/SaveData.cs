// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes a list of game objects to be saved for each Save Point.
    /// It knows how to encode / decode concrete game classes like Flowchart and FlowchartData.
    /// To extend the save system to handle other data types, just modify or subclass this component.
    /// </summary>
    public class SaveData : MonoBehaviour
    {
        protected const string FlowchartDataKey = "FlowchartData";

        protected const string NarrativeLogKey = "NarrativeLogData";

        [Tooltip("A list of Flowchart objects whose variables will be encoded in the save data. Boolean, Integer, Float and String variables are supported.")]
        [SerializeField] protected List<Flowchart> flowcharts = new List<Flowchart>();

        #region Public methods

        /// <summary>
        /// Encodes the objects to be saved as a list of SaveDataItems.
        /// </summary
        public virtual void Encode(List<SaveDataItem> saveDataItems)
        {
            for (int i = 0; i < flowcharts.Count; i++)
            {
                var flowchart = flowcharts[i];
                var flowchartData = FlowchartData.Encode(flowchart);

                var saveDataItem = SaveDataItem.Create(FlowchartDataKey, JsonUtility.ToJson(flowchartData));
                saveDataItems.Add(saveDataItem);

                var narrativeLogItem = SaveDataItem.Create(NarrativeLogKey, FungusManager.Instance.NarrativeLog.GetJsonHistory());
                saveDataItems.Add(narrativeLogItem);
            }
        }

        /// <summary>
        /// Decodes the loaded list of SaveDataItems to restore the saved game state.
        /// </summary>
        public virtual void Decode(List<SaveDataItem> saveDataItems)
        {
            for (int i = 0; i < saveDataItems.Count; i++)
            {
                var saveDataItem = saveDataItems[i];
                if (saveDataItem == null)
                {
                    continue;
                }

                if (saveDataItem.DataType == FlowchartDataKey)
                {
                    var flowchartData = JsonUtility.FromJson<FlowchartData>(saveDataItem.Data);
                    if (flowchartData == null)
                    {
                        Debug.LogError("Failed to decode Flowchart save data item");
                        return;
                    }

                    FlowchartData.Decode(flowchartData);
                }

                if (saveDataItem.DataType == NarrativeLogKey)
                {
                    FungusManager.Instance.NarrativeLog.LoadHistory(saveDataItem.Data);
                }
            }
        }

        #endregion
    }
}

#endif