// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Wrapper class for PlayerPrefs that adds the concept of multiple save slots.
    /// Save slots allow you to store multiple player save profiles.
    /// </summary>
    public static class FungusPrefs
    {
        #region Public members

        /// <summary>
        /// Deletes all saved values for all slots.
        /// </summary>
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// Removes key and its value from this save slot.
        /// </summary>
        public static void DeleteKey(int slot, string key)
        {
            string slotKey = GetSlotKey(slot, key);
            PlayerPrefs.DeleteKey(slotKey);
        }

        /// <summary>
        /// Returns the float value associated with this key in this save slot, it it exists.
        /// </summary>
        public static float GetFloat(int slot, string key, float defaultValue = 0f)
        {
            string slotKey = GetSlotKey(slot, key);
            return PlayerPrefs.GetFloat(slotKey, defaultValue);
        }
     
        /// <summary>
        /// Returns the int value associated with this key in this save slot, it it exists.
        /// </summary>
        public static int GetInt(int slot, string key, int defaultValue = 0)
        {
            string slotKey = GetSlotKey(slot, key);
            return PlayerPrefs.GetInt(slotKey, defaultValue);
        }

        /// <summary>
        /// Returns the string value associated with this key in this save slot, it it exists.
        /// </summary>
        public static string GetString(int slot, string key, string defaultValue = "")
        {
            string slotKey = GetSlotKey(slot, key);
            return PlayerPrefs.GetString(slotKey, defaultValue);
        }

        /// <summary>
        /// Returns true if the key exists in this save slot.
        /// </summary>
        public static bool HasKey(int slot, string key)
        {
            string slotKey = GetSlotKey(slot, key);
            return PlayerPrefs.HasKey(slotKey);
        }

        /// <summary>
        /// Writes all modified prefences to disk.
        /// </summary>
        public static void Save()
        {
            PlayerPrefs.Save();        
        }

        /// <summary>
        /// Sets the value of the preference identified by key for this save slot.
        /// </summary>
        public static void SetFloat(int slot, string key, float value)
        {
            string slotKey = GetSlotKey(slot, key);
            PlayerPrefs.SetFloat(slotKey, value);
        }

        /// <summary>
        /// Sets the value of the preference identified by key for this save slot.
        /// </summary>
        public static void SetInt(int slot, string key, int value)
        {
            string slotKey = GetSlotKey(slot, key);
            PlayerPrefs.SetInt(slotKey, value);
        }

        /// <summary>
        /// Sets the value of the preference identified by key for this save slot.
        /// </summary>
        public static void SetString(int slot, string key, string value)
        {
            string slotKey = GetSlotKey(slot, key);
            PlayerPrefs.SetString(slotKey, value);
        }

        /// <summary>
        /// Returns the combined key used to identify a key within a save slot.
        /// </summary>
        private static string GetSlotKey(int slot, string key)
        {
            return slot.ToString() + ":" + key;
        }

        #endregion
    }
}