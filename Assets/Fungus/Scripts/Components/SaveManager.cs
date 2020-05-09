// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine.SceneManagement;
using UnityEngine;

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
        protected static SaveHistory saveHistory = new SaveHistory();

        public static string STORAGE_DIRECTORY { get { return Application.persistentDataPath + "/FungusSaves/"; } }

        private static string GetFullFilePath(string saveDataKey)
        {
            return STORAGE_DIRECTORY + saveDataKey + ".json";
        }

        protected virtual bool ReadSaveHistory(string saveDataKey)
        {
            var historyData = string.Empty;
#if UNITY_WEBPLAYER || UNITY_WEBGL
            historyData = PlayerPrefs.GetString(saveDataKey);
#else
            var fullFilePath = GetFullFilePath(saveDataKey);
            if (System.IO.File.Exists(fullFilePath))
            {
                historyData = System.IO.File.ReadAllText(fullFilePath);
            }
#endif//UNITY_WEBPLAYER
            if (!string.IsNullOrEmpty(historyData))
            {
                var tempSaveHistory = JsonUtility.FromJson<SaveHistory>(historyData);
                if (tempSaveHistory != null)
                {
                    saveHistory = tempSaveHistory;
                    return true;
                }
            }

            return false;
        }

        protected virtual bool WriteSaveHistory(string saveDataKey)
        {
            var historyData = JsonUtility.ToJson(saveHistory, true);
            if (!string.IsNullOrEmpty(historyData))
            {
#if UNITY_WEBPLAYER || UNITY_WEBGL
                PlayerPrefs.SetString(saveDataKey, historyData);
                PlayerPrefs.Save();
#else
                var fileLoc = GetFullFilePath(saveDataKey);
                
                //make sure the dir exists
                System.IO.FileInfo file = new System.IO.FileInfo(fileLoc);
                file.Directory.Create();
                
                System.IO.File.WriteAllText(fileLoc, historyData);
#endif//UNITY_WEBPLAYER
                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts Block execution based on a Save Point Key
        /// The execution order is:
        /// 1. Save Point Loaded event handlers with a matching key.
        /// 2. First Save Point command (in any Block) with matching key. Execution starts at the following command.
        /// 3. Any label in any block with name matching the key. Execution starts at the following command.
        /// </summary>
        protected virtual void ExecuteBlocks(string savePointKey)
        {
            // Execute Save Point Loaded event handlers with matching key.
            SavePointLoaded.NotifyEventHandlers(savePointKey);

            // Execute any block containing a SavePoint command matching the save key, with Resume On Load enabled
            var savePoints = UnityEngine.Object.FindObjectsOfType<SavePoint>();
            for (int i = 0; i < savePoints.Length; i++)
            {
                var savePoint = savePoints[i];
                if (savePoint.ResumeOnLoad &&
                    string.Compare(savePoint.SavePointKey, savePointKey, true) == 0)
                {
                    int index = savePoint.CommandIndex;
                    var block = savePoint.ParentBlock;
                    var flowchart = savePoint.GetFlowchart();
                    flowchart.ExecuteBlock(block, index + 1);

                    // Assume there's only one SavePoint using this key
                    break;
                }
            }
        }

        /// <summary>
        /// Starts execution at the first Save Point found in the scene with the IsStartPoint property enabled.
        /// </summary>
        protected virtual void ExecuteStartBlock()
        {
            // Each scene should have one Save Point with the IsStartPoint property enabled.
            // We automatically start execution from this command whenever the scene starts 'normally' (i.e. first play, restart or scene load via the Load Scene command or SceneManager.LoadScene).

            var savePoints = UnityEngine.Object.FindObjectsOfType<SavePoint>();
            for (int i = 0; i < savePoints.Length; i++)
            {
                var savePoint = savePoints[i];
                if (savePoint.IsStartPoint)
                {
                    savePoint.GetFlowchart().ExecuteBlock(savePoint.ParentBlock, savePoint.CommandIndex);
                    break;
                }
            }
        }

        protected virtual void LoadSavedGame(string saveDataKey)
        {
            if (ReadSaveHistory(saveDataKey))
            {
                saveHistory.ClearRewoundSavePoints();
                saveHistory.LoadLatestSavePoint();
            }
        }

        // Scene loading in Unity is asynchronous so we need to take care to avoid race conditions. 
        // The following callbacks tell us when a scene has been loaded and when 
        // a saved game has been loaded. We delay taking action until the next 
        // frame (via a delegate) so that we know for sure which case we're dealing with.

        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSavePointLoaded += OnSavePointLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSavePointLoaded -= OnSavePointLoaded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected virtual void OnSavePointLoaded(string savePointKey)
        {
            var key = savePointKey;
            loadAction = () => ExecuteBlocks(key);
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Ignore additive scene loads
            if (mode == LoadSceneMode.Additive)
            {
                return;
            }

            // We first assume that this is a 'normal' scene load rather than a saved game being loaded.
            // If we subsequently receive a notification that a saved game was loaded then the load action 
            // set here will be overridden by the OnSavePointLoaded callback above.

            if (loadAction == null)
            {
                loadAction = ExecuteStartBlock;
            }
        }

        protected System.Action loadAction;

        protected virtual void Start()
        {
            // The OnSceneLoaded callback above may not be called for the initial scene load in the game,
            // so we call ExecuteStartBlock when the SaveManager starts up too.
            if (loadAction == null)
            {
                loadAction = ExecuteStartBlock;
            }
        }

        protected virtual void Update()
        {
            // Execute any previously scheduled load action
            if (loadAction != null)
            {
                loadAction();
                loadAction = null;
            }
        }

        #region Public members

        /// <summary>
        /// The scene that should be loaded when restarting a game.
        /// </summary>
        public string StartScene { get; set; }

        /// <summary>
        /// Returns the number of Save Points in the Save History.
        /// </summary>
        public virtual int NumSavePoints { get { return saveHistory.NumSavePoints; } }

        /// <summary>
        /// Returns the current number of rewound Save Points in the Save History.
        /// </summary>
        public virtual int NumRewoundSavePoints { get { return saveHistory.NumRewoundSavePoints; } }

        /// <summary>
        /// Writes the Save History to persistent storage.
        /// </summary>
        public virtual void Save(string saveDataKey)
        {
            WriteSaveHistory(saveDataKey);
        }

        /// <summary>
        /// Loads the Save History from persistent storage and loads the latest Save Point.
        /// </summary>
        public void Load(string saveDataKey)
        {
            // Set a load action to be executed on next update
            var key = saveDataKey;
            loadAction = () => LoadSavedGame(key);
        }

        /// <summary>
        /// Deletes a previously stored Save History from persistent storage.
        /// </summary>
        public static void Delete(string saveDataKey)
        {
#if UNITY_WEBPLAYER || UNITY_WEBGL
            PlayerPrefs.DeleteKey(saveDataKey);
            PlayerPrefs.Save();
#else
            var fullFilePath = GetFullFilePath(saveDataKey);
            if (System.IO.File.Exists(fullFilePath))
            {
                System.IO.File.Delete(fullFilePath);
            }
#endif//UNITY_WEBPLAYER
        }

        /// <summary>
        /// Returns true if save data has previously been stored using this key.
        /// </summary>
        public bool SaveDataExists(string saveDataKey)
        {
#if UNITY_WEBPLAYER || UNITY_WEBGL
            return PlayerPrefs.HasKey(saveDataKey);
#else
            var fullFilePath = GetFullFilePath(saveDataKey);
            return System.IO.File.Exists(fullFilePath);
#endif//UNITY_WEBPLAYER
            }

        /// <summary>
        /// Creates a new Save Point using a key and description, and adds it to the Save History.
        /// </summary>
        public virtual void AddSavePoint(string savePointKey, string savePointDescription)
        {
            saveHistory.AddSavePoint(savePointKey, savePointDescription);

            SaveManagerSignals.DoSavePointAdded(savePointKey, savePointDescription);
        }

        /// <summary>
        /// Rewinds to the previous Save Point in the Save History and loads that Save Point.
        /// </summary>
        public virtual void Rewind()
        {
            if (saveHistory.NumSavePoints > 0)
            {
                // Rewinding the first save point is not permitted
                if (saveHistory.NumSavePoints > 1)
                {
                    saveHistory.Rewind();
                }

                saveHistory.LoadLatestSavePoint();
            }
        }

        /// <summary>
        /// Fast forwards to the next rewound Save Point in the Save History and loads that Save Point.
        /// </summary>
        public virtual void FastForward()
        {
            if (saveHistory.NumRewoundSavePoints > 0)
            {
                saveHistory.FastForward();
                saveHistory.LoadLatestSavePoint();
            }
        }

        /// <summary>
        /// Deletes all Save Points in the Save History.
        /// </summary>
        public virtual void ClearHistory()
        {
            saveHistory.Clear();
        }

        /// <summary>
        /// Returns an info string to help debug issues with the save data.
        /// </summary>
        /// <returns>The debug info.</returns>
        public virtual string GetDebugInfo()
        {
            return saveHistory.GetDebugInfo();
        }

#endregion
    }
}

#endif