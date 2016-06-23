/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Variable", 
	             "Set Save Profile", 
	             "Sets the active profile that the Save Variable and Load Variable commands will use. This is useful to crete multiple player save games. Once set, the profile applies across all Flowcharts and will also persist across scene loads.")]
	[AddComponentMenu("")]
	public class SetSaveProfile : Command
	{
		/**
		 * Shared save profile name used by SaveVariable and LoadVariable.
		 */
		public static string saveProfile = "";

		[Tooltip("Name of save profile to make active.")]
		public string saveProfileName = "";

		public override void OnEnter()
		{
			saveProfile = saveProfileName;

			Continue();
		}
		
		public override string GetSummary()
		{
			return saveProfileName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}
	
}