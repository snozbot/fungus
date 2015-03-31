using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Fungus;

namespace Fungus
{
	/**
	 * Parses an exported strings file using the Fountain file format for screenplays
	 * See http://fountain.io for details.
	 * We only support a small subset of Fountain markup, and use note tags to embed meta data to
	 * bind dialogue text to the corresponding Say / Menu commands.
	 */
	public class StringsParser
	{
		public class StringItem
		{
			public string[] parameters;
			public string bodyText;
		}

		public virtual List<StringItem> ProcessText(string text) 
		{
			List<StringItem> items = new List<StringItem>();

			// Split text into lines. Add a newline at end to ensure last command is always parsed
			string[] lines = Regex.Split(text + "\n", "(?<=\n)");

			int i = 0;
			while (i < lines.Length)
			{
				string line = lines[i].Trim();

				if (!(line.StartsWith("[[") && line.EndsWith("]]")))
				{
					i++;
					continue;
				}

				string blockTag = line.Substring(2, line.Length - 4);

				// Find next empty line, #, [[ or eof
				int start = i + 1;
				int end = lines.Length - 1;
				for (int j = start; j <= end; ++j)
				{
					string line2 = lines[j].Trim();

					if (line2.Length == 0 ||
					    line2.StartsWith("#") ||
					    line2.StartsWith("[["))
					{
						end = j;
						break;
					}
				}

				if (end > start)
				{
					string blockBuffer = "";
					for (int j = start; j <= end; ++j)
					{
						blockBuffer += lines[j].Trim() + "\n";
					}

					blockBuffer = blockBuffer.Trim();

					StringItem item = CreateItem(blockTag, blockBuffer);
					if (item != null)
					{
						items.Add(item);
					}					
				}

				i = end + 1;
			}

			return items;
		}

		protected StringItem CreateItem(string commandInfo, string bodyText)
		{
			string[] parameters = commandInfo.Split(new char[] { ',' });
			if (parameters.Length > 0)
			{
				StringItem item = new StringItem();
				item.parameters = parameters;
				item.bodyText = bodyText;
				return item;
			}

			return null;
		}
	}
}
