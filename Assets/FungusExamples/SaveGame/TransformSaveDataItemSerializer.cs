// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus.Examples
{
    [System.Serializable]
    /// <summary>
    /// This examples shows how to work with the saveDataSerialiser for types you may need from unity itself or your own codebase.
    /// This is purely an example and is not for production use.
    /// </summary>
    public class TransformSaveDataItemSerializer : ISaveDataItemSerializer
    {
        //the items we want to load and save
        public List<Transform> transformsToSerialize = new List<Transform>();

        //keys and priority determine when we are asked to load and save in relation to other serializers
        protected const string TransformDataKey = "TransformData";

        protected const int TransformDataDataPriority = 1000;

        public string DataTypeKey => TransformDataKey;
        public int Order => TransformDataDataPriority;

        public SaveDataItem[] Encode()
        {
            var td = new TransformData();
            foreach (var item in transformsToSerialize)
            {
                td.positions.Add(item.position);
                td.rotations.Add(item.rotation);
            }

            return new SaveDataItem[]
            {
                new SaveDataItem()
                {
                    DataType = DataTypeKey,
                    Data = JsonUtility.ToJson(td)
                } 
            };
        }

        public bool Decode(SaveDataItem sdi)
        {
            //the reverse process of our encode,
            var tdata = JsonUtility.FromJson<TransformData>(sdi.Data);
            if (tdata == null)
            {
                Debug.LogError("Failed to decode Text Variation save data item");
                return false;
            }

            //use array index to push data from json array back into the objects in our list
            for (int i = 0; i < transformsToSerialize.Count; i++)
            {
                transformsToSerialize[i].SetPositionAndRotation(tdata.positions[i], tdata.rotations[i]);
            }

            return true;
        }

        public void PreDecode()
        {
        }

        public void PostDecode()
        {
        }

        //structure of our save data, we are using json utility but you do not have to
        [System.Serializable]
        public class TransformData
        {
            public List<Vector3> positions = new List<Vector3>();
            public List<Quaternion> rotations = new List<Quaternion>();
        }
    }
}