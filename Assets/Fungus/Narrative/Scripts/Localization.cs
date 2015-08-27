using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ideafixxxer.CsvParser;

namespace Fungus
{

	public interface ILocalizable
	{
		string GetStandardText();
		void SetStandardText(string standardText);
		string GetDescription();
		string GetStringId();
	}

	/**
	 * Multi-language localization support.
	 */
	public class Localization : MonoBehaviour
	{
		/**
		 * Language to use at startup, usually defined by a two letter language code (e.g DE = German)
		 */
		[Tooltip("Language to use at startup, usually defined by a two letter language code (e.g DE = German)")]
		public string activeLanguage = "";

		protected static Dictionary<string, string> localizedStrings = new Dictionary<string, string>();

		protected Dictionary<string, ILocalizable> localizeableObjects = new Dictionary<string, ILocalizable>();

		/**
		 * Temp storage for a single item of standard text and its localizations
		 */
		protected class TextItem
		{
			public string description = "";
			public string standardText = "";
			public Dictionary<string, string> localizedStrings = new Dictionary<string, string>();
		}

		/**
		 * CSV file containing localization data which can be easily edited in a spreadsheet tool.
		 */
		 [Tooltip("CSV file containing localization data which can be easily edited in a spreadsheet tool")]
		 public TextAsset localizationFile;

		/**
		 * Stores any notification message from export / import methods.
		 */
		[NonSerialized]
		public string notificationText = "";

		public virtual void Start()
		{
			CacheLocalizeableObjects();

			if (localizationFile != null &&
			    localizationFile.text.Length > 0)
			{
				SetActiveLanguage(activeLanguage);
			}
		}

		// Build a cache of all the localizeable objects in the scene
		protected virtual void CacheLocalizeableObjects()
		{
			Component[] components = GameObject.FindObjectsOfType<Component>();
			foreach (Component component in components)
			{
				ILocalizable localizable = component as ILocalizable;
				if (localizable != null)
				{
					localizeableObjects[localizable.GetStringId()] = localizable;
				}
			}
		}

		/**
		 * Looks up the specified string in the localized strings table.
		 * For this to work, a localization file and active language must have been set previously.
		 * Return null if the string is not found.
		 */
		public static string GetLocalizedString(string stringId)
		{
			if (localizedStrings == null)
			{
				return null;
			}

			if (localizedStrings.ContainsKey(stringId))
			{
				return localizedStrings[stringId];
			}

			return null;
		}

		/**
		 * Convert all text items and localized strings to an easy to edit CSV format.
		 */
		public virtual string GetCSVData()
		{
			// Collect all the text items present in the scene
			Dictionary<string, TextItem> textItems = FindTextItems();

			// Update text items with localization data from CSV file
			if (localizationFile != null &&
			    localizationFile.text.Length > 0)
			{
				AddCSVDataItems(textItems, localizationFile.text);
			}

			// Build CSV header row and a list of the language codes currently in use
			string csvHeader = "Key,Description,Standard";
			List<string> languageCodes = new List<string>();
			foreach (TextItem textItem in textItems.Values)
			{
				foreach (string languageCode in textItem.localizedStrings.Keys)
				{
					if (!languageCodes.Contains(languageCode))
					{
						languageCodes.Add(languageCode);
						csvHeader += "," + languageCode;
					}
				}
			}

			// Build the CSV file using collected text items
			int rowCount = 0;
			string csvData = csvHeader + "\n";
			foreach (string stringId in textItems.Keys)
			{
				TextItem textItem = textItems[stringId];

				string row = CSVSupport.Escape(stringId);
				row += "," + CSVSupport.Escape(textItem.description);
				row += "," + CSVSupport.Escape(textItem.standardText);

				foreach (string languageCode in languageCodes)
				{
					if (textItem.localizedStrings.ContainsKey(languageCode))
					{
						row += "," + CSVSupport.Escape(textItem.localizedStrings[languageCode]);
					}
					else
					{
						row += ","; // Empty field
					}
				}

				csvData += row + "\n";
				rowCount++;
			}

			notificationText = "Exported " + rowCount + " localization text items.";

			return csvData;
		}

		/**
		 * Builds a dictionary of localizable text items in the scene.
		 */
		protected Dictionary<string, TextItem> FindTextItems()
		{
			Dictionary<string, TextItem> textItems = new Dictionary<string, TextItem>();

			// Add localizable commands in same order as command list to make it
			// easier to localise / edit standard text.
			Flowchart[] flowcharts = GameObject.FindObjectsOfType<Flowchart>();
			foreach (Flowchart flowchart in flowcharts)
			{
				Block[] blocks = flowchart.GetComponentsInChildren<Block>();
				foreach (Block block in blocks)
				{
					foreach (Command command in block.commandList)
					{
						ILocalizable localizable = command as ILocalizable;
						if (localizable != null)
						{
							TextItem textItem = new TextItem();
							textItem.standardText = localizable.GetStandardText();
							textItem.description = localizable.GetDescription();
							textItems[localizable.GetStringId()] = textItem;
						}
					}
				}
			}

			// Add everything else that's localizable
			Component[] components = GameObject.FindObjectsOfType<Component>();
			foreach (Component component in components)
			{
				ILocalizable localizable = component as ILocalizable;
				if (localizable != null)
				{
					string stringId = localizable.GetStringId();
					if (textItems.ContainsKey(stringId))
					{
						// Already added
						continue;
					}

					TextItem textItem = new TextItem();
					textItem.standardText = localizable.GetStandardText();
					textItem.description = localizable.GetDescription();
					textItems[stringId] = textItem;
				}
			}

			return textItems;
		}

		/**
		 * Adds localized strings from CSV file data to a dictionary of text items in the scene.
		 */
		protected virtual void AddCSVDataItems(Dictionary<string, TextItem> textItems, string csvData)
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
				if (fields.Length < 3)
				{
					// No standard text or localized string fields present
					continue;
				}
				
				string stringId = fields[0];

				if (!textItems.ContainsKey(stringId))
				{
					if (stringId.StartsWith("CHARACTER.") || 
					    stringId.StartsWith("SAY.") || 
					    stringId.StartsWith("MENU.") ||
						stringId.StartsWith("WRITE.") ||
					    stringId.StartsWith("SETTEXT."))
					{
						// If it's a 'built-in' type this probably means that item has been deleted from its flowchart,
						// so there's no need to add a text item for it.
						continue;
					}

					// Key not found. Assume it's a custom string that we want to retain, so add a text item for it.
					TextItem newTextItem = new TextItem();
					newTextItem.description = CSVSupport.Unescape(fields[1]);
					newTextItem.standardText = CSVSupport.Unescape(fields[2]);
					textItems[stringId] = newTextItem;
				}

				TextItem textItem = textItems[stringId];

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
						textItem.localizedStrings[languageCode] = languageEntry;
					}
				}
			}
		}

		/**
		 * Scan a localization CSV file and copies the strings for the specified language code
		 * into the text properties of the appropriate scene objects.
		 */
		public virtual void SetActiveLanguage(string languageCode, bool forceUpdateSceneText = false)
		{
			if (!Application.isPlaying)
			{
				// This function should only ever be called when the game is playing (not in editor).
				return;
			}

			if (localizationFile == null)
			{
				// No localization file set
				return;
			}

			localizedStrings.Clear();

			CsvParser csvParser = new CsvParser();
			string[][] csvTable = csvParser.Parse(localizationFile.text);

			if (csvTable.Length <= 1)
			{
				// No data rows in file
				return;
			}

			// Parse header row
			string[] columnNames = csvTable[0];

			if (columnNames.Length < 3)
			{
				// No languages defined in CSV file
				return;
			}

			// First assume standard text column and then look for a matching language column
			int languageIndex = 2;
			for (int i = 3; i < columnNames.Length; ++i)
			{
				if (columnNames[i] == languageCode)
				{
					languageIndex = i;
					break;
				}
			}

			if (languageIndex == 2)
			{
				// Using standard text column
				// Add all strings to the localized strings dict, but don't replace standard text in the scene.
				// This allows string substitution to work for both standard and localized text strings.
				for (int i = 1; i < csvTable.Length; ++i)
				{
					string[] fields = csvTable[i];
					if (fields.Length < 3)
					{
						continue;
					}

					localizedStrings[fields[0]] = fields[languageIndex];
				}

				// Early out unless we've been told to force the scene text to update.
				// This happens when the Set Language command is used to reset back to the standard language.
				if (!forceUpdateSceneText)
				{
					return;
				}
			}

			// Using a localized language text column
			// 1. Add all localized text to the localized strings dict
			// 2. Update all scene text properties with localized versions
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
					PopulateTextProperty(stringId, languageEntry);
				}
			}
		}

		/**
		 * Populates the text property of a single scene object with a new text value.
		 */
		public virtual bool PopulateTextProperty(string stringId, string newText)
		{
			// Ensure that all localizeable objects have been cached
			if (localizeableObjects.Count == 0)
			{
				CacheLocalizeableObjects();
			}

			ILocalizable localizable = null;
			localizeableObjects.TryGetValue(stringId, out localizable);
			if (localizable != null)
			{
				localizable.SetStandardText(newText);
				return true;
			}

			return false;
		}

		/**
		 * Returns all standard text for localizeable text in the scene using an
		 * easy to edit custom text format.
		 */
		public virtual string GetStandardText()
		{
			// Collect all the text items present in the scene
			Dictionary<string, TextItem> textItems = FindTextItems();

			string textData = "";
			int rowCount = 0;
			foreach (string stringId in textItems.Keys)
			{
				TextItem languageItem = textItems[stringId];

				textData += "#" + stringId + "\n";
				textData += languageItem.standardText.Trim() + "\n\n";
				rowCount++;
			}

			notificationText = "Exported " + rowCount + " standard text items.";
			
			return textData;
		}

		/**
		 * Sets standard text on scene objects by parsing a text data file.
		 */
		public virtual void SetStandardText(string textData)
		{
			string[] lines = textData.Split('\n');

			int updatedCount = 0;

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
						if (PopulateTextProperty(stringId, buffer.Trim()))
						{
							updatedCount++;
						}
					}

					// Set the string id for the follow text lines
					stringId = line.Substring(1, line.Length - 1);
					buffer = "";
				}
				else
				{
					buffer += line + "\n";
				}
			}

			// Handle last buffered entry
			if (stringId.Length > 0)
			{
				if (PopulateTextProperty(stringId, buffer.Trim()))
				{
					updatedCount++;
				}
			}

			notificationText = "Updated " + updatedCount + " standard text items.";
		}
	}

}