using System.Collections;
using System.Collections.Generic;

namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// Creates SaveData without being given anything upon being called to do so.
    /// </summary>
    public interface ISaveCreator<TSaveData> where TSaveData: SaveData
    {
        TSaveData CreateSave();
    }

    /// <summary>
    /// Creates SaveData out of an object passed to it.
    /// </summary>
    public interface ISaveCreator<TSaveData, TMakeFrom> where TSaveData: SaveData
    {
        TSaveData CreateSave(TMakeFrom from);
    }

    /// <summary>
    /// Creates a group of SaveData at once without being given anything upon being called to do so.
    /// </summary>
    public interface IGroupSaver<TSaveData> where TSaveData: SaveData
    {
        IList<TSaveData> CreateSaves();
    }

    /// <summary>
    /// Creates a group of SaveData at once from a group of objects passed to it.
    /// </summary>
    public interface IGroupSaver<TSaveData, TMakeFrom> where TSaveData: SaveData
                                                        where TMakeFrom: IList
    {
        IList<TSaveData> CreateSaves(IList<TMakeFrom> from);
    }

    public interface ISaveDataItemsEncoder
    {
        IList<SaveDataItem> CreateItems();
    }

    
}