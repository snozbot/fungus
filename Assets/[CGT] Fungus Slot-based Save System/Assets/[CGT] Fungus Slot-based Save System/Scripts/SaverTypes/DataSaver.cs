using System.Collections.Generic;
using UnityEngine;


namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// For MonoBehaviours that can create SaveData that can be encoded to an array of SaveDataItems.
    /// </summary>
    public abstract class DataSaver : MonoBehaviour, ISaveDataItemsEncoder
    {
        public abstract IList<SaveDataItem> CreateItems();
    }

    
    public abstract class DataSaver<TSaveData> : DataSaver 
                                                where TSaveData: SaveData
    {
        protected System.Type saveType =            typeof(TSaveData);
    }

}