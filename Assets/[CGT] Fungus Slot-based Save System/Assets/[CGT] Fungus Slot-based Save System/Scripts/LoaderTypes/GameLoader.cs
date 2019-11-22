using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CGTUnity.Fungus.SaveSystem
{

    /// <summary>
    /// Loads save data for a whole game/playthrough.
    /// </summary>
    public class GameLoader : SaveLoader<GameSaveData>
    {
        protected List<SaveLoader> subloaders =               new List<SaveLoader>(); 
        // Handles loading the different types of data.

        #region Methods
        protected virtual void Awake()
        {
            // Register all the loaders attached to this game object.
            subloaders.AddRange(GetComponents<SaveLoader>());
            subloaders.RemoveAll(subloader => subloader == this); // This can't be its own subloader.
            subloaders.Sort(SortSubloadersByPriority);
        }

        /// <summary>
        /// Loads a whole game based on the state the passed save data holds. Starts by loading the scene
        /// the save data is registered with.
        /// </summary>
        public override bool Load(GameSaveData saveData)
        {
            // This loader will be gone by the time the scene is loaded. Best pass the job
            // to a lambda, which will then pass the job to the loader in the loaded scene.        
            UnityAction<Scene, LoadSceneMode> onSceneLoaded =   null;
            
            onSceneLoaded = (scene, mode) => 
            {
                if (mode == LoadSceneMode.Single)
                {
                    var loader =                                FindObjectOfType<GameLoader>();
                    if (loader == null) // The user might need a reminder.
                    {
                        Debug.LogError("There is no GameLoader in the scene named " + scene.name);
                        return;
                    }

                    SceneManager.sceneLoaded -=                 onSceneLoaded;
                    loader.LoadState(saveData);
                }
            };

            SceneManager.sceneLoaded +=             onSceneLoaded;
            SceneManager.LoadScene(saveData.SceneName);
            

            return true;
        }

        /// <summary>
        /// Sorts them in descending order.
        /// </summary>
        protected static int SortSubloadersByPriority(SaveLoader first, SaveLoader second)
        {
            if (first.LoadPriority < second.LoadPriority)
                return 1;
            else if (first.LoadPriority > second.LoadPriority)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// Loads the state of the passed save data without loading the scene it's registered with.
        /// </summary>
        public virtual void LoadState(GameSaveData saveData)
        {
            if (!string.IsNullOrEmpty(saveData.ProgressMarkerKey))
                ProgressMarker.latestExecuted =     ProgressMarker.FindWithKey(saveData.ProgressMarkerKey);
            // ^ In case any to-be-reexecuted blocks need to know

            // Have each subloader go through the items one at a time, loading as many of them 
            // as they can. 

            for (int i = 0; i < subloaders.Count; i++)
            {
                var loader =                        subloaders[i];

                if (loader == null)
                {
                    Debug.LogWarning(this.name + " has a null subloader registered under it.");
                    continue;
                }

                for (int j = 0; j < saveData.Items.Count; j++)
                {
                    var saveItem =                  saveData.Items[j];
                    loader.Load(saveItem);
                }

            }

        }
    
        #endregion
    }

}