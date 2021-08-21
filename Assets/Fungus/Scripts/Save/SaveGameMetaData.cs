// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Meta data about a save that has found by a save manager.
    /// These exist to prevent the Save Manager from keeping potentially a lot of very large json files in ram.
    /// </summary>
    public class SaveGameMetaData
    {
        public string saveName, category;
        public string description;
        public System.DateTime lastWritten;
        public string fileLocation;
        public string progressMarker;

        public string GetReadableTime()
        {
            return lastWritten.ToString("O");
        }
    }
}
