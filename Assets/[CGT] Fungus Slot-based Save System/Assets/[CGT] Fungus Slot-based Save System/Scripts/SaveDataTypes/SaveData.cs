
using UnityEngine;

namespace CGTUnity.Fungus.SaveSystem
{
    [System.Serializable]
    public class SaveData 
    {
        [SerializeField] string sceneName;

        public string SceneName
        {
            get                         { return sceneName; }
            set                         { sceneName = value; }
        }

        #region Constructors
        public SaveData()               {}

        public SaveData(string sceneName)
        {
            this.sceneName =            sceneName;
        }

        /// <summary>
        /// Creates a copy of the passed SaveData.
        /// </summary>
        public SaveData(SaveData other)
        {
            SetFrom(other);
        }

        #endregion

        /// <summary>
        /// Turns this save data into a deep copy of the one passed, given class constraints.
        /// </summary>
        public virtual void SetFrom(SaveData other)
        {
            this.sceneName =                other.sceneName;
        }

        /// <summary>
        /// Clears this save data of all state it was holding.
        /// </summary>
        public virtual void Clear()
        {
            this.sceneName =                string.Empty;
        }

    }
}