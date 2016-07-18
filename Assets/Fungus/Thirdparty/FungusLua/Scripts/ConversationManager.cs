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
		protected Character[] characters;

		protected Character currentCharacter;
		protected Sprite currentPortrait;
		protected RectTransform currentPosition;

		protected Character previousCharacter;
		protected Sprite previousPortrait;
		protected RectTransform previousPosition;

		protected bool exitSayWait;

		public ConversationManager ()
		{
			stage = Stage.activeStages[0];
			
			// cache characters for faster lookup
			characters = GameObject.FindObjectsOfType<Fungus.Character>();
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
			Regex sayRegex = new Regex(@"((?<sayParams>[\w ,]*):)?(?<text>.*\r*\n)");
			MatchCollection sayMatches = sayRegex.Matches(conv);

			for (int i = 0; i < sayMatches.Count; i++)
			{
				previousCharacter = currentCharacter;
				previousPortrait = currentPortrait;
				previousPosition = currentPosition;

				string sayParams = sayMatches[i].Groups["sayParams"].Value;
				if (!string.IsNullOrEmpty(sayParams))
				{
					string[] separateParams;
					if (sayParams.Contains(","))
					{
						separateParams = sayParams.Split(',');
						for (int j = 0; j < separateParams.Length; j++)
						{
							separateParams[j] = separateParams[j].Trim();
						}
					}
					else
					{
						separateParams = sayParams.Split(' ');
					}

					SetParams(separateParams);
				}
				else
				{
					//no params! Use previous settings?
				}

				string text = sayMatches[i].Groups["text"].Value;

				sayDialog.gameObject.SetActive(true);

				if (currentCharacter != null && currentCharacter != previousCharacter)
				{
					sayDialog.SetCharacter(currentCharacter);
				}

				if (stage != null && currentCharacter != null && (currentPortrait != previousPortrait || currentPosition != previousPosition))
				{
					PortraitOptions portraitOptions = new PortraitOptions(true);
					portraitOptions.character = currentCharacter;
					portraitOptions.fromPosition = currentCharacter.state.position ?? previousPosition;
					portraitOptions.toPosition = currentPosition;
					portraitOptions.portrait = currentPortrait;

					stage.Show(portraitOptions);
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
		
		/// <summary>
		/// Using the string of say parameters before the ':',
		/// set the current character, position and portrait if provided.
		/// </summary>
		/// <param name="sayParams">The list of say parameters</param>
		private void SetParams(string[] sayParams)
		{
			Sprite portrait = null;
			RectTransform position = null;

			//find the character first, since we need to get its portrait
			for (int i = 0; i < sayParams.Length; i++)
			{
				for (int j = 0; j < characters.Length; j++)
				{
					if (characters[j].NameStartsWith(sayParams[i]))
					{
						currentCharacter = characters[j];
						break;
					}
				}
			}

			for (int i = 0; i < sayParams.Length; i++)
			{
				if (portrait == null) portrait = currentCharacter.GetPortrait(sayParams[i]);
				if (position == null) position = stage.GetPosition(sayParams[i]);
			}
			currentPosition = position;
			currentPortrait = portrait;
		}
	}
}