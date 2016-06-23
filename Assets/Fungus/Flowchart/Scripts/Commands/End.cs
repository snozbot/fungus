/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "End", 
	             "Marks the end of a conditional block.")]
	[AddComponentMenu("")]
	public class End : Command
	{
		[NonSerialized]
		public bool loop = false;

		public override void OnEnter()
		{
			if (loop)
			{
				for (int i = commandIndex - 1; i >= 0; --i)
				{
					System.Type commandType = parentBlock.commandList[i].GetType();
					if (commandType == typeof(While))
					{
						Continue(i);
						return;
					}
				}
			}

			Continue();
		}

		public override bool CloseBlock()
		{
			return true;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}