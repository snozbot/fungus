// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    ///
    /// </summary>
    public class FungusPerProfileSaveDataItemSerializer : ISaveDataItemSerializer
    {
        protected const string FungusPerProfileKey = "FungusPerProfileData";
        protected const int DataPriority = 1000;

        public string DataTypeKey => FungusPerProfileKey;
        public int Order => DataPriority;

        public StringPair[] Encode()
        {
            var data = new FungusPerProfileSaveDataItem()
            {
                language = SetLanguage.mostRecentLanguage
            };

            return SaveDataItemUtils.CreateSingleElement(DataTypeKey, data);
        }

        public bool Decode(StringPair sdi)
        {
            var data = JsonUtility.FromJson<FungusPerProfileSaveDataItem>(sdi.val);
            if (data == null)
            {
                Debug.LogError("Failed to decode FungusPerProfileSaveDataItem");
                return false;
            }

            SetLanguage.SetActiveLanguage(data.language);

            return true;
        }

        public void PreDecode()
        {
        }

        public void PostDecode()
        {
        }

        [System.Serializable]
        public class FungusPerProfileSaveDataItem
        {
            public string language;
        }
    }
}
