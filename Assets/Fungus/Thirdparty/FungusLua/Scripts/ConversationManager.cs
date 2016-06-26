using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fungus
{
	public class ConversationManager
	{
		protected Stage stage;

		protected CharacterItem[] characters;

		protected struct CharacterItem
		{
			public string name;
			public string nameText;
			public Character character;

			public bool CharacterMatch(string matchString)
			{
				return name.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture)
					|| nameText.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture);
			}
		}

		protected bool exitSayWait;

		public ConversationManager () {
			stage = Stage.activeStages[0];
		
			Character[] characterObjects = GameObject.FindObjectsOfType<Fungus.Character>();
			characters = new CharacterItem[characterObjects.Length];

			for (int i = 0; i < characterObjects.Length; i++)
			{
				characters[i].name = characterObjects[i].name;
				characters[i].nameText = characterObjects[i].nameText;
				characters[i].character = characterObjects[i];
			}
		}

		/// <summary>
		/// Parse and execute a conversation string
		/// </summary>
		/// <param name="conv"></param>
		public IEnumerator Conversation(string conv)
		{
			if (string.IsNullOrEmpty(conv))
			{
				yield break;
			}
			
			SayDialog sayDialog = SayDialog.GetSayDialog();

			if (sayDialog == null)
			{
				yield break;
			}

			//find SimpleScript say strings with portrait options
			//You can test regex matches here: http://regexstorm.net/tester
			var sayRegex = new Regex(@"((?<character>\w*)( ?(?<portrait>\w*)( ?(?<position>\w*)))?:)?(?<text>.*\r*\n)");
			var sayMatches = sayRegex.Matches(conv);

			foreach (Match match in sayMatches)
			{
				string characterKey = match.Groups["character"].Value;
				string portrait = match.Groups["portrait"].Value;
				string position = match.Groups["position"].Value;
				string text = match.Groups["text"].Value;
				Character character = null;

				sayDialog.gameObject.SetActive(true);

				if (!string.IsNullOrEmpty(characterKey))
				{
					for (int i = 0; i < characters.Length; i++)
					{
						if( characters[i].CharacterMatch(characterKey)) {
							character = characters[i].character;
							break;
						}
					}
				}
				//We'll assume that if no character is specified, we'll leave it as the last one.
				//It's probably a continuation of the last character's dialog
				if (character != null)
				{
					sayDialog.SetCharacter(character);

					string currentPosition = null;
					if (character.state.position != null) currentPosition = character.state.position.name;
					if (stage != null) stage.Show(character, portrait, currentPosition ?? position, position);
				}

				exitSayWait = false;
				sayDialog.Say(text, true, true, true, false, null, () => {
					exitSayWait = true;
				});

				while (!exitSayWait)
				{
					yield return null;
				}
				exitSayWait = false;
			}
		}
	}
}
	