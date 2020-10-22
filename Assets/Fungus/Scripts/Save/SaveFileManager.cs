// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    /// <summary>
    /// </summary>
    public class SaveFileManager
    {
        [SerializeField] protected List<SaveGameMetaData> saveMetas = new List<SaveGameMetaData>();

        public void Init(UserProfileManager userProfileManager)
        {
            UserProfileManager = userProfileManager;
            PopulateSaveMetas();
        }

        /// <summary>
        /// Access list of all currently known saves, for the current profile.
        /// </summary>
        public List<SaveGameMetaData> SaveMetas { get { return saveMetas; } }

        public ISaveHandler CurrentSaveHandler { get; set; } = DefaultSaveGameSaveHandler.CreateDefaultWithSerializers();

        protected const string FileExtension = ".save";

        public UserProfileManager UserProfileManager { get; private set; }

        public virtual int NumSaveMetas { get { return saveMetas.Count; } }

        /// <summary>
        /// Gathers all saves for the current profile, filling the SaveMetas collection.
        ///
        /// If there are less existing user saves that configured, empty metas are generated.
        /// </summary>
        public void PopulateSaveMetas()
        {
            saveMetas.Clear();

            var dir = UserProfileManager.GetCurrentUserProfileDirectory();

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dir));

            var foundFiles = System.IO.Directory.GetFiles(dir, "*" + FileExtension);

            foreach (var item in foundFiles)
            {
                var fileContents = System.IO.File.ReadAllText(item);
                var save = CurrentSaveHandler.DecodeFromJSON(fileContents);
                GenerateMetaFromSave(item, save);
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
                saveMetas.Add(new SaveGameMetaData()
                {
                    fileLocation = fileLoc,
                    saveName = save.saveName,
                    description = save.stringPairs.GetOrDefault(FungusConstants.SaveDescKey),
                    lastWritten = save.LastWritten,
                });
            }
        }

        public int SaveNameToIndex(string saveName)
        {
            return saveMetas.FindIndex(x => x.saveName == saveName);
        }

        public SaveGameMetaData SaveNameToMeta(string saveName)
        {
            return saveMetas.FirstOrDefault(x => x.saveName == saveName);
        }

        /// <summary>
        /// Deletes a previously stored Save History from persistent storage.
        /// </summary>
        public void DeleteSave(int index)
        {
            var meta = saveMetas[index];
            if (System.IO.File.Exists(meta.fileLocation))
            {
                System.IO.File.Delete(meta.fileLocation);
            }
            saveMetas.RemoveAt(index);
            SaveManagerSignals.DoSaveDeleted(meta.saveName);
        }

        /// <summary>
        /// Creates a new Save Point using a key and description.
        /// </summary>
        public virtual void Save(string saveName, string savePointDescription, string prefix)
        {
            SaveManagerSignals.DoSavePrepare(saveName, savePointDescription);

            var existingMetaIndex = SaveNameToIndex(saveName);
            if (existingMetaIndex >= 0)
            {
                DeleteSave(existingMetaIndex);
            }

            var saveData = CurrentSaveHandler.CreateSaveData(saveName, savePointDescription);

            saveData.stringPairs.Add(FungusConstants.SceneNameKey, SceneManager.GetActiveScene().name);

            var savePointDataJSON = CurrentSaveHandler.EncodeToJSON(saveData);

            var dir = UserProfileManager.GetCurrentUserProfileDirectory();
            var fileName = dir + prefix + System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss.ffff") + FileExtension;
            GenerateMetaFromSave(fileName, saveData);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
            System.IO.File.WriteAllText(fileName, savePointDataJSON, System.Text.Encoding.UTF8);

            SaveManagerSignals.DoSaveSaved(saveName, savePointDescription);
        }

        public SaveData GetSaveDataFromMeta(SaveGameMetaData meta)
        {
            var saveContent = System.IO.File.ReadAllText(meta.fileLocation, System.Text.Encoding.UTF8);

            return CurrentSaveHandler.DecodeFromJSON(saveContent);
        }

        /// <summary>
        /// Helper to call LoadSavePoint via a meta.
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public virtual bool Load(SaveGameMetaData meta)
        {
            var savePointData = GetSaveDataFromMeta(meta);

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
            if (savePointData == null)
                return false;

            var sceneName = savePointData.stringPairs.GetOrDefault(FungusConstants.SceneNameKey);

            if (string.IsNullOrEmpty(sceneName))
                return false;

            UnityEngine.Events.UnityAction<Scene, LoadSceneMode> onSceneLoadedAction = null;

            onSceneLoadedAction = (scene, mode) =>
            {
                // Additive scene loads and non-matching scene loads could happen if the client is using the
                // SceneManager directly. We just ignore these events and hope they know what they're doing!
                if (mode == LoadSceneMode.Additive ||
                    scene.name != sceneName)
                {
                    return;
                }

                SceneManager.sceneLoaded -= onSceneLoadedAction;

                // Look for a SaveData component in the scene to process the save data items.
                CurrentSaveHandler.LoadSaveData(savePointData);

                SaveManagerSignals.DoSaveLoaded(savePointData.saveName);
            };

            SceneManager.sceneLoaded += onSceneLoadedAction;
            SaveManagerSignals.DoSavePreLoad(savePointData.saveName);
            SceneManager.LoadScene(sceneName);

            return true;
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
    }
}
