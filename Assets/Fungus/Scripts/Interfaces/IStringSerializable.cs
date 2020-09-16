// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Interface for converting to and from a stringified value, that can be roundtripped to serialize
    /// the object. Can also indicate if it can or cannot be serialised.
    /// </summary>
    public interface IStringSerializable
    { 
        bool IsSerializable { get;}
        string GetStringifiedValue();
        void RestoreFromStringifiedValue(string stringifiedValue);
    }
}
