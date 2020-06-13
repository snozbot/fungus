// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Abstract base for components that encodes and decodes parts of the game for saving
    /// and loading.
    /// </summary>
    public abstract class SaveDataSerializer : MonoBehaviour, IComparer<SaveDataSerializer>
    {
        protected static List<SaveDataSerializer> activeSerializers = new List<SaveDataSerializer>();
        public static List<SaveDataSerializer> ActiveSerializers { get { return activeSerializers; } }

        protected virtual void OnEnable()
        {
            activeSerializers.Add(this);
            activeSerializers.Sort(this);
        }

        protected virtual void OnDisable()
        {
            activeSerializers.Remove(this);
            //don't need to sort on remove, as its stable
        }

        /// <summary>
        /// Used to determine which specialised loader is to be used and saved with each item to
        /// signify what encoded it.
        /// </summary>
        public abstract string DataTypeKey { get; }

        /// <summary>
        /// Loaders are sorted by their order before being asked to decode, allowing for higher
        /// order serialisers to overwrite lower order ones.
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// Encodes the objects to be saved as a list of SaveDataItems.
        /// </summary
        public abstract void Encode(SavePointData data);

        /// <summary>
        /// Decodes the loaded list of SaveDataItems to restore the saved game state.
        /// </summary>
        public virtual void Decode(SavePointData data)
        {
            DecodeMatchingDataTypeItems(data);
        }

        /// <summary>
        /// Called on serialisers after all have completed thier decodes
        /// </summary>
        public virtual void PostDecode() { }

        /// <summary>
        /// Helper for child classes that wish to decode only exact matches to their TypeKey
        /// </summary>
        /// <param name="data"></param>
        public virtual void DecodeMatchingDataTypeItems(SavePointData data)
        {
            foreach (var item in data.SaveDataItems)
            {
                if (item.DataType == DataTypeKey)
                {
                    ProcessItem(item);
                }
            }
        }

        /// <summary>
        /// Internal function used for matching items found in DecodeMatchingDataTypeItems
        /// </summary>
        /// <param name="item"></param>
        protected virtual void ProcessItem(SaveDataItem item) { }

        public int Compare(SaveDataSerializer x, SaveDataSerializer y)
        {
            return x.Order.CompareTo(y.Order);
        }
    }
}