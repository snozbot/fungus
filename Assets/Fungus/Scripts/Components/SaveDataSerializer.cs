// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;

//TODO needs doco update
//  priority?

namespace Fungus
{
    /// <summary>
    /// This component encodes and decodes a list of game objects to be saved for each Save Point.
    /// It knows how to encode / decode concrete game classes like Flowchart and FlowchartData.
    /// To extend the save system to handle other data types, just modify or subclass this component.
    /// </summary>
    public abstract class SaveDataSerializer : MonoBehaviour
    {
        public abstract string DataTypeKey { get; }

        /// <summary>
        /// Encodes the objects to be saved as a list of SaveDataItems.
        /// </summary
        public abstract void Encode(SavePointData data);

        /// <summary>
        /// Decodes the loaded list of SaveDataItems to restore the saved game state.
        /// </summary>
        public abstract void Decode(SavePointData data);

        public void DecodeMatchingItem(SavePointData data, System.Action<SaveDataItem> itemAction)
        {
            foreach (var item in data.SaveDataItems)
            {
                if(item.DataType == DataTypeKey)
                {
                    itemAction(item);
                }
            }
        }
    }
}

#endif