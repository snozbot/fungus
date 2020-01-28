using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

//todo cleanup, update doco, move to fungus, rename

namespace Fungus.SaveSystem
{
    [CreateAssetMenu()]
    /// <summary>
    /// Contains functionality common to SaveWriters and SaveReaders.
    /// </summary>
    public class SaveDiskAccessor : ScriptableObject
    {
        //[SerializeField] protected ReadWriteEncoding encoding = ReadWriteEncoding.Unicode;

        //[Tooltip("The first part of the save files' names this works with.")]
        //[SerializeField] protected string savePrefix = "saveData";
        protected const string SavePrefix = "saveData";
        protected const string FileExtension = "save"; 
        protected const string writeFileNameFormatPattern = "{0}_0{1}.{2}";
        protected const string filePathFormat = "{0}/{1}";
        protected readonly string readFileNameFormatPattern = ".{" + SavePrefix.Length + "}" +
                                                           "_0" +
                                                           @"\d*" +
                                                           @"\..{" + FileExtension.Length + "}$";

        //[Tooltip("Just for flavor.")]
        //[SerializeField] protected string fileExtension = "save";


        //[Tooltip("Set this to true if you want it to read non-plaintext-written save data. Experimental.")]
        //[SerializeField] protected bool readEncrypted;

        //protected System.Text.Encoding actualEncoding; // The System.Text.Encoding class isn't serializable, so...

        //protected string fileNameFormat; // Helps optimize finding files to read

        /// <summary>
        /// Invoked when this particular SaveWriter reads GameSaveData.
        /// Params: saveData, filePath, fileName
        /// </summary>
        public UnityAction<GameSaveData, string, string> GameSaveRead = delegate { };

        /// <summary>
        /// Invoked when this particular SaveWriter writes GameSaveData.
        /// Params: saveData, filePath, fileName
        /// </summary>
        public UnityAction<GameSaveData, string, string> GameSaveWritten = delegate { };

        /// <summary>
        /// Reads a save file at the passed filePath, returning a GameSaveData if appropriate.
        /// </summary>
        public virtual GameSaveData ReadOneFromDisk(string filePath)
        {
            // Safety.
            if (!File.Exists(filePath)) // change it to File.Exists later
            {
                var messageFormat = "Read error: file path doesn't exist. {0}";
                var message = string.Format(messageFormat, filePath);
                throw new System.ArgumentException(message);
            }

            // Read the file's contents into a json string...
            var jsonSaveData = File.ReadAllText(filePath);

            // ... then make sure it worked as intended.
            var saveData = JsonUtility.FromJson<GameSaveData>(jsonSaveData);
            ValidateReadSaveData(saveData, filePath);

            // Alert listeners
            var fileNameIndex = filePath.LastIndexOf('/') + 1;
            var fileName = filePath.Substring(fileNameIndex);
            Signals.GameSaveRead.Invoke(saveData, filePath, fileName);
            GameSaveRead.Invoke(saveData, filePath, fileName);
            return saveData;
        }

        /// <summary>
        /// Reads GameSaveDatas from the passed directory, and into the passed collection
        /// (if appropriate).
        /// </summary>
        public virtual void ReadAllFromDisk(string saveDir, ICollection<GameSaveData> outputTo = null)
        {
            // Safety.
            if (!Directory.Exists(saveDir))
            {
                var messageFormat =
                @"Could not read saves from {0}; that directory does not exist.";
                var message = string.Format(messageFormat, saveDir);
                throw new System.ArgumentException(message);
            }

            // Get all the locations for the files this is meant to read.
            var directories = new List<string>(Directory.GetFiles(saveDir));
            directories.RemoveAll(ShouldBeIgnored);

            // Extract GameSaveDatas from the files, adding them to the passed output
            // container if appropriate.
            var directory = "";
            var passOutput = outputTo != null;

            for (int i = 0; i < directories.Count; i++)
            {
                directory = directories[i];
                var saveData = ReadOneFromDisk(directory);
                if (passOutput)
                    outputTo.Add(saveData);
            }
        }

        protected virtual bool ShouldBeIgnored(string filePath)
        {
            // All depends on the file name fitting this reader's pattern thereof.
            var fileNameIndex = filePath.LastIndexOf('/') + 1;
            var fileName = filePath.Substring(fileNameIndex);

            return !Regex.IsMatch(fileName, readFileNameFormatPattern);
        }

        protected virtual void ValidateReadSaveData(GameSaveData readData, string filePath)
        {
            // If the essential fields aren't filled, then there was a read error.
            if (string.IsNullOrEmpty(readData.SceneName) ||
                readData.Items.Count == 0)
                throw new System.FormatException("Save data at " + filePath + " is not valid.");
        }

                
        /// <summary>
        /// Writes the passed save data to the passed save directory, returning true if successful, or
        /// false otherwise.
        /// </summary>
        public virtual bool WriteOneToDisk(GameSaveData saveData, string saveDir)
        {
            // Safety.
            if (!Directory.Exists(saveDir))
            {
                var messageFormat =
                @"Could not write save to {0}; that directory does not exist.";
                var message = string.Format(messageFormat, saveDir);
                Debug.LogError(message);
                return false;
            }

            var dataToWrite = JsonUtility.ToJson(saveData);

            // Write the file at the appropriate directory with the appropriate writing method.
            var fileName = string.Format(writeFileNameFormatPattern,
                                         SavePrefix, saveData.SlotNumber,
                                                              FileExtension);
            var filePath = string.Format(filePathFormat, saveDir, fileName);

            File.WriteAllText(filePath, dataToWrite, System.Text.Encoding.UTF8);
            
            Signals.GameSaveWritten.Invoke(saveData, filePath, fileName);
            GameSaveWritten.Invoke(saveData, filePath, fileName);
            return true;
        }

        /// <summary>
        /// Writes the passed save data to the passed save directory, returning true if successful,
        /// and false otherwise. If successful, the saved file's location is put into the passed outputDir.
        /// </summary>
        public virtual bool WriteOneToDisk(GameSaveData saveData, string saveDir, out string outputDir)
        {
            var success = WriteOneToDisk(saveData, saveDir);
            outputDir = "";

            if (success)
            {
                var fileName = string.Format(writeFileNameFormatPattern,
                                             SavePrefix, saveData.SlotNumber,
                                                                  FileExtension);
                var filePath = string.Format(filePathFormat, saveDir, fileName);
                outputDir = filePath;
            }

            return success;
        }

        /// <summary>
        /// Writes all the save datas to the passed save directory, returning true if successful,
        /// false otherwise.
        /// </summary>
        public virtual bool WriteAllToDisk(IList<GameSaveData> saveDatas, string saveDir)
        {
            var success = false;

            for (int i = 0; i < saveDatas.Count; i++)
            {
                success = WriteOneToDisk(saveDatas[i], saveDir);

                if (!success)
                    return false;
            }

            return true;
        }
    }
}