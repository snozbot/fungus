/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Serialization;

namespace Fungus
{
    [CommandInfo("UI", 
                 "Get Text", 
                 "Gets the text property from a UI Text object and stores it in a string variable.")]
    
    [AddComponentMenu("")]
    public class GetText : Command 
    {
        [Tooltip("Text object to get text value from")]
        public GameObject targetTextObject;
        
        [Tooltip("String variable to store the text value in")]
        [VariableProperty(typeof(StringVariable))]
        public StringVariable stringVariable;
        
        public override void OnEnter()
        {
            if (stringVariable == null)
            {
                Continue();
                return;
            }
            
            if (targetTextObject != null)
            {
                // Use first component found of Text, Input Field or Text Mesh type
                Text uiText = targetTextObject.GetComponent<Text>();
                if (uiText != null)
                {
                    stringVariable.value = uiText.text;
                }
                else
                {
                    InputField inputField = targetTextObject.GetComponent<InputField>();
                    if (inputField != null)
                    {
                        stringVariable.value = inputField.text;
                    }
                    else
                    {
                        TextMesh textMesh = targetTextObject.GetComponent<TextMesh>();
                        if (textMesh != null)
                        {
                            stringVariable.value = textMesh.text;
                        }
                    }
                }
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (targetTextObject == null)
            {
                return "Error: No text object selected";
            }
            
            if (stringVariable == null)
            {
                return "Error: No variable selected";
            }
            
            return targetTextObject.name + " : " + stringVariable.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        // Backwards compatibility with Fungus v2.1.2
        [HideInInspector]
        [FormerlySerializedAs("textObject")]
        public Text _textObjectObsolete;
        protected virtual void OnEnable()
        {
            if (_textObjectObsolete != null)
            {
                targetTextObject = _textObjectObsolete.gameObject;
            }
        }
    }
    
}