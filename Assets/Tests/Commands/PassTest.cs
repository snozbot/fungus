/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{

	[CommandInfo("Tests",
	             "Pass",
	             "Passes the current integration test")]
	public class PassTest : Command
	{
		public override void OnEnter()
		{
			IntegrationTest.Pass();

			Continue();
		}
	}

}