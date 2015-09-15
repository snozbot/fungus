using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fungus
{

	/**
	 * Implement this interface to be notified about Writer events
	 */
	public interface IWriterListener
	{
		// Called when a user input event (e.g. a click) has been handled by the Writer
		void OnInput();

		// Called when the Writer starts writing new text
		// An optional audioClip sound effect can be supplied (e.g. for voiceover)
		void OnStart(AudioClip audioClip);

		// Called when the Writer has paused writing text (e.g. on a {wi} tag)
		void OnPause();

		// Called when the Writer has resumed writing text
		void OnResume();

		// Called when the Writer has finshed writing text
		void OnEnd();

		// Called every time the Writer writes a new character glyph
		void OnGlyph();
	}
	
	public class Writer : MonoBehaviour, IDialogInputListener
	{
		[Tooltip("Gameobject containing a Text, Inout Field or Text Mesh object to write to")]
		public GameObject targetTextObject;

		[Tooltip("Writing characters per second")]
		public float writingSpeed = 60;

		[Tooltip("Pause duration for punctuation characters")]
		public float punctuationPause = 0.25f;

		[Tooltip("Color of text that has not been revealed yet")]
		public Color hiddenTextColor = new Color(1,1,1,0);

		[Tooltip("Write one word at a time rather one character at a time")]
		public bool writeWholeWords = false;

		[Tooltip("Force the target text object to use Rich Text mode so text color and alpha appears correctly")]
		public bool forceRichText = true;

		/**
		 * This property is true when the writer is waiting for user input to continue
		 */
		[System.NonSerialized]
		public bool isWaitingForInput;

		/** 
		 * This property is true when the writer is writing text or waiting (i.e. still processing tokens)
		 */
		[System.NonSerialized]
		public bool isWriting;

		protected float currentWritingSpeed;
		protected float currentPunctuationPause;
		protected Text textUI;
		protected InputField inputField;
		protected TextMesh textMesh;
		protected bool boldActive = false;
		protected bool italicActive = false;
		protected bool colorActive = false;
		protected string colorText = "";
		protected bool inputFlag;
		protected bool exitFlag;

		protected List<IWriterListener> writerListeners = new List<IWriterListener>();

		public string text 
		{
			get 
			{
				if (textUI != null)
				{
					return textUI.text;
				}
				else if (inputField != null)
				{
					return inputField.text;
				}
				else if (textMesh != null)
				{
					return textMesh.text;
				}
				return "";
			}
			
			set 
			{
				if (textUI != null)
				{
					textUI.text = value;
				}
				else if (inputField != null)
				{
					inputField.text = value;
				}
				else if (textMesh != null)
				{
					textMesh.text = value;
				}
			}
		}
		
		protected virtual void Awake()
		{
			GameObject go = targetTextObject;
			if (go == null)
			{
				go = gameObject;
			}

			textUI = go.GetComponent<Text>();
			inputField = go.GetComponent<InputField>();
			textMesh = go.GetComponent<TextMesh>();

			// Cache the list of child writer listeners
			foreach (Component component in GetComponentsInChildren<Component>())
			{
				IWriterListener writerListener = component as IWriterListener;
				if (writerListener != null)
				{
					writerListeners.Add(writerListener);
				}
			}
		}

		protected virtual void Start()
		{
			if (forceRichText)
			{
				if (textUI != null)
				{
					textUI.supportRichText = true;
				}

				// Input Field does not support rich text

				if (textMesh != null)
				{
					textMesh.richText = true;
				}
			}
		}
		
		public virtual bool HasTextObject()
		{
			return (textUI != null || inputField != null || textMesh != null);
		}
		
		public virtual bool SupportsRichText()
		{
			if (textUI != null)
			{
				return textUI.supportRichText;
			}
			if (inputField != null)
			{
				return false;
			}
			if (textMesh != null)
			{
				return textMesh.richText;
			}
			return false;
		}
		
		protected virtual string OpenMarkup()
		{
			string tagText = "";
			
			if (SupportsRichText())
			{
				if (colorActive)
				{
					tagText += "<color=" + colorText + ">"; 
				}
				if (boldActive)
				{
					tagText += "<b>"; 
				}
				if (italicActive)
				{
					tagText += "<i>"; 
				}			
			}
			
			return tagText;
		}
		
		protected virtual string CloseMarkup()
		{
			string closeText = "";
			
			if (SupportsRichText())
			{
				if (italicActive)
				{
					closeText += "</i>"; 
				}			
				if (boldActive)
				{
					closeText += "</b>"; 
				}
				if (colorActive)
				{
					closeText += "</color>"; 
				}
			}
			
			return closeText;		
		}

		public virtual void SetTextColor(Color textColor)
		{
			if (textUI != null)
			{
				textUI.color = textColor;
			}
			else if (inputField != null)
			{
				if (inputField.textComponent != null)
				{
					inputField.textComponent.color = textColor;
				}
			}
			else if (textMesh != null)
			{
				textMesh.color = textColor;
			}
		}
		
		public virtual void SetTextAlpha(float textAlpha)
		{
			if (textUI != null)
			{
				Color tempColor = textUI.color;
				tempColor.a = textAlpha;
				textUI.color = tempColor;
			}
			else if (inputField != null)
			{
				if (inputField.textComponent != null)
				{
					Color tempColor = inputField.textComponent.color;
					tempColor.a = textAlpha;
					inputField.textComponent.color = tempColor;
				}
			}
			else if (textMesh != null)
			{
				Color tempColor = textMesh.color;
				tempColor.a = textAlpha;
				textMesh.color = tempColor;
			}
		}

		public virtual void Stop()
		{
			exitFlag = true;
		}

		public virtual void Write(string content, bool clear, bool waitForInput, AudioClip audioClip, Action onComplete)
		{
			if (clear)
			{
				this.text = "";
			}
			
			if (!HasTextObject())
			{
				return;
			}

			// If this clip is null then WriterAudio will play the default sound effect (if any)
			NotifyStart(audioClip);

			string tokenText = content;
			if (waitForInput)
			{
				tokenText += "{wi}";
			}

			TextTagParser tagParser = new TextTagParser();
			List<TextTagParser.Token> tokens = tagParser.Tokenize(tokenText);

			StartCoroutine(ProcessTokens(tokens, onComplete));
		}
		
		protected virtual IEnumerator ProcessTokens(List<TextTagParser.Token> tokens, Action onComplete)
		{
			// Reset control members
			boldActive = false;
			italicActive = false;
			colorActive = false;
			colorText = "";
			currentPunctuationPause = punctuationPause;
			currentWritingSpeed = writingSpeed;

			exitFlag = false;
			isWriting = true;

			foreach (TextTagParser.Token token in tokens)
			{

				switch (token.type)
				{
				case TextTagParser.TokenType.Words:
					yield return StartCoroutine(DoWords(token.param));
					break;
					
				case TextTagParser.TokenType.BoldStart:
					boldActive = true;
					break;
					
				case TextTagParser.TokenType.BoldEnd:
					boldActive = false;
					break;
					
				case TextTagParser.TokenType.ItalicStart:
					italicActive = true;
					break;
					
				case TextTagParser.TokenType.ItalicEnd:
					italicActive = false;
					break;
					
				case TextTagParser.TokenType.ColorStart:
					colorActive = true;
					colorText = token.param;
					break;
					
				case TextTagParser.TokenType.ColorEnd:
					colorActive = false;
					break;
					
				case TextTagParser.TokenType.Wait:
					yield return StartCoroutine(DoWait(token.param));
					break;
					
				case TextTagParser.TokenType.WaitForInputNoClear:
					yield return StartCoroutine(DoWaitForInput(false));
					break;
					
				case TextTagParser.TokenType.WaitForInputAndClear:
					yield return StartCoroutine(DoWaitForInput(true));
					break;
					
				case TextTagParser.TokenType.WaitOnPunctuationStart:
					if (!Single.TryParse(token.param, out currentPunctuationPause))
					{
						currentPunctuationPause = punctuationPause;
					}
					break;
					
				case TextTagParser.TokenType.WaitOnPunctuationEnd:
					currentPunctuationPause = punctuationPause;
					break;
					
				case TextTagParser.TokenType.Clear:
					text = "";
					break;
					
				case TextTagParser.TokenType.SpeedStart:
					if (!Single.TryParse(token.param, out currentWritingSpeed))
					{
						currentWritingSpeed = writingSpeed;
					}
					break;
					
				case TextTagParser.TokenType.SpeedEnd:
					currentWritingSpeed = writingSpeed;
					break;
					
				case TextTagParser.TokenType.Exit:
					exitFlag = true;
					break;
					
				case TextTagParser.TokenType.Message:
					Flowchart.BroadcastFungusMessage(token.param);
					break;
					
				case TextTagParser.TokenType.VerticalPunch:
					float vintensity;
					if (!Single.TryParse(token.param, out vintensity))
					{
						vintensity = 10f;
					}
					Punch(new Vector3(0, vintensity, 0), 0.5f);
					break;
					
				case TextTagParser.TokenType.HorizontalPunch:
					float hintensity;
					if (!Single.TryParse(token.param, out hintensity))
					{
						hintensity = 10f;
					}
					Punch(new Vector3(hintensity, 0, 0), 0.5f);
					break;
					
				case TextTagParser.TokenType.Punch:
					float intensity;
					if (!Single.TryParse(token.param, out intensity))
					{
						intensity = 10f;
					}
					Punch(new Vector3(intensity, intensity, 0), 0.5f);
					break;
					
				case TextTagParser.TokenType.Flash:
					float flashDuration;
					if (!Single.TryParse(token.param, out flashDuration))
					{
						flashDuration = 0.2f;
					}
					Flash(flashDuration);
					break;
					
				case TextTagParser.TokenType.Audio:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.PlayOneShot(audioSource.clip);
						}
					}
					break;
					
				case TextTagParser.TokenType.AudioLoop:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.Play();
							audioSource.loop = true;
						}
					}
					break;
					
				case TextTagParser.TokenType.AudioPause:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.Pause();
						}
					}
					break;
					
				case TextTagParser.TokenType.AudioStop:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.Stop();
						}
					}
					break;
				}
				
				if (exitFlag)
				{
					break;
				}
			}

			inputFlag = false;
			exitFlag = false;
			isWaitingForInput = false;
			isWriting = false;

			NotifyEnd();

			if (onComplete != null)
			{
				onComplete();
			}
		}
		
		protected virtual IEnumerator DoWords(string param)
		{
			string startText = text;
			string openText = OpenMarkup();
			string closeText = CloseMarkup();

			float timeAccumulator = Time.deltaTime;

			for (int i = 0; i < param.Length; ++i)
			{
				// Exit immediately if the exit flag has been set
				if (exitFlag)
				{
					break;
				}

				string left = "";
				string right = "";
				
				PartitionString(writeWholeWords, param, i, out left, out right);
				text = ConcatenateString(startText, openText, closeText, left, right);

				NotifyGlyph();

				// No delay if user has clicked
				if (inputFlag)
				{
					continue;
				}

				// Punctuation pause
				if (left.Length > 0 && 
				    right.Length > 0 &&
				    IsPunctuation(left.Substring(left.Length - 1)[0]))
				{
					yield return StartCoroutine(DoWait(currentPunctuationPause));
				}

				// Delay between characters
				if (currentWritingSpeed > 0f)
				{
					if (timeAccumulator > 0f)
					{
						timeAccumulator -= 1f / currentWritingSpeed;
					}
					else
					{
						yield return new WaitForSeconds(1f / currentWritingSpeed);
					}
				}
			}
		}

		protected void PartitionString(bool wholeWords, string inputString, int i, out string left, out string right)
		{
			left = "";
			right = "";

			if (wholeWords)
			{
				// Look ahead to find next whitespace or end of string
				for (int j = i; j < inputString.Length; ++j)
				{
					if (Char.IsWhiteSpace(inputString[j]) ||
					    j == inputString.Length - 1)
					{
						left = inputString.Substring(0, j + 1);
						right = inputString.Substring(j + 1, inputString.Length - j - 1);
						break;
					}
				}
			}
			else
			{
				left = inputString.Substring(0, i + 1);
				right = inputString.Substring(i + 1);
			}
		}

		protected string ConcatenateString(string startText, string openText, string closeText, string leftText, string rightText)
		{
			string tempText = startText + openText + leftText + closeText;
		
			Color32 c = hiddenTextColor;
			string hiddenColor = String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);

			// Make right hand side text hidden
			if (SupportsRichText() &&
			    rightText.Length > 0)
			{
				tempText += "<color=" + hiddenColor + ">" + rightText + "</color>";
			}

			return tempText;
		}

		public virtual string GetTagHelp()
		{
			return "";
		}
		
		protected virtual IEnumerator DoWait(string param)
		{
			float duration = 1f;
			if (!Single.TryParse(param, out duration))
			{
				duration = 1f;
			}

			NotifyPause();

			float timeRemaining = duration;
			while (timeRemaining > 0f && !inputFlag)
			{
				timeRemaining -= Time.deltaTime;
				yield return null;
			}

			NotifyResume();
		}

		protected virtual IEnumerator DoWait(float duration)
		{
			NotifyPause();

			float timeRemaining = duration;
			while (timeRemaining > 0f && !inputFlag && !exitFlag)
			{
				timeRemaining -= Time.deltaTime;
				yield return null;
			}

			NotifyResume();
		}

		protected virtual IEnumerator DoWaitForInput(bool clear)
		{
			NotifyPause();

			inputFlag = false;
			isWaitingForInput = true;

			while (!inputFlag && !exitFlag)
			{
				yield return null;
			}
		
			isWaitingForInput = false;			
			inputFlag = false;

			if (clear)
			{
				textUI.text = "";
			}

			NotifyResume();
		}
		
		protected virtual bool IsPunctuation(char character)
		{
			return character == '.' || 
				character == '?' ||  
					character == '!' || 
					character == ',' ||
					character == ':' ||
					character == ';' ||
					character == ')';
		}
		
		protected virtual void Punch(Vector3 axis, float time)
		{
			iTween.ShakePosition(this.gameObject, axis, time);
		}
		
		protected virtual void Flash(float duration)
		{
			CameraController cameraController = CameraController.GetInstance();
			cameraController.screenFadeTexture = CameraController.CreateColorTexture(new Color(1f,1f,1f,1f), 32, 32);
			cameraController.Fade(1f, duration, delegate {
				cameraController.screenFadeTexture = CameraController.CreateColorTexture(new Color(1f,1f,1f,1f), 32, 32);
				cameraController.Fade(0f, duration, null);
			});
		}
		
		protected virtual AudioSource FindAudio(string audioObjectName)
		{
			GameObject go = GameObject.Find(audioObjectName);
			if (go == null)
			{
				return null;
			}
			
			return go.GetComponent<AudioSource>();
		}

		protected virtual void NotifyInput()
		{
			foreach (IWriterListener writerListener in writerListeners)
			{
				writerListener.OnInput();
			}
		}


		protected virtual void NotifyStart(AudioClip audioClip)
		{
			foreach (IWriterListener writerListener in writerListeners)
			{
				writerListener.OnStart(audioClip);
			}
		}

		protected virtual void NotifyPause()
		{
			foreach (IWriterListener writerListener in writerListeners)
			{
				writerListener.OnPause();
			}
		}

		protected virtual void NotifyResume()
		{
			foreach (IWriterListener writerListener in writerListeners)
			{
				writerListener.OnResume();
			}
		}

		protected virtual void NotifyEnd()
		{
			foreach (IWriterListener writerListener in writerListeners)
			{
				writerListener.OnEnd();
			}
		}

		protected virtual void NotifyGlyph()
		{
			foreach (IWriterListener writerListener in writerListeners)
			{
				writerListener.OnGlyph();
			}
		}

		//
		// IDialogInputListener implementation
		//
		public virtual void OnNextLineEvent()
		{
			inputFlag = true;

			if (isWriting)
			{
				NotifyInput();
			}
		}
	}

}
