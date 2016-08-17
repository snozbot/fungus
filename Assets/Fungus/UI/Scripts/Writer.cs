/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;

namespace Fungus
{

    /**
     * Implement this interface to be notified about Writer events
     */
    public interface IWriterListener
    {
        ///
        /// Called when a user input event (e.g. a click) has been handled by the Writer.
        ///
        void OnInput();

        /// Called when the Writer starts writing new text
        /// <param name="audioClip">An optional audioClip sound effect can be supplied (e.g. for voiceover)</param>
        void OnStart(AudioClip audioClip);

        /// Called when the Writer has paused writing text (e.g. on a {wi} tag).
        void OnPause();

        /// Called when the Writer has resumed writing text.
        void OnResume();

        /// Called when the Writer has finshed writing text.
        /// <param name="stopAudio">Controls whether audio should be stopped when writing ends.</param>
        void OnEnd(bool stopAudio);

        /// Called every time the Writer writes a new character glyph.
        void OnGlyph();
    }
    
    public class Writer : MonoBehaviour, IDialogInputListener
    {
        [Tooltip("Gameobject containing a Text, Inout Field or Text Mesh object to write to")]
        public GameObject targetTextObject;

        [Tooltip("Gameobject to punch when the punch tags are displayed. If none is set, the main camera will shake instead.")]
        public GameObject punchObject;

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

        [Tooltip("Click while text is writing to finish writing immediately")]
        public bool instantComplete = true;

        // This property is true when the writer is waiting for user input to continue
        [System.NonSerialized]
        public bool isWaitingForInput;

        // This property is true when the writer is writing text or waiting (i.e. still processing tokens)
        [System.NonSerialized]
        public bool isWriting;

        protected float currentWritingSpeed;
        protected float currentPunctuationPause;
        protected Text textUI;
        protected InputField inputField;
        protected TextMesh textMesh;
        protected Component textComponent;
        protected PropertyInfo textProperty;

        protected bool boldActive = false;
        protected bool italicActive = false;
        protected bool colorActive = false;
        protected string colorText = "";
        protected bool sizeActive = false;
        protected float sizeValue = 16f;
        protected bool inputFlag;
        protected bool exitFlag;

        protected List<IWriterListener> writerListeners = new List<IWriterListener>();

        protected StringBuilder openString = new StringBuilder(256);
        protected StringBuilder closeString = new StringBuilder(256);
        protected StringBuilder leftString = new StringBuilder(1024);
        protected StringBuilder rightString = new StringBuilder(1024);
        protected StringBuilder outputString = new StringBuilder(1024);

        protected string hiddenColorOpen = "";
        protected string hiddenColorClose = "";

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
                else if (textProperty != null)
                {
                    return textProperty.GetValue(textComponent, null) as string;
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
                else if (textProperty != null)
                {
                    textProperty.SetValue(textComponent, value, null);
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

            // Try to find any component with a text property
            if (textUI == null && inputField == null && textMesh == null)
            {
                foreach (Component c in go.GetComponents<Component>())
                {
                    textProperty = c.GetType().GetProperty("text");
                    if (textProperty != null)
                    {
                        textComponent = c;
                        break;
                    }
                }
            }

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
            // Cache the hidden color string
            Color32 c = hiddenTextColor;
            hiddenColorOpen = String.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>", c.r, c.g, c.b, c.a);
            hiddenColorClose = "</color>";

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
            return (textUI != null || inputField != null || textMesh != null || textComponent != null);
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
        
        protected virtual void UpdateOpenMarkup()
        {
            openString.Length = 0;
            
            if (SupportsRichText())
            {
                if (sizeActive)
                {
                    openString.Append("<size=");
                    openString.Append(sizeValue);
                    openString.Append(">"); 
                }
                if (colorActive)
                {
                    openString.Append("<color=");
                    openString.Append(colorText);
                    openString.Append(">"); 
                }
                if (boldActive)
                {
                    openString.Append("<b>"); 
                }
                if (italicActive)
                {
                    openString.Append("<i>"); 
                }           
            }
        }
        
        protected virtual void UpdateCloseMarkup()
        {
            closeString.Length = 0;
            
            if (SupportsRichText())
            {
                if (italicActive)
                {
                    closeString.Append("</i>"); 
                }           
                if (boldActive)
                {
                    closeString.Append("</b>"); 
                }
                if (colorActive)
                {
                    closeString.Append("</color>"); 
                }
                if (sizeActive)
                {
                    closeString.Append("</size>"); 
                }
            }
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
            if (isWriting || isWaitingForInput)
            {
                exitFlag = true;
            }
        }

        public virtual IEnumerator Write(string content, bool clear, bool waitForInput, bool stopAudio, AudioClip audioClip, Action onComplete)
        {
            if (clear)
            {
                this.text = "";
            }
            
            if (!HasTextObject())
            {
                yield break;
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

            gameObject.SetActive(true);

            yield return StartCoroutine(ProcessTokens(tokens, stopAudio, onComplete));
        }

        virtual protected bool CheckParamCount(List<string> paramList, int count) 
        {
            if (paramList == null)
            {
                Debug.LogError("paramList is null");
                return false;
            }
            if (paramList.Count != count)
            {
                Debug.LogError("There must be exactly " + paramList.Count + " parameters.");
                return false;
            }
            return true;
        }


        protected virtual bool TryGetSingleParam(List<string> paramList, int index, float defaultValue, out float value) 
        {
            value = defaultValue;
            if (paramList.Count > index) 
            {
                Single.TryParse(paramList[index], out value);
                return true;
            }
            return false;
        }

        protected virtual IEnumerator ProcessTokens(List<TextTagParser.Token> tokens, bool stopAudio, Action onComplete)
        {
            // Reset control members
            boldActive = false;
            italicActive = false;
            colorActive = false;
            sizeActive = false;
            colorText = "";
            sizeValue = 16f;
            currentPunctuationPause = punctuationPause;
            currentWritingSpeed = writingSpeed;

            exitFlag = false;
            isWriting = true;

            TextTagParser.TokenType previousTokenType = TextTagParser.TokenType.Invalid;

            foreach (TextTagParser.Token token in tokens)
            {

                switch (token.type)
                {
                case TextTagParser.TokenType.Words:
                    yield return StartCoroutine(DoWords(token.paramList, previousTokenType));
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
                    if (CheckParamCount(token.paramList, 1)) 
                    {
                        colorActive = true;
                        colorText = token.paramList[0];
                    }
                    break;
                    
                case TextTagParser.TokenType.ColorEnd:
                    colorActive = false;
                    break;

                case TextTagParser.TokenType.SizeStart:
                    if (TryGetSingleParam(token.paramList, 0, 16f, out sizeValue))
                    {
                        sizeActive = true;
                    }
                    break;

                case TextTagParser.TokenType.SizeEnd:
                    sizeActive = false;
                    break;

                case TextTagParser.TokenType.Wait:
                    yield return StartCoroutine(DoWait(token.paramList));
                    break;
                    
                case TextTagParser.TokenType.WaitForInputNoClear:
                    yield return StartCoroutine(DoWaitForInput(false));
                    break;
                    
                case TextTagParser.TokenType.WaitForInputAndClear:
                    yield return StartCoroutine(DoWaitForInput(true));
                    break;
                    
                case TextTagParser.TokenType.WaitOnPunctuationStart:
                    TryGetSingleParam(token.paramList, 0, punctuationPause, out currentPunctuationPause);
                    break;
                    
                case TextTagParser.TokenType.WaitOnPunctuationEnd:
                    currentPunctuationPause = punctuationPause;
                    break;
                    
                case TextTagParser.TokenType.Clear:
                    text = "";
                    break;
                    
                case TextTagParser.TokenType.SpeedStart:
                    TryGetSingleParam(token.paramList, 0, writingSpeed, out currentWritingSpeed);
                    break;
                    
                case TextTagParser.TokenType.SpeedEnd:
                    currentWritingSpeed = writingSpeed;
                    break;
                    
                case TextTagParser.TokenType.Exit:
                    exitFlag = true;
                    break;


                case TextTagParser.TokenType.Message:
                    if (CheckParamCount(token.paramList, 1)) 
                    {
                        Flowchart.BroadcastFungusMessage(token.paramList[0]);
                    }
                    break;
                    
                case TextTagParser.TokenType.VerticalPunch: 
                    {
                        float vintensity;
                        float time;
                        TryGetSingleParam(token.paramList, 0, 10.0f, out vintensity);
                        TryGetSingleParam(token.paramList, 1, 0.5f, out time);
                        Punch(new Vector3(0, vintensity, 0), time);
                    }
                    break;
                    
                case TextTagParser.TokenType.HorizontalPunch: 
                    {
                        float hintensity;
                        float time;
                        TryGetSingleParam(token.paramList, 0, 10.0f, out hintensity);
                        TryGetSingleParam(token.paramList, 1, 0.5f, out time);
                        Punch(new Vector3(hintensity, 0, 0), time);
                    }
                    break;
                    
                case TextTagParser.TokenType.Punch: 
                    {
                        float intensity;
                        float time;
                        TryGetSingleParam(token.paramList, 0, 10.0f, out intensity);
                        TryGetSingleParam(token.paramList, 1, 0.5f, out time);
                        Punch(new Vector3(intensity, intensity, 0), time);
                    }
                    break;
                    
                case TextTagParser.TokenType.Flash:
                    float flashDuration;
                    TryGetSingleParam(token.paramList, 0, 0.2f, out flashDuration);
                    Flash(flashDuration);
                    break;

                case TextTagParser.TokenType.Audio: 
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1))
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                    }
                    break;
                    
                case TextTagParser.TokenType.AudioLoop:
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1)) 
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.Play();
                            audioSource.loop = true;
                        }
                    }
                    break;
                    
                case TextTagParser.TokenType.AudioPause:
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1)) 
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.Pause();
                        }
                    }
                    break;
                    
                case TextTagParser.TokenType.AudioStop:
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1)) 
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.Stop();
                        }
                    }
                    break;
                }

                previousTokenType = token.type;

                if (exitFlag)
                {
                    break;
                }
            }

            inputFlag = false;
            exitFlag = false;
            isWaitingForInput = false;
            isWriting = false;

            NotifyEnd(stopAudio);

            if (onComplete != null)
            {
                onComplete();
            }
        }

        protected virtual IEnumerator DoWords(List<string> paramList, TextTagParser.TokenType previousTokenType)
        {
            if (!CheckParamCount(paramList, 1))
            {
                yield break;
            }

            string param = paramList[0];

            // Trim whitespace after a {wc} or {c} tag
            if (previousTokenType == TextTagParser.TokenType.WaitForInputAndClear ||
                previousTokenType == TextTagParser.TokenType.Clear)
            {
                param = param.TrimStart(' ', '\t', '\r', '\n');
            }
            
            string startText = text;
            UpdateOpenMarkup();
            UpdateCloseMarkup();

            float timeAccumulator = Time.deltaTime;

            for (int i = 0; i < param.Length + 1; ++i)
            {
                // Exit immediately if the exit flag has been set
                if (exitFlag)
                {
                    break;
                }

                PartitionString(writeWholeWords, param, i);
                ConcatenateString(startText);
                text = outputString.ToString();

                NotifyGlyph();

                // No delay if user has clicked and Instant Complete is enabled
                if (instantComplete && inputFlag)
                {
                    continue;
                }

                // Punctuation pause
                if (leftString.Length > 0 && 
                    rightString.Length > 0 &&
                    IsPunctuation(leftString.ToString(leftString.Length - 1, 1)[0]))
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

        protected void PartitionString(bool wholeWords, string inputString, int i)
        {
            leftString.Length = 0;
            rightString.Length = 0;

            // Reached last character
            leftString.Append(inputString);
            if (i >= inputString.Length)
            {
                return;
            }

            rightString.Append(inputString);

            if (wholeWords)
            {
                // Look ahead to find next whitespace or end of string
                for (int j = i; j < inputString.Length + 1; ++j)
                {
                    if (j == inputString.Length || Char.IsWhiteSpace(inputString[j]))
                    {
                        leftString.Length = j;
                        rightString.Remove(0, j);
                        break;
                    }
                }
            }
            else
            {
                leftString.Remove(i, inputString.Length - i);
                rightString.Remove(0, i);
            }
        }

        protected void ConcatenateString(string startText)
        {
            outputString.Length = 0;

            // string tempText = startText + openText + leftText + closeText;
            outputString.Append(startText);
            outputString.Append(openString);
            outputString.Append(leftString);
            outputString.Append(closeString);

            // Make right hand side text hidden
            if (SupportsRichText() &&
                rightString.Length > 0)
            {
                outputString.Append(hiddenColorOpen);
                outputString.Append(rightString);
                outputString.Append(hiddenColorClose);
            }
        }

        public virtual string GetTagHelp()
        {
            return "";
        }
        
        protected virtual IEnumerator DoWait(List<string> paramList)
        {
            var param = "";
            if (paramList.Count == 1)
            {
                param = paramList[0];
            }

            float duration = 1f;
            if (!Single.TryParse(param, out duration))
            {
                duration = 1f;
            }

            yield return StartCoroutine( DoWait(duration) );
        }

        protected virtual IEnumerator DoWait(float duration)
        {
            NotifyPause();

            float timeRemaining = duration;
            while (timeRemaining > 0f && !exitFlag)
            {
                if (instantComplete && inputFlag)
                {
                    break;
                }

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
            GameObject go = punchObject;
            if (go == null)
            {
                go = Camera.main.gameObject;
            }

            if (go != null)
            {
                iTween.ShakePosition(go, axis, time);
            }
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

        protected virtual void NotifyEnd(bool stopAudio)
        {
            foreach (IWriterListener writerListener in writerListeners)
            {
                writerListener.OnEnd(stopAudio);
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
