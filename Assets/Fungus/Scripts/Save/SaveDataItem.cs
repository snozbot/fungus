// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// A container for a single unit of saved data.
    /// The data and its associated type are stored as string properties.
    /// The data would typically be a JSON string representing a saved object.
    /// </summary>
    [System.Serializable]
    public class SaveDataItem
    {
        public string DataType;
        public string Data;
    }

    public static class SaveDataItemUtility
    { 
        public static SaveDataItem[] CreateSingleElement<T>(string key, T dataItem)
        {
            var sdi = new SaveDataItem()
            {
                DataType = key,
                Data = UnityEngine.JsonUtility.ToJson(dataItem)
            };

            return new SaveDataItem[] { sdi };
        }
    }
}