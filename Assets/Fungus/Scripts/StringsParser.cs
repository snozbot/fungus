using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	/**
	 * Parses a text file using a simple format and adds string values to the global string table.
	 * The format is:
	 * $FirstString
	 * The first string text goes here
	 * $SecondString
	 * The second string text goes here
	 * # This is a comment line and will be ignored by the parser
	 */
	public class StringsParser : MonoBehaviour
	{
		public TextAsset stringsFile;
		
		private enum ParseMode
		{
			Idle,
			Text,
		};

		void Start()
		{
			ProcessText(stringsFile.text);
		}
		
		void ProcessText(string text) 
		{
			StringTable stringTable = Game.GetInstance().stringTable; 

			// Split text into lines. Add a newline at end to ensure last command is always parsed
			string[] lines = Regex.Split(text + "\n", "(?<=\n)");
			
			string blockBuffer = "";

			ParseMode mode = ParseMode.Idle;

			string blockTag = "";
			for (int i = 0; i < lines.Length; ++i)
			{
				string line = lines[i];

				bool newBlock = line.StartsWith("$");

				if (mode == ParseMode.Idle && !newBlock)
				{
					// Ignore any text not preceded by a label tag
					continue;
				}

				string newBlockTag = "";
				if (newBlock)
				{
					newBlockTag = line.Replace ("\n", "");
				}

				bool endOfFile = (i == lines.Length-1);

				bool storeBlock = false;

				if (newBlock)
				{
					storeBlock = true;
				}
				else if (mode == ParseMode.Text && endOfFile)
				{
					storeBlock = true;
					if (!line.StartsWith("#"))
					{
						blockBuffer += line;
					}
				}
				else
				{
					if (!line.StartsWith("#"))
					{
						blockBuffer += line;
					}
				}

				if (storeBlock)
				{
					if (blockTag.Length > 0 && blockBuffer.Length > 0)
					{
						// Trim off last newline
						blockBuffer = blockBuffer.TrimEnd( '\r', '\n', ' ', '\t');

						stringTable.SetString(blockTag, blockBuffer);
					}

					// Prepare to parse next block
					mode = ParseMode.Idle;
					if (newBlock)
					{
						mode = ParseMode.Text;
						blockTag = newBlockTag;
					}

					blockBuffer = "";
				}
			}
		}
	}
}
