// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class DefaultSaveHandler : ISaveHandler
    {
        public static ISaveHandler CreateDefaultWithSerializers()
        {
            return new DefaultSaveHandler(
                new FungusSystemSaveDataItemSerializer(),
                new GlobalVariableSaveDataItemSerializer(),
                new MenuSaveDataItemSerializer());
        }

        protected List<ISaveDataItemSerializer> saveDataItemSerializers = new List<ISaveDataItemSerializer>();
        public List<ISaveDataItemSerializer> SaveDataItemSerializers => saveDataItemSerializers;

        public DefaultSaveHandler()
        {
        }

        public DefaultSaveHandler(params ISaveDataItemSerializer[] handlers)
        {
            saveDataItemSerializers.AddRange(handlers);
        }

        public SaveData CreateSaveData(string saveName, string saveDesc)
        {
            var sd = new SaveData(saveName, 
                new StringPair() { key = FungusConstants.SaveDescKey,val = saveDesc });

            saveDataItemSerializers = saveDataItemSerializers.OrderBy(x => x.Order).ToList();

            foreach (var item in saveDataItemSerializers)
            {
                sd.saveDataItems.AddRange(item.Encode());
            }

            return sd;
        }

        public bool LoadSaveData(SaveData sd)
        {
            saveDataItemSerializers = saveDataItemSerializers.OrderBy(x => x.Order).ToList();

            foreach (var item in saveDataItemSerializers)
            {
                item.PreDecode();
            }

            foreach (var item in saveDataItemSerializers)
            {
                var matches = sd.saveDataItems.Where(x => x.DataType == item.DataTypeKey);
                foreach (var match in matches)
                {
                    item.Decode(match);
                }
            }

            foreach (var item in saveDataItemSerializers)
            {
                item.PostDecode();
            }
            //hack, not checking for failures

            return true;
        }

        public SaveData DecodeFromJSON(string jsonSave)
        {
            var sd = JsonUtility.FromJson<SaveData>(jsonSave);

            if (sd == null || (sd.saveDataItems.Count > 0 && sd.saveDataItems.IndexOf(null) != -1))
            {
                Debug.LogError("Failed to decode save from json.");
                return null;
            }

            if (sd != null && sd.version != FungusConstants.CurrentSaveDataVersion)
            {
                var success = HandleVersionMismatch(sd);

                if (!success)
                {
                    Debug.LogError(sd.saveName + " could not be updated from " +
                        sd.version.ToString() + " to " + FungusConstants.CurrentSaveDataVersion.ToString());
                    return null;
                }
            }

            return sd;
        }

        public string EncodeToJSON(SaveData sd)
        {
            return JsonUtility.ToJson(sd);
        }

        public bool HandleVersionMismatch(SaveData sd)
        {
            return true;
        }
    }
}