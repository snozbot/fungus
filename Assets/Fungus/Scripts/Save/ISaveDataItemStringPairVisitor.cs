// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public interface ISaveDataItemStringPairVisitor
    {
        void AddToVisitorPairs(string key, string value);
        bool TryGetVisitorValueByKey(string key, out string value);
    }
}