using UnityEngine;
using System.Collections;

namespace Fungus
{

	[CommandInfo("Tests",
	             "Fail",
	             "Fails the current integration test")]
	public class FailTest : Command
	{
		public string failMessage;

		public override void OnEnter()
		{
			IntegrationTest.Fail(failMessage);

			Continue();
		}
	}

}