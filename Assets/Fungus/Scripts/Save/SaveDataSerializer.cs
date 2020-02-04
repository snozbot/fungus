// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

//TODO doco update

namespace Fungus
{
    /// <summary>
    /// Abstract base for components that encodes and decodes parts of the game for saving
    /// and loading.
    /// </summary>
    public abstract class SaveDataSerializer : MonoBehaviour
    {
        public abstract string DataTypeKey { get; }

        public abstract int Order { get; }

        /// <summary>
        /// Encodes the objects to be saved as a list of SaveDataItems.
        /// </summary
        public abstract void Encode(SavePointData data);

        /// <summary>
        /// Decodes the loaded list of SaveDataItems to restore the saved game state.
        /// </summary>
        public abstract void Decode(SavePointData data);

        public virtual void PostDecode(){ }

        public virtual void DecodeMatchingDataTypeItems(SavePointData data)
        {
            foreach (var item in data.SaveDataItems)
            {
                if(item.DataType == DataTypeKey)
                {
                    ProcessItem(item);
                }
            }
        }

        protected virtual void ProcessItem(SaveDataItem item) { }
    }
}