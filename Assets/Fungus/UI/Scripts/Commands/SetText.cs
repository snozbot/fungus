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
                 "Set Text", 
                 "Sets the text property on a UI Text object and/or an Input Field object.")]
    
    [AddComponentMenu("")]
    public class SetText : Command, ILocalizable 
    {
        [Tooltip("Text object to set text on. Can be a UI Text, Text Field or Text Mesh object.")]
        public GameObject targetTextObject;
        
        [Tooltip("String value to assign to the text object")]
        [FormerlySerializedAs("stringData")]
        public StringDataMulti text;

        [Tooltip("Notes about this story text for other authors, localization, etc.")]
        public string description;
        
        public override void OnEnter()
        {
            Flowchart flowchart = GetFlowchart();
            string newText = flowchart.SubstituteVariables(text.Value);
            
            if (targetTextObject == null)
            {
                Continue();
                return;
            }
            
            // Use first component found of Text, Input Field or Text Mesh type
            Text uiText = targetTextObject.GetComponent<Text>();
            if (uiText != null)
            {
                uiText.text = newText;
            }
            else
            {
                InputField inputField = targetTextObject.GetComponent<InputField>();
                if (inputField != null)
                {
                    inputField.text = newText;
                }
                else
                {
                    TextMesh textMesh = targetTextObject.GetComponent<TextMesh>();
                    if (textMesh != null)
                    {
                        textMesh.text = newText;
                    }
                }
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (targetTextObject != null)
            {
                return targetTextObject.name + " : " + text.Value;
            }
            
            return "Error: No text object selected";
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

        //
        // ILocalizable implementation
        //
        
        public virtual string GetStandardText()
        {
            return text;
        }

        public virtual void SetStandardText(string standardText)
        {
            text.Value = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Set Text commands is SETTEXT.<Localization Id>.<Command id>
            return "SETTEXT." + GetFlowchartLocalizationId() + "." + itemId;
        }
    }
    
}