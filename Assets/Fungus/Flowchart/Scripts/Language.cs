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
	public class Language : MonoBehaviour
	{
		/**
		 * Currently active language, usually defined by a two letter language code (e.g DE = German)
		 */
		public string activeLanguage = "";

		protected Dictionary<string, string> localizedStrings = new Dictionary<string, string>();

		/**
		 * Temp storage for a single item of standard text and its localizations
		 */
		protected class LanguageItem
		{
			public string timeStamp;
			public string description;
			public string standardText;
			public Dictionary<string, string> localizedStrings = new Dictionary<string, string>();
		}

		/**
		 * CSV file containing localization data
		 */
		public TextAsset localizationFile;

		public virtual void Start()
		{
			if (activeLanguage.Length > 0 &&
			    localizationFile != null &&
			    localizationFile.text.Length > 0)
			{
				SetActiveLanguage(activeLanguage, localizationFile.text);
			}
		}

		/**
		 * Export all localized strings to an easy to edit CSV file.
		 */
		public virtual string ExportCSV()
		{
			// Collect all the language items present in the scene
			Dictionary<string, LanguageItem> languageItems = FindLanguageItems();

			// Update language items with localization data from CSV file
			if (localizationFile != null &&
			    localizationFile.text.Length > 0)
			{
				AddLocalisedStrings(languageItems, localizationFile.text);
			}

			// Build CSV header row and a list of the language codes currently in use
			string csvHeader = "Key,Timestamp,Description,Standard";
			List<string> languageCodes = new List<string>();
			foreach (LanguageItem languageItem in languageItems.Values)
			{
				foreach (string languageCode in languageItem.localizedStrings.Keys)
				{
					if (!languageCodes.Contains(languageCode))
					{
						languageCodes.Add(languageCode);
						csvHeader += "," + languageCode;
					}
				}
			}

			// Build the CSV file using collected language items
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
					if (languageItem.localizedStrings.ContainsKey(languageCode))
					{
						row += "," + CSVSupport.Escape(languageItem.localizedStrings[languageCode]);
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

		protected virtual void AddLocalisedStrings(Dictionary<string, LanguageItem> languageItems, string csvData)
		{
			// Split into lines
			// Excel on Mac exports csv files with \r line endings, so we need to support that too.
			string[] lines = csvData.Split('\n', '\r');

			if (lines.Length == 0)
			{
				// Early out if no data in file
				return;
			}
			
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

				// Store localized strings for this string id
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
						if (languageItems.ContainsKey(stringId))
						{
							languageItems[stringId].localizedStrings[languageCode] = languageEntry;
						}
					}
				}
			}
		}

		public virtual void SetActiveLanguage(string languageCode, string csvData)
		{
			if (!Application.isPlaying)
			{
				// This function should only ever be called when the game is playing (not in editor).
				return;
			}

			localizedStrings.Clear();

			// Split into lines
			// Excel on Mac exports csv files with \r line endings, so we need to support that too.
			string[] lines = csvData.Split('\n', '\r');
			
			if (lines.Length == 0)
			{
				// No data rows in file
				return;
			}
			
			// Parse header row
			string[] columnNames = CSVSupport.SplitCSVLine(lines[0]);

			if (columnNames.Length < 5)
			{
				// No languages defined in CSV file
				return;
			}

			int languageIndex = -1;
			for (int i = 4; i < columnNames.Length; ++i)
			{
				if (columnNames[i] == languageCode)
				{
					languageIndex = i;
					break;
				}
			}

			if (languageIndex == -1)
			{
				// Language not found
				return;
			}

			for (int i = 1; i < lines.Length; ++i)
			{
				string line = lines[i];
				
				string[] fields = CSVSupport.SplitCSVLine(line);
				if (fields.Length < languageIndex + 1)
				{
					continue;
				}
				
				string stringId = fields[0];
				string languageEntry = CSVSupport.Unescape(fields[languageIndex]);
					
				if (languageEntry.Length > 0)
				{
					localizedStrings[stringId] = languageEntry;
					PopulateGameString(stringId, languageEntry);
				}
			}
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

		/**
		 * Import strings from a CSV file.
		 * 1. Any changes to standard text items will be applied to the corresponding scene object.
		 * 2. Any localized strings will be added to the localization dictionary.
		 */
		public virtual void ImportCSV(string csvData)
		{
			/*
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
			*/
		}
	}

}