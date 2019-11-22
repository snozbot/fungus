using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace Fungus
{
    /// <summary>
    /// Helper class for hiding the many, many ways we might want to show text to the user.
    /// </summary>
    public class TextAdapter : IWriterTextDestination
    {
        protected Text textUI;
        protected InputField inputField;
        protected TextMesh textMesh;
        protected Component textComponent;
        protected PropertyInfo textProperty;
        protected IWriterTextDestination writerTextDestination;

        public void InitFromGameObject(GameObject go, bool includeChildren = false)
        {
            if (go == null)
            {
                return;
            }

            if (!includeChildren)
            {
                textUI = go.GetComponent<Text>();
                inputField = go.GetComponent<InputField>();
                textMesh = go.GetComponent<TextMesh>();
                writerTextDestination = go.GetComponent<IWriterTextDestination>();
            }
            else
            {
                textUI = go.GetComponentInChildren<Text>();
                inputField = go.GetComponentInChildren<InputField>();
                textMesh = go.GetComponentInChildren<TextMesh>();
                writerTextDestination = go.GetComponentInChildren<IWriterTextDestination>();
            }
            
            // Try to find any component with a text property
            if (textUI == null && inputField == null && textMesh == null && writerTextDestination == null)
            {
                Component[] allcomponents = null;
                if (!includeChildren)
                    allcomponents = go.GetComponents<Component>();
                else
                    allcomponents = go.GetComponentsInChildren<Component>();

                for (int i = 0; i < allcomponents.Length; i++)
                {
                    var c = allcomponents[i];
                    textProperty = c.GetType().GetProperty("text");
                    if (textProperty != null)
                    {
                        textComponent = c;
                        break;
                    }
                }
            }
        }

        public void ForceRichText()
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

            if(writerTextDestination != null)
            {
                writerTextDestination.ForceRichText();
            }
        }

        public void SetTextColor(Color textColor)
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
            else if (writerTextDestination != null)
            {
                writerTextDestination.SetTextColor(textColor);
            }
        }

        public void SetTextAlpha(float textAlpha)
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
            else if (writerTextDestination != null)
            {
                writerTextDestination.SetTextAlpha(textAlpha);
            }
        }

        public bool HasTextObject()
        {
            return (textUI != null || inputField != null || textMesh != null || textComponent != null || writerTextDestination != null);
        }

        public bool SupportsRichText()
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
            if (writerTextDestination != null)
            {
                return writerTextDestination.SupportsRichText();
            }
            return false;
        }

        public virtual string Text
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
                else if (writerTextDestination != null)
                {
                    return Text;
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
                else if (writerTextDestination != null)
                {
                    Text = value;
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
    }
}