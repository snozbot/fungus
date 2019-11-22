namespace CGTUnity.Fungus.SaveSystem
{
    public interface ISaveLoader
    {
        bool Load(SaveDataItem item);
    }

    /// <summary>
    /// For objects that load save data, restoring state based on said data.
    /// </summary>
    public interface ISaveLoader<TSaveData> : ISaveLoader
    {
        bool Load(TSaveData toLoad);
    }

}