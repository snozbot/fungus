// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;

namespace Fungus
{
    public interface ISaveHandler
    {
        List<ISaveDataItemSerializer> SaveDataItemSerializers { get; }

        SaveData CreateSaveData(string saveName, string saveDesc);

        bool LoadSaveData(SaveData sd);

        string EncodeToJSON(SaveData sd);

        SaveData DecodeFromJSON(string jsonSave);

        bool HandleVersionMismatch(SaveData sd);
    }
}