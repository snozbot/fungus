// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

//TODO update doco

namespace Fungus
{
    /// <summary>
    /// Manages the Save History (a list of Save Points) and provides a set of operations for saving and loading games.
    /// 
    /// Note WebGL and Webplayer (deprecated) save using playerprefs instead of using a json file in persistent storage
    /// -webgl would require additional js to force a sync of FS.syncfs
    /// -webplayer does not implement system io
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public class SavePointMeta
        {
            public string saveName;
            public string savePointDescription;
            public System.DateTime savePointLastWritten;
            public string fileLocation;
            public string progressMarker;
        }

        [SerializeField] protected List<SavePointMeta> saveMetas = new List<SavePointMeta>();

        [SerializeField] protected SaveSettings saveSettings;
        public SaveSettings SaveSettings
        {
            get
            {
                return saveSettings;
            }
            set
            {
                saveSettings = value;
                PopulateSaveMetas();
            }
        }

        public List<SavePointMeta> SaveMetas { get { return saveMetas; } }

        [SerializeField] protected string currentSaveProfileKey = string.Empty;

        public string CurrentSaveProfileKey { get { return currentSaveProfileKey; } }


        //#if UNITY_WEBPLAYER || UNITY_WEBGL
        [System.Serializable]
        public class WebSaveBlob
        {
            public List<string> saveJSONs = new List<string>();
        }

        [SerializeField] protected WebSaveBlob webSaveBlob = new WebSaveBlob();

        [System.Serializable]
        protected class SaveManagerData
        {
            public string lastProfileName;
        }

        public static string STORAGE_DIRECTORY { get { return Application.persistentDataPath + "/FungusSaves/"; } }
        protected const string FileExtension = ".save";

        private string GetFullSaveDir()
        {
            return System.IO.Path.GetFullPath(STORAGE_DIRECTORY + currentSaveProfileKey + "/");
        }
        private string GetSaveManagerDataFile()
        {
            return System.IO.Path.GetFullPath(STORAGE_DIRECTORY + "save_manager_data.json");
        }

        /// <summary>
        /// The scene that should be loaded when restarting a game.
        /// </summary>
        public string StartScene { get; set; }

        /// <summary>
        /// Returns the number of Save Points in the Save History.
        /// </summary>
        public virtual int NumSaves { get { return saveMetas.Count; } }

        public bool IsSaveLoading { get; protected set; }

        public void Awake()
        {
            IsSaveLoading = false;
            StartScene = SceneManager.GetActiveScene().name;

            try
            {
                var fileName = GetSaveManagerDataFile();
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
                var datString = System.IO.File.ReadAllText(fileName);
                var dat = JsonUtility.FromJson<SaveManagerData>(datString);
                if(dat != null)
                {
                    ChangeProfile(dat.lastProfileName);
                }
            }
            catch (Exception)
            {
                ChangeProfile(FungusConstants.DefaultSaveProfileKey);
            }

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(!IsSaveLoading)
            {
                //scene was loaded not a save game
                var savePoints = UnityEngine.Object.FindObjectsOfType<ProgressMarker>().ToList();
                var startingSavePoint = savePoints.FirstOrDefault(x => x.IsStartPoint);
                if(startingSavePoint != null)
                {
                    startingSavePoint.GetFlowchart().ExecuteBlock(startingSavePoint.ParentBlock, startingSavePoint.CommandIndex);
                }
            }
        }

        public void ChangeProfile(string saveProfileKey)
        {
            if (saveProfileKey != currentSaveProfileKey)
            {
                currentSaveProfileKey = saveProfileKey;

                var fileName = GetSaveManagerDataFile();
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
                var profile = new SaveManagerData() { lastProfileName = currentSaveProfileKey };
                System.IO.File.WriteAllText(fileName, JsonUtility.ToJson(profile));

                PopulateSaveMetas();
                SaveManagerSignals.DoSaveProfileChanged();
                SaveManagerSignals.DoSaveReset();
            }
        }

        //TODO needs web version
        public void PopulateSaveMetas()
        {
            saveMetas.Clear();

            var dir = GetFullSaveDir();

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dir));

            var foundFiles = System.IO.Directory.GetFiles(dir, "*" + FileExtension);

            foreach (var item in foundFiles)
            {
                var fileContents = System.IO.File.ReadAllText(item);
                var save = SavePointData.DecodeFromJSON(fileContents);
                GenerateMetaFromSave(item, save);
            }

            //TODO look at the settings and ensure we have saves in correct order for user saves and put dumbies in where we don't
            if (saveSettings != null)
            {
                var userSaves = CollectUserSaves();

                for (int i = 0; i < saveSettings.NumberOfUserSaves; i++)
                {
                    if (userSaves.Find(x => x.saveName.EndsWith(i.ToString())) == null)
                    {
                        saveMetas.Add(new SavePointMeta() { saveName = FungusConstants.UserSavePrefix + i.ToString() });
                    }
                }
            }
        }

        private void GenerateMetaFromSave(string fileLoc, SavePointData save)
        {
            if (save != null)
            {
                saveMetas.Add(new SavePointMeta()
                {
                    fileLocation = fileLoc,
                    saveName = save.SaveName,
                    progressMarker = save.ProgressMarkerName,
                    savePointDescription = save.SavePointDescription,
                    savePointLastWritten = save.LastWritten,
                });
            }
        }

        public int SavePointNameToIndex(string saveName)
        {
            return saveMetas.FindIndex(x => x.saveName == saveName);
        }

        /// <summary>
        /// Deletes a previously stored Save History from persistent storage.
        /// </summary>
        public void DeleteSave(int index, bool suppressReplaceSlot = false)
        {
            var meta = saveMetas[index];
#if UNITY_WEBPLAYER || UNITY_WEBGL
            webSaveBlob.saveJSONs.RemoveAt(index);
            var webBlogJSON = JsonUtility.ToJson(webSaveBlob);
            PlayerPrefs.SetString(currentSaveDataKey, webBlogJSON);
            PlayerPrefs.Save();
#else
            if (System.IO.File.Exists(meta.fileLocation))
            {
                System.IO.File.Delete(meta.fileLocation);
            }
#endif//UNITY_WEBPLAYER
            if(meta.saveName.StartsWith(FungusConstants.UserSavePrefix) && !suppressReplaceSlot)
            {
                saveMetas.Add(new SavePointMeta() { saveName = meta.saveName });
            }
            saveMetas.RemoveAt(index);
            SaveManagerSignals.DoSaveDeleted(meta.saveName);
            //TODO if user slot save then put dumby there
        }

        public void DeleteSave(SavePointMeta meta)
        {
            DeleteSave(saveMetas.IndexOf(meta));
        }

        /// <summary>
        /// Creates a new Save Point using a key and description, and adds it to the Save History.
        /// </summary>
        public virtual void Save(string saveName, string savePointDescription, bool isAutoSave = false)
        {
            SaveManagerSignals.DoSavePrepare(saveName, savePointDescription);

            var existingMetaIndex = SavePointNameToIndex(saveName);
            if (existingMetaIndex >= 0)
            {
                DeleteSave(existingMetaIndex, true);
            }
            
            var savePointDataJSON = SavePointData.EncodeToJson(saveName, savePointDescription, out SavePointData save);
            var fileName = GetFullSaveDir() + (isAutoSave ? FungusConstants.AutoSavePrefix : FungusConstants.UserSavePrefix) 
                + System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss.ffff") + FileExtension;
            GenerateMetaFromSave(fileName, save);
#if UNITY_WEBPLAYER || UNITY_WEBGL

#else
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
            System.IO.File.WriteAllText(fileName, savePointDataJSON, System.Text.Encoding.UTF8);
#endif
            //https://docs.unity3d.com/ScriptReference/ScreenCapture.CaptureScreenshot.html with unique name

            //if we limit autos and it is an auto, are there now to many, delete oldest until not over limit
            if (isAutoSave && saveSettings.NumberOfAutoSaves >= 0)
            {
                var autoSaves = CollectAutoSaves();

                for (int i = 0; i < autoSaves.Count - saveSettings.NumberOfAutoSaves; i++)
                {
                    DeleteSave(saveMetas.IndexOf(autoSaves[i]), true);
                }
            }

            //TODO save created
            SaveManagerSignals.DoSaveSaved(saveName, savePointDescription);
        }

        public virtual bool Load(SavePointMeta meta)
        {
            var saveContent = System.IO.File.ReadAllText(meta.fileLocation, System.Text.Encoding.UTF8);

            var savePointData = SavePointData.DecodeFromJSON(saveContent);

            if (!LoadSavePoint(savePointData))
            {
                Debug.LogError("Failed to Load " + meta.saveName);
                return false;
            }
            return true;
        }

        public virtual bool LoadSavePoint(SavePointData savePointData)
        {
            if (savePointData == null)
                return false;

            var markerKey = savePointData.ProgressMarkerName;

            UnityEngine.Events.UnityAction<Scene, LoadSceneMode> onSceneLoadedAction = null;

            onSceneLoadedAction = (scene, mode) =>
            {
                // Additive scene loads and non-matching scene loads could happen if the client is using the
                // SceneManager directly. We just ignore these events and hope they know what they're doing!
                if (mode == LoadSceneMode.Additive ||
                    scene.name != savePointData.SceneName)
                {
                    return;
                }

                SceneManager.sceneLoaded -= onSceneLoadedAction;

                // Look for a SaveData component in the scene to process the save data items.
                savePointData.RunDeserialize();

                SaveManagerSignals.DoSaveLoaded(savePointData.SaveName);

                // Execute Save Point Loaded event handlers with matching key.
                SaveLoaded.NotifyEventHandlers(savePointData.ProgressMarkerName);

                StartCoroutine(DelaySetNotLoading());
            };

            SceneManager.sceneLoaded += onSceneLoadedAction;
            IsSaveLoading = true;
            SaveManagerSignals.DoSavePreLoad(savePointData.SaveName);
            SceneManager.LoadScene(savePointData.SceneName);

            return true;
        }

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
            for (int i = saveMetas.Count-1; i >= 0; i--)
            {
                DeleteSave(i);
            }
        }

        public virtual SavePointMeta GetMostRecentSave()
        {
            if (SaveMetas.Count > 0)
            {
                var newestSaveTime = SaveMetas.Max(x => x.savePointLastWritten);

                return SaveMetas.FirstOrDefault(x => x.savePointLastWritten == newestSaveTime);
            }

            return null;
        }

        public List<SavePointMeta> CollectAutoSaves()
        {
            return FungusManager.Instance.SaveManager.SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.AutoSavePrefix))
               .OrderBy(x => x.savePointLastWritten.Ticks).ToList();
        }

        public List<SavePointMeta> CollectUserSaves()
        {
            return FungusManager.Instance.SaveManager.SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.UserSavePrefix))
               .OrderBy(x => x.saveName).ToList();
        }

        /// <summary>
        /// Handler function called when the Restart button is pressed.
        /// </summary>
        public virtual bool Restart()
        {
            if (string.IsNullOrEmpty(StartScene))
            {
                Debug.LogError("No start scene specified");
                return false;
            }

            // Reset the Save History for a new game
            if (saveSettings.RestartDeletesSave)
            {
                DeleteAllSaves();
                SaveManagerSignals.DoSaveReset();
            }

            SceneManager.LoadScene(StartScene);
            return true;
        }
    }
}