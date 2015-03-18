using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Fungus
{

	/**
	 * Import and export a Fungus story in the .fountain screenplay format.
	 * The exported file contains special tags in note blocks which map the
	 * story text to the corresponding commands.
	 */
	public class FountainExporter 
	{

		public static void ExportStrings(FungusScript fungusScript)
		{
			if (fungusScript == null)
			{
				return;
			}
			
			string path = EditorUtility.SaveFilePanel("Export strings", "",
			                                          fungusScript.name + ".txt", "");
			
			if(path.Length == 0) 
			{
				return;
			}
			
			// Write out character names
			
			string exportText = "Title: " + fungusScript.name + "\n";
			exportText += "Draft date: " + System.DateTime.Today.ToString("d") + "\n";
			exportText += "\n";
			
			// In every sequence, write out Say & Menu text in order
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();
			foreach (Sequence s in sequences)
			{
				// Check for any Say, Menu or Comment commands
				bool hasText = false;
				foreach (Command c in s.commandList)
				{
					System.Type t = c.GetType();
					if (t == typeof(Say) ||
					    t == typeof(Menu) ||
					    t == typeof(Comment))
					{
						hasText = true;
					}
				}
				if (!hasText)
				{
					continue;
				}
				
				exportText += "." + s.sequenceName.ToUpper() + "\n\n";
				
				foreach (Command c in s.commandList)
				{
					if (c.GetType() == typeof(Say))
					{
						string idText = "";
						Say say = c as Say;
						
						if (say.character == null)
						{
							exportText += "NO CHARACTER\n";
						}
						else
						{
							exportText += say.character.nameText.ToUpper() + "\n";
						}
						
						idText += "[[Say," + c.commandId + "]]\n";
						
						exportText += idText;
						
						// Fountain requires blank dialogue lines to contain 2 spaces or else
						// they will be interpreted as ACTION text.
						string trimmedText = say.storyText.Trim();
						string[] lines = trimmedText.Split(new [] { '\r', '\n' });
						foreach (string line in lines)
						{
							string trimmed = line.Trim();
							if (line.Length == 0)
							{
								exportText += "  \n";
							}
							else
							{
								exportText += trimmed + "\n";
							}
						}
						
						exportText += "\n";
					}
					else if (c.GetType() == typeof(Menu))
					{
						exportText += "MENU\n";
						
						string idText = "";
						Menu menu = c as Menu;
						idText += "[[Menu," + c.commandId + "]]\n";
						
						exportText += idText + menu.text.Trim() + "\n\n";
					}
					else if (c.GetType() == typeof(Comment))
					{
						string idText = "";
						Comment comment = c as Comment;
						idText += "[[Comment," + c.commandId + "]]\n";
						
						exportText += idText + comment.commentText.Trim() + "\n\n";
					}
				}
			}
			
			File.WriteAllText(path, exportText);
		}
		
		public static void ImportStrings(FungusScript fungusScript)
		{
			string path = EditorUtility.OpenFilePanel("Import strings", "", "");
			
			if(path.Length == 0) 
			{
				return;
			}
			
			string stringsFile = File.ReadAllText(path);
			
			StringsParser parser = new StringsParser();
			List<StringsParser.StringItem> items = parser.ProcessText(stringsFile);
			
			// Build dict of commands
			Dictionary<int, Command> commandDict = new Dictionary<int, Command>();
			foreach (Command c in fungusScript.gameObject.GetComponentsInChildren<Command>())
			{
				commandDict.Add (c.commandId, c);
			}
			
			foreach (StringsParser.StringItem item in items)
			{
				if (item.parameters.Length != 2)
				{
					continue;
				}
				
				string stringType = item.parameters[0];
				if (stringType == "Say")
				{
					int commandId = int.Parse(item.parameters[1]);					
					Say sayCommand = commandDict[commandId] as Say;
					if (sayCommand != null)
					{
						sayCommand.storyText = item.bodyText;
					}
				}
				else if (stringType == "Menu")
				{
					int commandId = int.Parse(item.parameters[1]);					
					Menu menuCommand = commandDict[commandId] as Menu;
					if (menuCommand != null)
					{
						menuCommand.text = item.bodyText;
					}
				}
				else if (stringType == "Comment")
				{
					int commandId = int.Parse(item.parameters[1]);					
					Comment commentCommand = commandDict[commandId] as Comment;
					if (commentCommand != null)
					{
						commentCommand.commentText = item.bodyText;
					}
				}
			}
		}
	}

}
