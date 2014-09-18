using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if !UNITY_WINRT
using System.Runtime.Serialization.Formatters.Binary;
#endif
using System.IO;

namespace Fungus
{
	/**
	 * Static data storage class for managing global game variables.
	 * Provides save and load functionality for persistent storage between game sessions.
	 */
	public class GlobalVariables
	{
		protected static Dictionary<string, string> stringDict = new Dictionary<string, string>();
		protected static Dictionary<string, int> intDict = new Dictionary<string, int>();
		protected static Dictionary<string, float> floatDict = new Dictionary<string, float>();
		protected static Dictionary<string, bool> boolDict = new Dictionary<string, bool>();

		/**
		 * Save the variable dictionaries to persistent storage using a name tag.
		 */
		public static void Save(string saveName)
		{
#if !UNITY_WINRT
			// Save strings
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream();
				b.Serialize(m, stringDict);
				PlayerPrefs.SetString(saveName + "." + "stringDict", Convert.ToBase64String(m.GetBuffer()));
			}

			// Save ints
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream();
				b.Serialize(m, intDict);
				PlayerPrefs.SetString(saveName + "." + "intDict", Convert.ToBase64String(m.GetBuffer()));
			}

			// Save floats
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream();
				b.Serialize(m, floatDict);
				PlayerPrefs.SetString(saveName + "." + "floatDict", Convert.ToBase64String(m.GetBuffer()));
			}

			// Save bools
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream();
				b.Serialize(m, boolDict);
				PlayerPrefs.SetString(saveName + "." + "boolDict", Convert.ToBase64String(m.GetBuffer()));
			}

			PlayerPrefs.Save();
#endif
		}

		/**
		 * Loads the variable dictionaries from persistent storage using a name tag.
		 */
		public static void Load(string saveName)
		{
#if !UNITY_WINRT
			var stringData = PlayerPrefs.GetString(saveName + "." + "stringDict");
			if (string.IsNullOrEmpty(stringData))
			{
				stringDict.Clear();
			}
			else
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream(Convert.FromBase64String(stringData));
				stringDict = (Dictionary<string, string>)b.Deserialize(m);
			}

			var floatData = PlayerPrefs.GetString(saveName + "." + "floatDict");
			if (!string.IsNullOrEmpty(floatData))
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream(Convert.FromBase64String(floatData));
				floatDict = b.Deserialize(m) as Dictionary<string, float>;
			}

			var intData = PlayerPrefs.GetString(saveName + "." + "intDict");
			if (!string.IsNullOrEmpty(intData))
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream(Convert.FromBase64String(intData));
				intDict = b.Deserialize(m) as Dictionary<string, int>;
			}
		
			var boolData = PlayerPrefs.GetString(saveName + "." + "boolDict");
			if (!string.IsNullOrEmpty(boolData))
			{
				var b = new BinaryFormatter();
				var m = new MemoryStream(Convert.FromBase64String(boolData));
				boolDict = b.Deserialize(m) as Dictionary<string, bool>;
			}
#endif
		}

		/**
		 * Clears all stored variables.
		 */
		public static void ClearAll()
		{
			stringDict.Clear();
			intDict.Clear();
			floatDict.Clear();
			boolDict.Clear();
		}

		/**
		 * Returns the float variable associated with the key.
		 */
		public static float GetFloat(string key)
		{
			if (String.IsNullOrEmpty(key) ||
				!floatDict.ContainsKey(key))
			{
				return 0;
			}

			return floatDict[key];
		}

		/**
		 * Returns the integer variable associated with the key.
		 */
		public static int GetInteger(string key)
		{
			if (intDict == null)
			{
				Debug.Log ("Dict is null somehow");
			}

			if (String.IsNullOrEmpty(key) ||
				!intDict.ContainsKey(key))
			{
				return 0;
			}
			
			return intDict[key];
		}

		/**
		 * Returns the boolean variable associated with the key.
		 */
		public static bool GetBoolean(string key)
		{
			if (String.IsNullOrEmpty(key) ||
				!boolDict.ContainsKey(key))
			{
				return false;
			}
			
			return boolDict[key];
		}

		/**
		 * Returns the string variable associated with the key.
		 */
		public static string GetString(string key)
		{
			if (String.IsNullOrEmpty(key) ||
				!stringDict.ContainsKey(key))
			{
				return "";
			}
			
			return stringDict[key];
		}

		/**
		 * Stores a float variable using the key.
		 */
		public static void SetFloat(string key, float value)
		{
			if (stringDict.ContainsKey(key) ||
			    intDict.ContainsKey(key) ||
			    boolDict.ContainsKey(key)) 
			{
				Debug.LogError("Key already in use with a string, integer or boolean variable");
				return;
			}

			floatDict[key] = value;
		}

		/**
		 * Stores an integer variable using the key.
		 */
		public static void SetInteger(string key, int value)
		{
			if (stringDict.ContainsKey(key) ||
			    floatDict.ContainsKey(key) ||
			    boolDict.ContainsKey(key)) 
			{
				Debug.LogError("Key already in use with a string, float or boolean variable");
				return;
			}

			intDict[key] = value;
		}

		/**
		 * Stores a boolean variable using the key.
		 */
		public static void SetBoolean(string key, bool value)
		{
			if (stringDict.ContainsKey(key) ||
			    floatDict.ContainsKey(key) ||
			    intDict.ContainsKey(key)) 
			{
				Debug.LogError("Key already in use with a string, float or integer variable");
				return;
			}

			boolDict[key] = value;
		}

		/**
		 * Stores a string variable using the key.
		 */
		public static void SetString(string key, string value)
		{
			if (boolDict.ContainsKey(key) ||
			    floatDict.ContainsKey(key) ||
			    intDict.ContainsKey(key)) 
			{
				Debug.LogError("Key already in use with a boolean, float or integer variable");
				return;
			}

			stringDict[key] = value;
		}

		/** 
		 * Replace keys in the input string with the string table entry.
		 * Example format: "This {string_key} string"
		 */
		public static string SubstituteStrings(string text)
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
		public static string FormatLinkText(string text)
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
	}
}