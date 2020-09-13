// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public static class SaveDataItemUtils
    {
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
