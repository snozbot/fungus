using UnityEngine;
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