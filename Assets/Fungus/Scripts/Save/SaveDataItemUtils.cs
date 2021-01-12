// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public static class SaveDataItemUtils
    {
        /// <summary>
        /// Often a save data item will only want to create a single element but the return
        /// of the encode expects an array, this helper hides that conversion.
        /// </summary>
        public static StringPair[] CreateSingleElement<T>(string key, T dataItem)
        {
            var sdi = new StringPair()
            {
                key = key,
                val = UnityEngine.JsonUtility.ToJson(dataItem)
            };

            return new StringPair[] { sdi };
        }
    }
}
