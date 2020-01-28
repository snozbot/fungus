using UnityEngine;

//todo remove

namespace Fungus.SaveSystem
{
    public abstract class SaveLoader : MonoBehaviour
    {
        [Tooltip("Higher priority means this loads before the others.")]
        [SerializeField] protected int loadPriority = 0;

        public virtual int LoadPriority { get { return loadPriority; } }

        /// <summary>
        /// Tries to load the passed item. Returns true if
        /// successful, false otherwise.
        /// </summary>
        public abstract bool Load(SaveDataItem item);
    }

    public abstract class SaveLoader<TSaveData> : SaveLoader
    {
        protected System.Type saveType = typeof(TSaveData);

        /// <summary>
        /// Loads the passed item if the right kind of SaveData can be extracted from it.
        /// </summary>
        public override bool Load(SaveDataItem item)
        {
            if (item.DataType != this.saveType.Name)
                return false;

            TSaveData saveData = JsonUtility.FromJson<TSaveData>(item.Data);

            if (saveData == null)
                return false;
            else
                return Load(saveData);
        }

        public abstract bool Load(TSaveData saveData);
    }
}