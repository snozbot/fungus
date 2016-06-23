/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;
using Fungus;

namespace Fungus
{

	[CommandInfo("Variable",
	             "Read Text File",
	             "Reads in a text file and stores the contents in a string variable")]
	public class ReadTextFile : Command
	{
		[Tooltip("Text file to read into the string variable")]
		public TextAsset textFile;

		[Tooltip("String variable to store the tex file contents in")]
		[VariableProperty(typeof(StringVariable))]
		public StringVariable stringVariable;

		public override void OnEnter() 
		{
			if (textFile == null || 
			    stringVariable == null)	
			{
				Continue();
				return;
			}

			stringVariable.value = textFile.text;

			Continue();
		}

		public override string GetSummary()
		{
			if (stringVariable == null)
			{
				return "Error: Variable not selected";
			}

			if (textFile == null)
			{
				return "Error: Text file not selected";
			}

			return stringVariable.key;
		}
		
		public override bool HasReference(Variable variable)
		{
			return (variable == stringVariable);
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}