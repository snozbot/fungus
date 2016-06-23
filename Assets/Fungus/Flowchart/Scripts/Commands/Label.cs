/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Label", 
	             "Marks a position in the command list for execution to jump to.")]
	[AddComponentMenu("")]
	public class Label : Command
	{
		[Tooltip("Display name for the label")]
		public string key = "";

		public override void OnEnter()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return key;
		}

		public override Color GetButtonColor()
		{
			return new Color32(200, 200, 253, 255);
		}
	}

}