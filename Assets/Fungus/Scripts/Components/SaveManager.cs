// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


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

        public List<SavePointMeta> SaveMetas { get { return saveMetas; } }

        [SerializeField] protected string currentSaveProfileKey = FungusConstants.DefaultSaveProfileKey;


        //#if UNITY_WEBPLAYER || UNITY_WEBGL
        [System.Serializable]
        public class WebSaveBlob
        {
            public List<string> saveJSONs = new List<string>();
        }

        [SerializeField] protected WebSaveBlob webSaveBlob = new WebSaveBlob();

        public static string STORAGE_DIRECTORY { get { return Application.persistentDataPath + "/FungusSaves/"; } }
        protected const string FileExtension = ".save";

        private string GetFullSaveDir()
        {
            return System.IO.Path.GetFullPath(STORAGE_DIRECTORY + currentSaveProfileKey + "/");
        }

        /// <summary>
        /// Starts Block execution based on a Save Point Key
        /// The execution order is:
        /// 1. Save Point Loaded event handlers with a matching key.
        /// 2. First Save Point command (in any Block) with matching key. Execution starts at the following command.
        /// 3. Any label in any block with name matching the key. Execution starts at the following command.
        /// </summary>
        protected virtual void ExecuteBlocks(string progressMarkerName)
        {
            // Execute Save Point Loaded event handlers with matching key.
            SaveLoaded.NotifyEventHandlers(progressMarkerName);

            // Execute any block containing a SavePoint command matching the save key, with Resume On Load enabled
            var savePoints = UnityEngine.Object.FindObjectsOfType<AutoSave>();
            for (int i = 0; i < savePoints.Length; i++)
            {
                var savePoint = savePoints[i];
                if (string.Compare(savePoint.CustomKey, progressMarkerName, true) == 0)
                {
                    savePoint.RequestResumeAfterLoad();

                    // Assume there's only one AutoSave using this key
                    break;
                }
            }
        }

        public string GetDebugInfo()
        {
            string retval = string.Empty;
            foreach (var item in saveMetas)
            {
                retval += item.saveName + "\n";
            }
            return retval;
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
            PopulateSaveMetas(currentSaveProfileKey);
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


        //TODO needs web version
        public void PopulateSaveMetas(string saveDataKey)
        {
            saveMetas.Clear();
            currentSaveProfileKey = saveDataKey;

            var dir = GetFullSaveDir();

            var foundFiles = System.IO.Directory.GetFiles(dir, "*" + FileExtension);

            foreach (var item in foundFiles)
            {
                var fileContents = System.IO.File.ReadAllText(item);
                var save = SavePointData.DecodeFromJSON(fileContents);
                GenerateMetaFromSave(item, save);
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
        public void DeleteSave(int index)
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
            saveMetas.RemoveAt(index);
        }

        public void DeleteSave(SavePointMeta meta)
        {
            DeleteSave(saveMetas.IndexOf(meta));
        }

        /// <summary>
        /// Creates a new Save Point using a key and description, and adds it to the Save History.
        /// </summary>
        public virtual void Save(string saveName, string savePointDescription)
        {
            var existingMetaIndex = SavePointNameToIndex(saveName);
            if (existingMetaIndex >= 0)
            {
                DeleteSave(existingMetaIndex);
            }

            string sceneName = SceneManager.GetActiveScene().name;
            var savePointDataJSON = SavePointData.EncodeToJson(saveName, savePointDescription, sceneName, out SavePointData save);
            var fileName = GetFullSaveDir() + System.DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss.ffff") + FileExtension;
            GenerateMetaFromSave(fileName, save);
#if UNITY_WEBPLAYER || UNITY_WEBGL

#else
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
            System.IO.File.WriteAllText(fileName, savePointDataJSON, System.Text.Encoding.UTF8);
#endif
            //https://docs.unity3d.com/ScriptReference/ScreenCapture.CaptureScreenshot.html with unique name

            //TODO save created
            SaveManagerSignals.DoSaveSaved(saveName, savePointDescription);
        }

        public virtual bool Load(string savePointKey)
        {
            var existingMetaIndex = SavePointNameToIndex(savePointKey);
            if (existingMetaIndex < 0)
            {
                Debug.LogError("Asked to Load save point of key " + savePointKey + " but none of that key exist.");

                return false;
            }

            return Load(saveMetas[existingMetaIndex]);
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

                ExecuteBlocks(markerKey);
                StartCoroutine(DelaySetNotLoading());
            };

            SceneManager.sceneLoaded += onSceneLoadedAction;
            IsSaveLoading = true;
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
            while(saveMetas.Count > 0)
            {
                DeleteSave(saveMetas.Count - 1);
            }
        }
    }
}

#endif