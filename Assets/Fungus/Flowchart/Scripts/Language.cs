using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ideafixxxer.CsvParser;

namespace Fungus
{

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
			string csvHeader = "Key,Description,Standard";
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

			// Export all character names
			foreach (Character character in GameObject.FindObjectsOfType<Character>())
			{
				LanguageItem languageItem = new LanguageItem();
				languageItem.standardText = character.nameText;
				languageItem.description = character.description;
				languageItems["CHARACTER." + character.nameText] = languageItem;
			}

			// Export all Say and Menu commands in the scene
			// To make it easier to localize, we preserve the command order in each exported block.
			Flowchart[] flowcharts = GameObject.FindObjectsOfType<Flowchart>();
			foreach (Flowchart flowchart in flowcharts)
			{
				// Have to set a unique localization id to export strings
				if (flowchart.localizationId.Length == 0)
				{
					continue;
				}

				Block[] blocks = flowchart.GetComponentsInChildren<Block>();
				foreach (Block block in blocks)
				{
					foreach (Command command in block.commandList)
					{
						string stringID = "";
						string standardText = "";
						string description = "";

						System.Type type = command.GetType();
						if (type == typeof(Say))
						{
							stringID = "SAY." + flowchart.localizationId + "." + command.itemId;
							Say sayCommand = command as Say;
							standardText = sayCommand.storyText;
							description = sayCommand.description;
						}
						else if (type == typeof(Menu))
						{							
							stringID = "MENU." + flowchart.localizationId + "." + command.itemId;
							Menu menuCommand = command as Menu;
							standardText = menuCommand.text;
							description = menuCommand.description;
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
						languageItem.standardText = standardText;
						languageItem.description = description;
					}
				}
			}
			
			return languageItems;
		}

		protected virtual void AddLocalisedStrings(Dictionary<string, LanguageItem> languageItems, string csvData)
		{
			CsvParser csvParser = new CsvParser();
			string[][] csvTable = csvParser.Parse(csvData);

			if (csvTable.Length <= 1)
			{
				// No data rows in file
				return;
			}

			// Parse header row
			string[] columnNames = csvTable[0];
			
			for (int i = 1; i < csvTable.Length; ++i)
			{
				string[] fields = csvTable[i];
				if (fields.Length < 4)
				{
					// No localized string fields present
					continue;
				}
				
				string stringId = fields[0];

				if (!languageItems.ContainsKey(stringId))
				{
					continue;
				}

				// Store localized strings for this string id
				LanguageItem languageItem = languageItems[stringId];
				for (int j = 3; j < fields.Length; ++j)
				{
					if (j >= columnNames.Length)
					{
						continue;
					}
					string languageCode = columnNames[j];
					string languageEntry = CSVSupport.Unescape(fields[j]);
					
					if (languageEntry.Length > 0)
					{
						languageItem.localizedStrings[languageCode] = languageEntry;
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

			CsvParser csvParser = new CsvParser();
			string[][] csvTable = csvParser.Parse(csvData);

			if (csvTable.Length <= 1)
			{
				// No data rows in file
				return;
			}

			// Parse header row
			string[] columnNames = csvTable[0];

			if (columnNames.Length < 4)
			{
				// No languages defined in CSV file
				return;
			}

			int languageIndex = -1;
			for (int i = 3; i < columnNames.Length; ++i)
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

			// Cache a lookup table of characters in the scene
			Dictionary<string, Character> characterDict = new Dictionary<string, Character>();
			foreach (Character character in GameObject.FindObjectsOfType<Character>())
			{
				characterDict[character.nameText] = character;
			}

			// Cache a lookup table of flowcharts in the scene
			Dictionary<string, Flowchart> flowchartDict = new Dictionary<string, Flowchart>();
			foreach (Flowchart flowChart in GameObject.FindObjectsOfType<Flowchart>())
			{
				flowchartDict[flowChart.localizationId] = flowChart;
			}

			for (int i = 1; i < csvTable.Length; ++i)
			{
				string[] fields = csvTable[i];

				if (fields.Length < languageIndex + 1)
				{
					continue;
				}
				
				string stringId = fields[0];
				string languageEntry = CSVSupport.Unescape(fields[languageIndex]);
					
				if (languageEntry.Length > 0)
				{
					localizedStrings[stringId] = languageEntry;
					PopulateGameString(stringId, languageEntry, flowchartDict, characterDict);
				}
			}
		}

		public virtual void PopulateGameString(string stringId, 
		                                       string localizedText, 
		                                       Dictionary<string, Flowchart> flowchartDict,
		                                       Dictionary<string, Character> characterDict)
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
				
				string flowchartId = idParts[1];
				if (!flowchartDict.ContainsKey(flowchartId))
				{
					return;
				}
				Flowchart flowchart = flowchartDict[flowchartId];
	
				int itemId = int.Parse(idParts[2]);
				
				if (flowchart != null)
				{
					foreach (Say say in flowchart.GetComponentsInChildren<Say>())
					{
						if (say.itemId == itemId)
						{
							say.storyText = localizedText;
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
				
				string flowchartId = idParts[1];
				if (!flowchartDict.ContainsKey(flowchartId))
				{
					return;
				}
				Flowchart flowchart = flowchartDict[flowchartId];

				int itemId = int.Parse(idParts[2]);
				
				if (flowchart != null)
				{
					foreach (Menu menu in flowchart.GetComponentsInChildren<Menu>())
					{
						if (menu.itemId == itemId)
						{
							menu.text = localizedText;
						}
					}
				}
			}
			else if (stringType == "CHARACTER")
			{
				if (idParts.Length != 2)
				{
					return;
				}
				
				string characterName = idParts[1];
				if (!characterDict.ContainsKey(characterName))
				{
					return;
				}

				Character character = characterDict[characterName];
				if (character != null)
				{
					character.nameText = localizedText;
				}
			}
		}
	}

}