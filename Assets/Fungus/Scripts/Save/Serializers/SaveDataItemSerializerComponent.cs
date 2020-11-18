// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for ease of creating MonoBehaviours that hold a SaveDataItemSerializer. Serializers are
    /// not required to be MonoBeh but often want to be so they can direct reference scene elements. 
    /// </summary>
    [AddComponentMenu("")]
    public abstract class SaveDataItemSerializerComponent : MonoBehaviour
    {
        public abstract ISaveDataItemSerializer SaveDataItemSerializer { get; }
    }

    /// <summary>
    /// Base to ease defining MonoBeh Serializers, see MultiFlowchartSaveDataItemSerializerComponent for simple example.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [AddComponentMenu("")]
    public abstract class SaveDataItemSerializerComponentT<T> : SaveDataItemSerializerComponent where T : ISaveDataItemSerializer
    {
        public T serializerSettings;
        public override ISaveDataItemSerializer SaveDataItemSerializer => serializerSettings;
    }
}
