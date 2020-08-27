// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus.Examples
{
    /// <summary>
    /// This examples shows how to work with the saveDataSerialiser for types you may need from unity itself or your own codebase.
    /// This is purely an example and is not for production use.
    /// </summary>
    public class TransformDataSerializer : SaveDataSerializer
    {
        //the items we want to load and save
        public List<Transform> transformsToSerialize = new List<Transform>();

        //structure of our save data, we are using json utility but you do not have to
        [System.Serializable]
        public class TransformData
        {
            public List<Vector3> positions = new List<Vector3>();
            public List<Quaternion> rotations = new List<Quaternion>();
        }

        //keys and priority determine when we are asked to load and save in relation to other serializers
        protected const string TransformDataKey = "TransformData";

        protected const int TransformDataDataPriority = 1000;

        public override string DataTypeKey => TransformDataKey;
        public override int Order => TransformDataDataPriority;

        public override void Encode(SavePointData data)
        {
            //gather the info we wish to save, we use index in array and index in saved arrays to sync
            var td = new TransformData();
            foreach (var item in transformsToSerialize)
            {
                td.positions.Add(item.position);
                td.rotations.Add(item.rotation);
            }

            //put the data into a save data item
            var tDataItem = new SaveDataItem()
            {
                DataType = TransformDataKey, 
                Data = JsonUtility.ToJson(td)
            };
            //push it into the save data item collection
            data.SaveDataItems.Add(tDataItem);
        }

        protected override void ProcessItem(SaveDataItem item)
        {
            //the reverse process of our encode,
            var tdata = JsonUtility.FromJson<TransformData>(item.Data);
            if (tdata == null)
            {
                Debug.LogError("Failed to decode Text Variation save data item");
                return;
            }

            //use array index to push data from json array back into the objects in our list
            for (int i = 0; i < transformsToSerialize.Count; i++)
            {
                transformsToSerialize[i].SetPositionAndRotation(tdata.positions[i], tdata.rotations[i]);
            }
        }
    }
}