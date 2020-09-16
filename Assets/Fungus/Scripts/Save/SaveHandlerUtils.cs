// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEngine;

namespace Fungus
{
    //Our current version of c# in unity doesn't yet support default methods in interfaces so we use helpers
    public static class SaveHandlerUtils
    {
        public static SaveData CreateSaveData(ISaveHandler saveHandler,string saveName, string saveDesc, int version)
        {
            var sd = new SaveData(saveName,
                new StringPair() { key = FungusConstants.SaveDescKey, val = saveDesc })
            { 
                version = version
            };

            var serializers = saveHandler.SaveDataItemSerializers;

            foreach (var item in serializers)
            {
                sd.saveDataItems.AddRange(item.Encode());
            }

            return sd;
        }

        public static bool LoadSaveData(ISaveHandler saveHandler, SaveData sd)
        {
            var serializers = saveHandler.SaveDataItemSerializers;

            foreach (var item in serializers)
            {
                item.PreDecode();
            }

            foreach (var item in serializers)
            {
                var matches = sd.saveDataItems.AsReadOnly().Where(x => x.key == item.DataTypeKey);
                foreach (var match in matches)
                {
                    item.Decode(match);
                }
            }

            foreach (var item in serializers)
            {
                item.PostDecode();
            }
            //hack, not checking for failures

            return true;
        }

        public static SaveData DecodeFromJSON(ISaveHandler saveHandler, string jsonSave)
        {
            var sd = JsonUtility.FromJson<SaveData>(jsonSave);

            if (sd == null)
            {
                Debug.LogError("Failed to decode save from json.");
                return null;
            }

            if (sd.version != saveHandler.CurrentExpectedVersion)
            {
                var success = saveHandler.HandleVersionMismatch(sd);

                if (!success)
                {
                    Debug.LogError(sd.saveName + " could not be updated from " +
                        sd.version.ToString() + " to " + saveHandler.CurrentExpectedVersion.ToString());
                    return null;
                }
            }

            return sd;
        }
    }
}