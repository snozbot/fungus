using UnityEngine;

namespace Fungus.SaveSystem
{
    /// <summary>
    /// Contains functionality common to SaveWriters and SaveReaders.
    /// </summary>
    public class SaveDiskAccessor : ScriptableObject
    {
        [SerializeField] protected ReadWriteEncoding encoding =     ReadWriteEncoding.Unicode;

        [Tooltip("The first part of the save files' names this works with.")]
        [SerializeField] protected string savePrefix =              "saveData";
        [Tooltip("Just for flavor.")]
        [SerializeField] protected string fileExtension =           "save";

    }
}