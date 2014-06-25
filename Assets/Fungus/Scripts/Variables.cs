using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace Fungus
{
	/**
	 * Peristent data storage class for tracking game state.
	 * Provides a basic save game system.
	 */
	public class Variables
	{
		static string saveName = "_fungus";

		/**
		 * Sets a name to prefix before all keys used with Set & Get methods.
		 * You can use this to support multiple save data profiles, (e.g. for multiple users or a list of checkpoints).
		 */
		static public void SetSaveName(string _saveName)
		{
			saveName = _saveName;
		}

		/**
		 * Save the variable state to persistent storage.
		 * The currently loaded scene name is stored so that Game.LoadGame() will automatically move to the appropriate scene.
		 */
		static public void Save()
		{
			SetString("_scene", Application.loadedLevelName);
			PlayerPrefs.Save();
		}

		/**
		 * Deletes all stored variables.
		 */
		static public void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		/**
		 * Deletes a single stored variable.
		 */
		static public void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(AddPrefix(key));
		}

		/**
		 * Returns the float variable associated with the key.
		 */
		static public float GetFloat(string key)
		{
			return PlayerPrefs.GetFloat(AddPrefix(key));
		}

		/**
		 * Returns the integer variable associated with the key.
		 */
		static public int GetInteger(string key)
		{
			return PlayerPrefs.GetInt(AddPrefix(key));
		}

		/**
		 * Returns the boolean variable associated with the key.
		 */
		static public bool GetBoolean(string key)
		{
			return (bool)(PlayerPrefs.GetInt(AddPrefix(key)) != 0);
		}

		/**
		 * Returns the string variable associated with the key.
		 */
		static public string GetString(string key)
		{
			return PlayerPrefs.GetString(AddPrefix(key));
		}

		/**
		 * Returns true if a variable has been stored with this key.
		 */
		static public bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(AddPrefix(key));
		}

		/**
		 * Stores a float variable using the key.
		 */
		static public void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(AddPrefix(key), value);
		}

		/**
		 * Stores an integer variable using the key.
		 */
		static public void SetInteger(string key, int value)
		{
			PlayerPrefs.SetInt(AddPrefix(key), value);
		}

		/**
		 * Stores a boolean variable using the key.
		 */
		static public void SetBoolean(string key, bool value)
		{
			PlayerPrefs.SetInt(AddPrefix(key), value ? 1 : 0);
		}

		/**
		 * Stores a string variable using the key.
		 */
		static public void SetString(string key, string value)
		{
			PlayerPrefs.SetString(AddPrefix(key), value);
		}

		/** 
		 * Replace keys in the input string with the string table entry.
		 * Example format: "This {string_key} string"
		 */
		static public string SubstituteStrings(string text)
		{
			string subbedText = text;
			
			// Instantiate the regular expression object.
			Regex r = new Regex("{.*?}");
			
			// Match the regular expression pattern against a text string.
			var results = r.Matches(text);
			foreach (Match match in results)
			{
				string stringKey = match.Value.Substring(1, match.Value.Length - 2);
				string stringValue = GetString(stringKey);
				
				subbedText = subbedText.Replace(match.Value, stringValue);
			}
			
			return subbedText;
		}
		
		/**
		 * Chops a string at the first new line character encountered.
		 * This is useful for link / button strings that must fit on a single line.
		 */
		static public string FormatLinkText(string text)
		{
			string trimmed;
			if (text.Contains("\n"))
			{
				trimmed = text.Substring(0, text.IndexOf("\n"));
			}
			else
			{
				trimmed = text;
			}
			
			return trimmed;
		}

		static string AddPrefix(string key)
		{
			if (saveName.Length == 0)
			{
				return key;
			}
			
			return saveName + "." + key;
		}
	}
}