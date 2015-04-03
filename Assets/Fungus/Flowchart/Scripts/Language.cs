using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	interface ILocalizable
	{
		string GetLocalizationID();

		string GetStandardText();

		void SetStandardText(string standardText);

		string GetTimestamp();

		string GetDescription();
	}

	/**
	 * Multi-language localization support.
	 */
	public class Language : MonoBehaviour, ISerializationCallbackReceiver
	{
		/**
		 * Currently active language, usually defined by a two letter language code (e.g DE = German)
		 */
		public string activeLanguage = "";

		[SerializeField]
		protected List<string> keys;

		[SerializeField]
		protected List<string> values;

		// We store the localized strings in a dictionary for easy lookup, but use lists for serialization
		// http://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnBeforeSerialize.html
		protected Dictionary<string, string> localizedStrings = new Dictionary<string, string>();

		// Gameobjects that are being managed for localization.
		// Each game object should have a child component that implements ISerializable
		// As Unity doesn't provide a persistant object identifier, we use the index
		// in this list as a way to uniquely identify string objects
		public List<GameObject> localizedObjects = new List<GameObject>();

		/**
		 * Temp storage for a single item of standard text read from a scene object.
		 */
		protected class LanguageItem
		{
			public string timeStamp;
			public string description;
			public string standardText;
		}

		public virtual void Start()
		{
			if (activeLanguage.Length > 0)
			{
				SetActiveLanguage(activeLanguage);
			}
		}

		/**
		 * Export all localized strings to an easy to edit CSV file.
		 */
		public virtual string ExportCSV()
		{
			// Build a list of the language codes currently in use
			string csvHeader = "Key,Timestamp,Description,Standard";
			List<string> languageCodes = FindLanguageCodes();
			foreach (string languageCode in languageCodes)
			{
				csvHeader += "," + languageCode;
			}

			// Collect all the language items present in the scene
			Dictionary<string, LanguageItem> languageItems = FindLanguageItems();

			// Build the CSV file using collected language items and the corresponding store localized strings
			string csvData = csvHeader + "\n";
			foreach (string stringId in languageItems.Keys)
			{
				LanguageItem languageItem = languageItems[stringId];

				string row = CSVSupport.Escape(stringId);
				row += "," + CSVSupport.Escape(languageItem.timeStamp);
				row += "," + CSVSupport.Escape(languageItem.description);
				row += "," + CSVSupport.Escape(languageItem.standardText);

				foreach (string languageCode in languageCodes)
				{
					string key = stringId + "." + languageCode;
					if (localizedStrings.ContainsKey(key))
					{
						row += "," + CSVSupport.Escape(localizedStrings[key]);
					}
					else
					{
						row += ","; // Empty field
					}
				}

				csvData += row + "\n";
			}

			return csvData;
		}

		/**
		 * Import strings from a CSV file.
		 * 1. Any changes to standard text items will be applied to the corresponding scene object.
		 * 2. Any localized strings will be added to the localization dictionary.
		 */
		public virtual void ImportCSV(string csvData)
		{
			// Split into lines
			// Excel on Mac exports csv files with \r line endings, so we need to support that too.
			string[] lines = csvData.Split('\n', '\r');

			if (lines.Length == 0)
			{
				return;
			}

			localizedStrings.Clear();

			// Parse header row
			string[] columnNames = CSVSupport.SplitCSVLine(lines[0]);

			for (int i = 1; i < lines.Length; ++i)
			{
				string line = lines[i];

				string[] fields = CSVSupport.SplitCSVLine(line);
				if (fields.Length < 4)
				{
					continue;
				}

				string stringId = fields[0];
				// Ignore timestamp & notes fields
				string standardText = CSVSupport.Unescape(fields[3]);

				PopulateGameString(stringId, standardText);

				// Store localized string in stringDict
				for (int j = 4; j < fields.Length; ++j)
				{
					if (j >= columnNames.Length)
					{
						continue;
					}
					string languageCode = columnNames[j];
					string languageEntry = CSVSupport.Unescape(fields[j]);

					if (languageEntry.Length > 0)
					{
						// The dictionary key is the basic string id with .<LanguageCode> appended
						localizedStrings[stringId + "." + languageCode] = languageEntry;
					}
				}
			}
		}

		/**
		 * Search through the scene 
		 */
		protected Dictionary<string, LanguageItem> FindLanguageItems()
		{
			Dictionary<string, LanguageItem> languageItems = new Dictionary<string, LanguageItem>();

			// Export all Say and Menu commands in the scene
			Flowchart[] flowcharts = GameObject.FindObjectsOfType<Flowchart>();
			foreach (Flowchart flowchart in flowcharts)
			{
				Block[] blocks = flowchart.GetComponentsInChildren<Block>();
				foreach (Block block in blocks)
				{
					foreach (Command command in block.commandList)
					{
						string stringID = "";
						string standardText = "";
						
						System.Type type = command.GetType();
						if (type == typeof(Say))
						{
							stringID = "SAY." + flowchart.name + "." + command.itemId;
							Say sayCommand = command as Say;
							standardText = sayCommand.storyText;
						}
						else if (type == typeof(Menu))
						{							
							stringID = "MENU." + flowchart.name + "." + command.itemId;
							Menu menuCommand = command as Menu;
							standardText = menuCommand.text;
						}
						else
						{
							continue;
						}

						LanguageItem languageItem = null;
						if (languageItems.ContainsKey(stringID))
						{
							languageItem = languageItems[stringID];
						}
						else
						{
							languageItem = new LanguageItem();
							languageItems[stringID] = languageItem;
						}
						    
						// Update basic properties,leaving localised strings intact
						languageItem.timeStamp = "10/10/2015";
						languageItem.description = "Note";
						languageItem.standardText = standardText;
					}
				}
			}

			return languageItems;
		}

		public virtual void PopulateGameString(string stringId, string text)
		{
			string[] idParts = stringId.Split('.');
			if (idParts.Length == 0)
			{
				return;
			}

			string stringType = idParts[0];
			if (stringType == "SAY")
			{
				if (idParts.Length != 3)
				{
					return;
				}

				string flowchartName = idParts[1];
				int itemId = int.Parse(idParts[2]);

				GameObject go = GameObject.Find(flowchartName);
				Flowchart flowchart = go.GetComponentInChildren<Flowchart>();
				if (flowchart != null)
				{
					foreach (Say say in flowchart.GetComponentsInChildren<Say>())
					{
						if (say.itemId == itemId)
						{
							say.storyText = text;
						}
					}
				}
			}
			else if (stringType == "MENU")
			{
				if (idParts.Length != 3)
				{
					return;
				}

				string flowchartName = idParts[1];
				int itemId = int.Parse(idParts[2]);
				
				GameObject go = GameObject.Find(flowchartName);
				Flowchart flowchart = go.GetComponentInChildren<Flowchart>();
				if (flowchart != null)
				{
					foreach (Menu menu in flowchart.GetComponentsInChildren<Menu>())
					{
						if (menu.itemId == itemId)
						{
							menu.text = text;
						}
					}
				}
			}
		}

		public virtual void SetActiveLanguage(string languageCode)
		{
			// This function should only ever be called when the game is playing (not in editor).
			// If it was called in the editor it would permanently modify the text properties in the scene objects.
			if (!Application.isPlaying)
			{
				return;
			}

			List<string> languageCodes = FindLanguageCodes();
			if (!languageCodes.Contains(languageCode))
			{
				Debug.LogWarning("Language code " + languageCode + " not found.");
			}

			// Find all string keys that match the language code and populate the corresponding game object
			foreach (string key in localizedStrings.Keys)
			{
				if (GetLanguageId(key) == languageCode)
				{
					PopulateGameString(GetStringId(key), localizedStrings[key]);
				}
			}
		}

		public void OnBeforeSerialize()
		{
			keys.Clear();
			values.Clear();
			foreach (string key in localizedStrings.Keys)
			{
				string value = localizedStrings[key];
				keys.Add(key);
				values.Add(value);
			}
		}
		
		public void OnAfterDeserialize()
		{
			// Both arrays should be the same length, but use the min length just in case
			int minCount = Math.Min(keys.Count, values.Count);

			// Populate the string dict
			localizedStrings.Clear();
			for (int i = 0; i < minCount; ++i)
			{
				string key = keys[i];
				string value = values[i];
				localizedStrings[key] = value;
			}
		}

		protected virtual string GetStringId(string key)
		{
			int lastDotIndex = key.LastIndexOf(".");
			if (lastDotIndex <= 0 ||
			    lastDotIndex == key.Length - 1)
			{
				// Malformed key
				return "";
			}
			
			return key.Substring(0, lastDotIndex);
		}

		protected virtual string GetLanguageId(string key)
		{
			int lastDotIndex = key.LastIndexOf(".");
			if (lastDotIndex <= 0 ||
			    lastDotIndex == key.Length - 1)
			{
				// Malformed key
				return "";
			}

			return key.Substring(lastDotIndex + 1, key.Length - lastDotIndex - 1);
		}

		protected virtual List<string> FindLanguageCodes()
		{
			// Build a list of the language codes actually in use
			List<string> languageCodes = new List<string>();
			foreach (string key in keys)
			{
				string languageId = GetLanguageId(key);
				if (!languageCodes.Contains(languageId))
				{
					languageCodes.Add(languageId);
				}
			}

			return languageCodes;
		}
	}

}