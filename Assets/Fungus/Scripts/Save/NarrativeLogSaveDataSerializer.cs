// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes the narrative log contents.
    /// </summary>
    public class NarrativeLogSaveDataSerializer : SaveDataSerializer
    {
        protected const string NarrativeLogKey = "NarrativeLogData";
        protected const int NarLogDataPriority = 1000;

        public override string DataTypeKey => NarrativeLogKey;
        public override int Order => NarLogDataPriority;

        public override void Encode(SavePointData data)
        {
            var narLogJSON = FungusManager.Instance.NarrativeLog.GetJsonHistory();

            var narrativeLogItem = SaveDataItem.Create(NarrativeLogKey, narLogJSON);
            data.SaveDataItems.Add(narrativeLogItem);
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            FungusManager.Instance.NarrativeLog.LoadHistory(item.Data);
        }
    }
}