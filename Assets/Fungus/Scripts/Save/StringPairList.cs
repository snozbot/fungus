// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fungus
{
    [System.Serializable]
    public struct StringPair
    {
        public string key, val;
    }

    [System.Serializable]
    public class StringPairList
    {
        [UnityEngine.SerializeField]
        private List<StringPair> stringPairs = new List<StringPair>();

        public ReadOnlyCollection<StringPair> AsReadOnly() => stringPairs.AsReadOnly();

        public int Count => stringPairs.Count;

        public void Add(string key, string val)
        {
            Add(new StringPair() { key = key, val = val });
        }

        public void AddUnique(string key, string val)
        {
            stringPairs.RemoveAll(x => x.key == key);
            Add(new StringPair() { key = key, val = val });
        }

        public string GetOrDefault(string key)
        {
            return stringPairs.FirstOrDefault(x => x.key == key).val;
        }

        public bool TryGetValue(string key, out string val)
        {
            int loc = stringPairs.FindIndex(x => x.key == key);
            if (loc != -1)
            {
                val = stringPairs[loc].val;
                return true;
            }

            val = string.Empty;
            return false;
        }

        public void AddRange(IEnumerable<StringPair> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public void Add(StringPair sp)
        {
            stringPairs.Add(sp);
        }

        public void MakeUnique()
        {
            stringPairs = stringPairs.Distinct().ToList();
        }
    }
}
