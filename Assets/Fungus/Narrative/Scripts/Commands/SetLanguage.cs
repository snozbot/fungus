using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Narrative", 
	             "Set Language", 
	             "Set the active language for the scene. A Localization object with a localization file must be present in the scene.")]
	[AddComponentMenu("")]
	public class SetLanguage : Command, ISerializationCallbackReceiver
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("languageCode")] public string languageCodeOLD;
		#endregion

		[Tooltip("Code of the language to set. e.g. ES, DE, JA")]
		public StringData _languageCode = new StringData(); 

		public static string mostRecentLanguage = "";

		public override void OnEnter()
		{
			Localization localization = GameObject.FindObjectOfType<Localization>();
			if (localization != null)
			{
				localization.SetActiveLanguage(_languageCode.Value, true);

				// Cache the most recently set language code so we can continue to 
				// use the same language in subsequent scenes.
				mostRecentLanguage = _languageCode.Value;
			}

			Continue();
		}

		public override string GetSummary()
		{
			return _languageCode.Value;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (languageCodeOLD != default(string))
			{
				_languageCode.Value = languageCodeOLD;
				languageCodeOLD = default(string);
			}
		}
	}
}