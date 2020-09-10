// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [AddComponentMenu("")]
    /// <summary>
    /// Manages the Save History (a list of Save Points) and provides a set of operations for saving and loading games.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        /// <summary>
        /// Meta data about a save that has found by a save manager.
        /// These exist to prevent the Save Manager from keeping potentially a lot of very large json files in ram.
        /// </summary>
        public class SavePointMeta
        {
            public string saveName;
            public string description;
            public System.DateTime lastWritten;
            public string fileLocation;
            public string progressMarker;

            public string GetReadableTime()
            {
                return lastWritten.ToString("O");
            }
        }

        public enum SaveType
        {
            Auto,
            User,
            Any,
        }

        [SerializeField] protected List<SavePointMeta> saveMetas = new List<SavePointMeta>();

        /// <summary>
        /// Access list of all currently known saves, for the current profile.
        /// </summary>
        public List<SavePointMeta> SaveMetas { get { return saveMetas; } }

        public ISaveHandler CurrentSaveHandler { get; set; } = DefaultSaveHandler.CreateDefaultWithSerializers();

        protected const string FileExtension = ".save";


        /// <summary>
        /// The scene that should be loaded when restarting a game.
        /// </summary>
        public string StartScene { get; set; }

        /// <summary>
        /// Returns the number of Save Points in the Save History.
        /// </summary>
        public virtual int NumSaves { get { return saveMetas.Count; } }

        protected int numAutoSaves = 1, numUserSaves = 0;

        /// <summary>
        /// SaveManager wants to know how many UserSaves are expected, this is set via the ConfigureSaveNumber.
        /// </summary>
        public int NumberOfUserSaves { get { return numUserSaves; } }

        /// <summary>
        /// SaveManager wants to know how many Auto Saves are allowed, this is set via the ConfigureSaveNumber.
        /// </summary>
        public int NumberOfAutoSaves { get { return numAutoSaves; } }

        /// <summary>
        /// Determines the number of saves expected and maintained for the current profile by the save manager.
        ///
        /// PopulatesSaveMetas when called.
        /// </summary>
        /// <param name="numAutos">Max auto saves, after which, the oldest will be removed</param>
        /// <param name="numUser">Slots for user saves that will be maintained</param>
        public void ConfigureSaveNumber(int numAutos, int numUser)
        {
            numAutoSaves = numAutos;
            numUserSaves = numUser;
            PopulateSaveMetas();
        }

        /// <summary>
        /// Set during SaveManager loading, intended to be used by any class that wants conditional logic
        /// for a 'normal' level load vs one caused by a the save manager.
        /// </summary>
        public bool IsSaveLoading { get; protected set; }

        /// <summary>
        /// If false, calls to Save will be immediately short circuited. Intended for user to prevent saving
        /// during gameplay sections that are either undesirable or not safe to save within.
        /// </summary>
        public virtual bool IsSavingAllowed
        {
            get { return _isSavingAllowed; }
            set { _isSavingAllowed = value; SaveManagerSignals.DoSavingLoadingAllowedChanged(); }
        }

        protected bool _isSavingAllowed = true;

        /// <summary>
        /// If false, calls to Load will be immediately short circuited. Intended for user to prevent loading
        /// during gameplay sections that are either undesirable or for somereason unsafe to do so.
        /// </summary>
        public virtual bool IsLoadingAllowed
        {
            get { return _isLoadingAllowed; }
            set { _isLoadingAllowed = value; SaveManagerSignals.DoSavingLoadingAllowedChanged(); }
        }

        protected bool _isLoadingAllowed = true;

        public void Awake()
        {
            IsSaveLoading = false;
            IsSavingAllowed = true;
            IsLoadingAllowed = true;
            StartScene = SceneManager.GetActiveScene().name;

            //listen to profile changes
            //and reset when it changes
        }

        //TODO needs web version
        /// <summary>
        /// Gathers all saves for the current profile, filling the SaveMetas collection.
        ///
        /// If there are less existing user saves that configured, empty metas are generated.
        /// </summary>
        public void PopulateSaveMetas()
        {
            saveMetas.Clear();

            var dir = FungusManager.Instance.UserProfileManager.GetCurrentUserProfileDirectory();
#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dir));

            var foundFiles = System.IO.Directory.GetFiles(dir, "*" + FileExtension);

            foreach (var item in foundFiles)
            {
                var fileContents = System.IO.File.ReadAllText(item);
                var save = CurrentSaveHandler.DecodeFromJSON(fileContents);
                GenerateMetaFromSave(item, save);
            }

            //TODO look at the settings and ensure we have saves in correct order for user saves and put dumbies in where we don't
            var userSaves = CollectUserSaves();

            for (int i = 0; i < NumberOfUserSaves; i++)
            {
                if (userSaves.Find(x => x.saveName.EndsWith(i.ToString())) == null)
                {
                    saveMetas.Add(new SavePointMeta() { saveName = FungusConstants.UserSavePrefix + i.ToString() });
                }
            }
        }

        /// <summary>
        /// Helpder to create the meta from a fullsave
        /// </summary>
        /// <param name="fileLoc"></param>
        /// <param name="save"></param>
        private void GenerateMetaFromSave(string fileLoc, SaveData save)
        {
            if (save != null)
            {
                saveMetas.Add(new SavePointMeta()
                {
                    fileLocation = fileLoc,
                    saveName = save.saveName,
                    description = save.GetStringPairValue(FungusConstants.SaveDescKey),
                    lastWritten = save.LastWritten,
                });
            }
        }

        public int SaveNameToIndex(string saveName)
        {
            return saveMetas.FindIndex(x => x.saveName == saveName);
        }

        /// <summary>
        /// Deletes a previously stored Save History from persistent storage.
        /// </summary>
        public void DeleteSave(int index, bool suppressReplaceSlot = false)
        {
#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
            var meta = saveMetas[index];
            if (System.IO.File.Exists(meta.fileLocation))
            {
                System.IO.File.Delete(meta.fileLocation);
            }
            if (meta.saveName.StartsWith(FungusConstants.UserSavePrefix) && !suppressReplaceSlot)
            {
                saveMetas.Add(new SavePointMeta() { saveName = meta.saveName });
            }
            saveMetas.RemoveAt(index);
            SaveManagerSignals.DoSaveDeleted(meta.saveName);
        }

        public void DeleteSave(SavePointMeta meta)
        {
            DeleteSave(saveMetas.IndexOf(meta));
        }

        /// <summary>
        /// Creates a new Save Point using a key and description.
        /// </summary>
        public virtual void Save(string saveName, string savePointDescription, bool isAutoSave = false)
        {
            if (!IsSavingAllowed)
                return;

            SaveManagerSignals.DoSavePrepare(saveName, savePointDescription);

            var existingMetaIndex = SaveNameToIndex(saveName);
            if (existingMetaIndex >= 0)
            {
                DeleteSave(existingMetaIndex, true);
            }

            var saveData = CurrentSaveHandler.CreateSaveData(saveName, savePointDescription);

            saveData.stringPairs.Add(new StringPair() { key = FungusConstants.SceneNameKey, val = SceneManager.GetActiveScene().name });

            var savePointDataJSON = CurrentSaveHandler.EncodeToJSON(saveData);

            var dir = FungusManager.Instance.UserProfileManager.GetCurrentUserProfileDirectory();
            var fileName = dir + (isAutoSave ? FungusConstants.AutoSavePrefix : FungusConstants.UserSavePrefix)
                + System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss.ffff") + FileExtension;
            GenerateMetaFromSave(fileName, saveData);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
            System.IO.File.WriteAllText(fileName, savePointDataJSON, System.Text.Encoding.UTF8);
#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif

            //if we limit autos and it is an auto, are there now to many, delete oldest until not over limit
            if (isAutoSave && NumberOfAutoSaves >= 0)
            {
                var autoSaves = CollectAutoSaves();

                for (int i = 0; i < autoSaves.Count - NumberOfAutoSaves; i++)
                {
                    DeleteSave(saveMetas.IndexOf(autoSaves[i]), true);
                }
            }

            SaveManagerSignals.DoSaveSaved(saveName, savePointDescription);
        }

        /// <summary>
        /// Helper to call LoadSavePoint via a meta.
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public virtual bool Load(SavePointMeta meta)
        {
            if (!IsLoadingAllowed)
                return false;

#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
            var saveContent = System.IO.File.ReadAllText(meta.fileLocation, System.Text.Encoding.UTF8);

            var savePointData = CurrentSaveHandler.DecodeFromJSON(saveContent);

            if (!LoadSavePoint(savePointData))
            {
                Debug.LogError("Failed to Load " + meta.saveName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cause a scene load, flagging that we are loading a save during it, in IsSaveLoading.
        /// When scene is loaded, we ask the savepoint to RunDeserialize. Means the serializers
        /// need to exist in target scene, either existing in all scenes manually, or singlying
        /// via a Don'tDestroyOnLoad
        /// </summary>
        /// <param name="savePointData"></param>
        /// <returns></returns>
        public virtual bool LoadSavePoint(SaveData savePointData)
        {
            if (!IsLoadingAllowed)
                return false;

            if (savePointData == null)
                return false;

            var sceneName = savePointData.GetStringPairValue(FungusConstants.SceneNameKey);

            if (string.IsNullOrEmpty(sceneName))
                return false;

            UnityEngine.Events.UnityAction<Scene, LoadSceneMode> onSceneLoadedAction = null;

            onSceneLoadedAction = (scene, mode) =>
            {
                // Additive scene loads and non-matching scene loads could happen if the client is using the
                // SceneManager directly. We just ignore these events and hope they know what they're doing!
                if (mode == LoadSceneMode.Additive ||
                //TODO use savedata string pair for this
                    scene.name != sceneName)
                {
                    return;
                }

                SceneManager.sceneLoaded -= onSceneLoadedAction;

                // Look for a SaveData component in the scene to process the save data items.
                CurrentSaveHandler.LoadSaveData(savePointData);

                SaveManagerSignals.DoSaveLoaded(savePointData.saveName);

                StartCoroutine(DelaySetNotLoading());
            };

            SceneManager.sceneLoaded += onSceneLoadedAction;
            IsSaveLoading = true;
            SaveManagerSignals.DoSavePreLoad(savePointData.saveName);
            SceneManager.LoadScene(sceneName);

            return true;
        }

        /// <summary>
        /// Used to allow full enable, start, update to run before turning off our loading flag.
        /// </summary>
        /// <returns></returns>
        private System.Collections.IEnumerator DelaySetNotLoading()
        {
            yield return new WaitForEndOfFrame();
            IsSaveLoading = false;
        }

        /// <summary>
        /// Deletes all Save Points in the Save History.
        /// </summary>
        public virtual void DeleteAllSaves()
        {
            for (int i = saveMetas.Count - 1; i >= 0; i--)
            {
                DeleteSave(i);
            }
        }

        /// <summary>
        /// Return the most recently written save regardless of type of save.
        /// </summary>
        /// <returns></returns>
        public virtual SavePointMeta GetMostRecentSave()
        {
            if (SaveMetas.Count > 0)
            {
                var newestSaveTime = SaveMetas.Max(x => x.lastWritten);

                return SaveMetas.FirstOrDefault(x => x.lastWritten == newestSaveTime);
            }

            return null;
        }

        /// <summary>
        /// Gather and return all Auto saves currently in our meta list.
        /// </summary>
        /// <returns></returns>
        public List<SavePointMeta> CollectAutoSaves()
        {
            return SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.AutoSavePrefix))
                .OrderBy(x => x.lastWritten.Ticks).ToList();
        }

        /// <summary>
        /// Gather and return all User (slot) saves currently in our meta list.
        /// </summary>
        /// <returns></returns>
        public List<SavePointMeta> CollectUserSaves()
        {
            return SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.UserSavePrefix))
                .OrderBy(x => System.Convert.ToInt32(x.saveName.Substring(FungusConstants.UserSavePrefix.Length))).ToList();
        }

        /// <summary>
        /// Reload the starting scene, without setting the loading flag. If requested, can delete all saves on
        /// the current profile.
        /// </summary>
        /// <param name="deleteSaves"></param>
        /// <returns></returns>
        public virtual bool Restart(bool deleteSaves)
        {
            if (string.IsNullOrEmpty(StartScene))
            {
                Debug.LogError("No start scene specified");
                return false;
            }

            // Reset the Save History for a new game
            if (deleteSaves)
            {
                DeleteAllSaves();
            }

            SaveManagerSignals.DoSaveReset();
            SceneManager.LoadScene(StartScene);
            return true;
        }
    }
}