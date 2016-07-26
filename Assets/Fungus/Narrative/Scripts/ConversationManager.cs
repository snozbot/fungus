using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fungus
{
	public class ConversationManager
	{
        protected struct ConversationItem
        {
            public string Text { get; set; }
            public Character Character { get; set; }
            public Sprite Portrait { get; set; }
            public RectTransform Position { get; set; }
        }

		protected Character[] characters;

		protected bool exitSayWait;

		public void PopulateCharacterCache()
		{
			// cache characters for faster lookup
			characters = UnityEngine.Object.FindObjectsOfType<Character>();
		}

        protected SayDialog GetSayDialog(Character character)
        {
            SayDialog sayDialog = null;
            if (character != null)
            {
                if (character.setSayDialog != null)
                {
                    sayDialog = character.setSayDialog;
                }
            }

            if (sayDialog == null)
            {
                sayDialog = SayDialog.GetSayDialog();
            }

            return sayDialog;
        }

		/// <summary>
		/// Parse and execute a conversation string
		/// </summary>
		/// <param name="conv"></param>
		public IEnumerator DoConversation(string conv)
		{
			if (string.IsNullOrEmpty(conv))
			{
				yield break;
			}
			
            var conversationItems = Parse(conv);

            // Track the current and previous parameter values
            Character currentCharacter = null;
            Sprite currentPortrait = null;
            RectTransform currentPosition = null;
            Character previousCharacter = null;
            Sprite previousPortrait = null;
            RectTransform previousPosition = null;

            // Play the conversation
            for (int i = 0; i < conversationItems.Count; ++i)
            {
                ConversationItem item = conversationItems[i];

                if (item.Character != null)
                {
                    currentCharacter = item.Character;
                }

                currentPortrait = item.Portrait;
                currentPosition = item.Position;

                SayDialog sayDialog = GetSayDialog(currentCharacter);

                if (sayDialog == null)
                {
                    // Should never happen
                    yield break;
                }

                sayDialog.gameObject.SetActive(true);

                if (currentCharacter != null && 
                    currentCharacter != previousCharacter)
                {
                    sayDialog.SetCharacter(currentCharacter);
                }

                Stage stage = Stage.GetActiveStage();

                if (stage != null && 
                    currentCharacter != null && 
                    (currentPortrait != previousPortrait || currentPosition != previousPosition))
                {
                    PortraitOptions portraitOptions = new PortraitOptions(true);
                    portraitOptions.character = currentCharacter;
                    portraitOptions.fromPosition = currentCharacter.state.position;
                    portraitOptions.toPosition = currentPosition;
                    portraitOptions.portrait = currentPortrait;

                    // Do a move tween if the same character as before is moving to a new position
                    // In all other cases snap to the new position.
                    if (previousCharacter == currentCharacter &&
                        previousPosition != currentPosition)
                    {
                        portraitOptions.move = true;
                    }

                    stage.Show(portraitOptions);
                }
                    
                previousCharacter = currentCharacter;
                previousPortrait = currentPortrait;
                previousPosition = currentPosition;

                // Ignore Lua style comments and blank lines
                if (item.Text.StartsWith("--") || item.Text.Trim() == "")
                {
                    continue;
                }

                exitSayWait = false;
                sayDialog.Say(item.Text, true, true, true, false, null, () => {
                    exitSayWait = true;
                });

                while (!exitSayWait)
                {
                    yield return null;
                }
                exitSayWait = false;
            }
		}

        protected virtual List<ConversationItem> Parse(string conv)
        {
            //find SimpleScript say strings with portrait options
            //You can test regex matches here: http://regexstorm.net/tester
            Regex sayRegex = new Regex(@"((?<sayParams>[\w ,]*):)?(?<text>.*\r*\n)");
            MatchCollection sayMatches = sayRegex.Matches(conv);

            var items = new List<ConversationItem>(sayMatches.Count);

            Character currentCharacter = null;
            for (int i = 0; i < sayMatches.Count; i++)
            {
                string text = sayMatches[i].Groups["text"].Value;
                string sayParams = sayMatches[i].Groups["sayParams"].Value;
                string[] separateParams = null;

                if (!string.IsNullOrEmpty(sayParams))
                {
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
                }

                var item = CreateConversationItem(separateParams, text, currentCharacter);

                // Previous speaking character is the default for next conversation item
                currentCharacter = item.Character;

                items.Add(item);
            }

            return items;
        }
                        		
		/// <summary>
		/// Using the string of say parameters before the ':',
		/// set the current character, position and portrait if provided.
		/// </summary>
		/// <param name="sayParams">The list of say parameters</param>
        protected virtual ConversationItem CreateConversationItem(string[] sayParams, string text, Character currentCharacter)
		{
            var item = new ConversationItem();

            // Populate the story text to be written
            item.Text = text;

            if (sayParams == null || sayParams.Length == 0)
            {
                // Text only, no params - early out.
                return item;
            }

			// try to find the character param first, since we need to get its portrait
            int characterIndex = -1;
            for (int i = 0; item.Character == null && i < sayParams.Length; i++)
			{
				for (int j = 0; j < characters.Length; j++)
				{
					if (characters[j].NameStartsWith(sayParams[i]))
					{
                        characterIndex = i;
                        item.Character = characters[j];
						break;
					}
				}
			}

            // Assume last used character if none is specified now
            if (item.Character == null)
            {
                item.Character = currentCharacter;
            }

            // Next see if we can find a portrait for this character
            int portraitIndex = -1;
            if (item.Character != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (item.Portrait == null && 
                        item.Character != null &&
                        i != characterIndex) 
                    {
                        Sprite s = item.Character.GetPortrait(sayParams[i]);
                        if (s != null)
                        {
                            portraitIndex = i;
                            item.Portrait = s;
                            break;
                        }
                    }
                }
            }

            // Next check if there's a position parameter
            Stage stage = Stage.GetActiveStage();
            if (stage != null)
            {
                for (int i = 0; i < sayParams.Length; i++)
                {
                    if (i != characterIndex &&
                        i != portraitIndex)
                    {
                        item.Position = stage.GetPosition(sayParams[i]);
                        break;
                    }
                }
            }

            return item;
		}
	}
}