// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Text coloring mode for Write command.
    /// </summary>
    public enum TextColor
    {
        /// <summary> Don't change the text color. </summary>
        Default,
        /// <summary> Set the text alpha to 1. </summary>
        SetVisible,
        /// <summary> Set the text alpha to a value. </summary>
        SetAlpha,
        /// <summary> Set the text color to a value. </summary>
        SetColor
    }

    /// <summary>
    /// Writes content to a UI Text or Text Mesh object.
    /// </summary>
    [CommandInfo("UI", 
                 "Write", 
                 "Writes content to a UI Text or Text Mesh object.")]
    [AddComponentMenu("")]
    public class Write : Command, ILocalizable
    {
        [Tooltip("Text object to set text on. Text, Input Field and Text Mesh objects are supported.")]
        [SerializeField] protected GameObject textObject;

        [Tooltip("String value to assign to the text object")]
        [SerializeField] protected StringDataMulti text;

        [Tooltip("Notes about this story text for other authors, localization, etc.")]
        [SerializeField] protected string description;

        [Tooltip("Clear existing text before writing new text")]
        [SerializeField] protected bool clearText = true;

        [Tooltip("Wait until this command finishes before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        [Tooltip("Color mode to apply to the text.")]
        [SerializeField] protected TextColor textColor = TextColor.Default;

        [Tooltip("Alpha to apply to the text.")]
        [SerializeField] protected FloatData setAlpha = new FloatData(1f);

        [Tooltip("Color to apply to the text.")]
        [SerializeField] protected ColorData setColor = new ColorData(Color.white);

        protected Writer GetWriter()
        {
            var writer = textObject.GetComponent<Writer>();
            if (writer == null)
            {
                writer = textObject.AddComponent<Writer>();
            }
            
            return writer;
        }

        #region Public members

        public override void OnEnter()
        {
            if (textObject == null)
            {
                Continue();
                return;
            }
        
            var writer = GetWriter();
            if (writer == null)
            {
                Continue();
                return;
            }

            switch (textColor)
            {
            case TextColor.SetAlpha:
                writer.SetTextAlpha(setAlpha);
                break;
            case TextColor.SetColor:
                writer.SetTextColor(setColor);
                break;
            case TextColor.SetVisible:
                writer.SetTextAlpha(1f);
                break;
            }

            var flowchart = GetFlowchart();
            string newText = flowchart.SubstituteVariables(text.Value);

            if (!waitUntilFinished)
            {
                StartCoroutine(writer.Write(newText, clearText, false, true, false, null, null));
                Continue();
            }
            else
            {
                StartCoroutine(writer.Write(newText, clearText, false, true, false, null,
                             () => { Continue (); }
                ));
            }
        }

        public override string GetSummary()
        {
            if (textObject != null)
            {
                return textObject.name + " : " + text.Value;
            }

            return "Error: No text object selected";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override void OnStopExecuting()
        {
            GetWriter().Stop();
        }

        #endregion

        #region ILocalizable implementation

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
            // String id for Write commands is WRITE.<Localization Id>.<Command id>
            return "WRITE." + GetFlowchartLocalizationId() + "." + itemId;
        }

        public override bool HasReference(Variable variable)
        {
            return text.stringRef == variable || setAlpha.floatRef == variable || setColor.colorRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}