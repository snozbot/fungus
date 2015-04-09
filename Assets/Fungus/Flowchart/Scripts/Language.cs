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
		 * Convert all language items and localized strings to an easy to edit CSV format.
		 */
		public virtual string GetCSVData()
		{
			// Collect all the language items present in the scene
			Dictionary<string, LanguageItem> languageItems = FindLanguageItems();

			// Update language items with localization data from CSV file
			if (localizationFile != null &&
			    localizationFile.text.Length > 0)
			{
				AddLocalizedStrings(languageItems, localizationFile.text);
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

		/**
		 * Buidls a dictionary of localizable objects in the scene.
		 */
		protected Dictionary<string, LanguageItem> FindLanguageItems()
		{
			Dictionary<string, LanguageItem> languageItems = new Dictionary<string, LanguageItem>();

			// Export all character names
			foreach (Character character in GameObject.FindObjectsOfType<Character>())
			{
				// String id for character names is CHARACTER.<Character Name>
				LanguageItem languageItem = new LanguageItem();
				languageItem.standardText = character.nameText;
				languageItem.description = character.description;
				string stringId = "CHARACTER." + character.nameText;
				languageItems[stringId] = languageItem;
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
						string stringId = "";
						string standardText = "";
						string description = "";

						System.Type type = command.GetType();
						if (type == typeof(Say))
						{
							// String id for Say commands is SAY.<Flowchart id>.<Command id>.<Character Name>
							Say sayCommand = command as Say;
							standardText = sayCommand.storyText;
							description = sayCommand.description;
							stringId = "SAY." + flowchart.localizationId + "." + sayCommand.itemId + ".";
							if (sayCommand.character != null)
							{
								stringId += sayCommand.character.nameText;
							}
						}
						else if (type == typeof(Menu))
						{							
							// String id for Menu commands is MENU.<Flowchart id>.<Command id>
							Menu menuCommand = command as Menu;
							standardText = menuCommand.text;
							description = menuCommand.description;
							stringId = "MENU." + flowchart.localizationId + "." + menuCommand.itemId;
						}
						else
						{
							continue;
						}
						
						LanguageItem languageItem = null;
						if (languageItems.ContainsKey(stringId))
						{
							languageItem = languageItems[stringId];
						}
						else
						{
							languageItem = new LanguageItem();
							languageItems[stringId] = languageItem;
						}
						
						// Update basic properties,leaving localised strings intact
						languageItem.standardText = standardText;
						languageItem.description = description;
					}
				}
			}
			
			return languageItems;
		}

		/**
		 * Adds localized strings from CSV file data to a dictionary of language items in the scene.
		 */
		protected virtual void AddLocalizedStrings(Dictionary<string, LanguageItem> languageItems, string csvData)
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

		/**
		 * Scan a localization CSV file and copies the strings for the specified language code
		 * into the text properties of the appropriate scene objects.
		 */
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
					PopulateTextProperty(stringId, languageEntry, flowchartDict, characterDict);
				}
			}
		}

		/**
		 * Populates the text property of a single scene object with localized text.
		 */
		public virtual void PopulateTextProperty(string stringId, 
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
				if (idParts.Length != 4)
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

		/**
		 * Returns all standard text for SAY & MENU commands in the scene using an
		 * easy to edit custom text format.
		 */
		public virtual string GetStandardText()
		{
			// Collect all the language items present in the scene
			Dictionary<string, LanguageItem> languageItems = FindLanguageItems();

			string textData = "";
			foreach (string stringId in languageItems.Keys)
			{
				if (!stringId.StartsWith("SAY.") && !(stringId.StartsWith("MENU.")))
				{
					continue;
				}

				LanguageItem languageItem = languageItems[stringId];

				textData += "#" + stringId + "\n";
				textData += languageItem.standardText.Trim() + "\n\n";
			}

			return textData;
		}

		/**
		 * Sets standard text on scene objects by parsing a text data file.
		 */
		public virtual void SetStandardText(string textData)
		{
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

			string[] lines = textData.Split('\n');

			string stringId = "";
			string buffer = "";
			foreach (string line in lines)
			{
				// Check for string id line	
				if (line.StartsWith("#"))
				{
					if (stringId.Length > 0)
					{
						// Write buffered text to the appropriate text property
						PopulateTextProperty(stringId, buffer.Trim(), flowchartDict, characterDict);
					}

					// Set the string id for the follow text lines
					stringId = line.Substring(1, line.Length - 1);
					buffer = "";
				}
				else
				{
					buffer += line;
				}
			}

			// Handle last buffered entry
			if (stringId.Length > 0)
			{
				PopulateTextProperty(stringId, buffer.Trim(), flowchartDict, characterDict);
			}
		}
	}

}