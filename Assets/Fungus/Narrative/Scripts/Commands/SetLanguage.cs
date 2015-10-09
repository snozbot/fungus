using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Narrative", 
	             "Set Language", 
	             "Set the active language for the scene. A Localization object with a localization file must be present in the scene.")]
	[AddComponentMenu("")]
	public class SetLanguage : Command 
	{
		[Tooltip("Code of the language to set. e.g. ES, DE, JA")]
		public string languageCode; 

		public static string mostRecentLanguage = "";

		public override void OnEnter()
		{
			Localization localization = GameObject.FindObjectOfType<Localization>();
			if (localization != null)
			{
				localization.SetActiveLanguage(languageCode, true);

				// Cache the most recently set language code so we can continue to 
				// use the same language in subsequent scenes.
				mostRecentLanguage = languageCode;
			}

			Continue();
		}

		public override string GetSummary()
		{
			return languageCode;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}
}