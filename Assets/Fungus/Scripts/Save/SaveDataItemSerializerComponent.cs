// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public abstract class SaveDataItemSerializerComponent : MonoBehaviour
    {
        public abstract ISaveDataItemSerializer SaveDataItemSerializer { get; }
    }

    [AddComponentMenu("")]
    public abstract class SaveDataItemSerializerComponentT<T> : SaveDataItemSerializerComponent where T : ISaveDataItemSerializer
    {
        public T serializerSettings;
        public override ISaveDataItemSerializer SaveDataItemSerializer => serializerSettings;
    }
}