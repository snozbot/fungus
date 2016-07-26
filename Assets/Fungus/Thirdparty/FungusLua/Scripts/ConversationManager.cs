using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fungus
{
	public class ConversationManager
	{
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
			// cache characters for faster lookup
			characters = GameObject.FindObjectsOfType<Fungus.Character>();
		}

        protected Stage GetActiveStage()
        {
            if (Stage.activeStages == null ||
                Stage.activeStages.Count == 0)
            {
                return null;
            }

            return Stage.activeStages[0];
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

                Stage stage = GetActiveStage();

				if (stage != null && currentCharacter != null && (currentPortrait != previousPortrait || currentPosition != previousPosition))
				{
					PortraitOptions portraitOptions = new PortraitOptions(true);
					portraitOptions.character = currentCharacter;
					portraitOptions.fromPosition = currentCharacter.state.position ?? previousPosition;
					portraitOptions.toPosition = currentPosition;
					portraitOptions.portrait = currentPortrait;

					stage.Show(portraitOptions);
				}
				
                // Ignore Lua style comments and blank lines
                if (text.StartsWith("--") || text.Trim() == "")
                {
                    continue;
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
            Character character = null;
			Sprite portrait = null;
			RectTransform position = null;

			// try to find the character first, since we need to get its portrait
            int characterIndex = -1;
			for (int i = 0; character == null && i < sayParams.Length; i++)
			{
				for (int j = 0; j < characters.Length; j++)
				{
					if (characters[j].NameStartsWith(sayParams[i]))
					{
                        characterIndex = i;
                        character = characters[j];
						break;
					}
				}
			}

            // Assume last used character if none is specified now
            if (character == null)
            {
                character = currentCharacter;
            }

            // Next see if we can find a portrait for this character
            int portraitIndex = -1;
            if (character != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (portrait == null && 
                        character != null &&
                        i != characterIndex) 
                    {
                        Sprite s = character.GetPortrait(sayParams[i]);
                        if (s != null)
                        {
                            portraitIndex = i;
                            portrait = s;
                            break;
                        }
                    }
                }
            }

            // Next check if there's a position parameter
            Stage stage = GetActiveStage();
            if (stage != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (i != characterIndex &&
                        i != portraitIndex)
                    {
                        position = stage.GetPosition(sayParams[i]);
                        break;
                    }
                }
            }

            if (character != null)
            {
                currentCharacter = character;
            }
                
            if (portrait != null)
            {
			    currentPortrait = portrait;
            }

            if (position != null)
            {
                currentPosition = position;
            }
		}
	}
}