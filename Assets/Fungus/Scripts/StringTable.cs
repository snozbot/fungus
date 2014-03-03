using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	/**
	 * Stores long or frequently repeated strings in a dictionary.
	 * Strings can then be retrieved using a short key string.
	 */
	public class StringTable
	{
		Dictionary<string, string> stringTable = new Dictionary<string, string>();

		/**
		 * Removes all strings from the string table.
		 */
		public void ClearStringTable()
		{
			stringTable.Clear();
		}

		/**
		 * Retrieves a string from the table by key.
		 */
		public string GetString(string key)
		{
			if (stringTable.ContainsKey(key))
			{
				return stringTable[key];
			}
			return "";
		}
		
		/**
		 * Adds or updates a string in the table.
		 */
		public void SetString(string key, string value)
		{
			stringTable[key] = value;
		}

		/** 
		 * Replace keys in the input string with the string table entry.
		 * Example format: "This {string_key} string"
		 */
		public string SubstituteStrings(string text)
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
		public string FormatLinkText(string text)
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