// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    public abstract class DefaultSaveHandler : ISaveHandler
    {
        public abstract int CurrentExpectedVersion { get; }

        protected List<ISaveDataItemSerializer> saveDataItemSerializers = new List<ISaveDataItemSerializer>();
        public List<ISaveDataItemSerializer> SaveDataItemSerializers => saveDataItemSerializers;

        public SaveData CreateSaveData(string saveName, string saveDesc)
        {
            saveDataItemSerializers = saveDataItemSerializers.OrderBy(x => x.Order).ToList();

            return SaveHandlerUtils.CreateSaveData(this, saveName, saveDesc);
        }

        public bool LoadSaveData(SaveData sd)
        {
            saveDataItemSerializers = saveDataItemSerializers.OrderBy(x => x.Order).ToList();

            return SaveHandlerUtils.LoadSaveData(this, sd);
        }

        public SaveData DecodeFromJSON(string jsonSave)
        {
            return SaveHandlerUtils.DecodeFromJSON(this, jsonSave);
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
