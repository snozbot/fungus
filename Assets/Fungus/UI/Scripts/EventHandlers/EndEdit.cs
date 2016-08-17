/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fungus 
{
    [EventHandlerInfo("UI",
                      "End Edit",
                      "The block will execute when the user finishes editing the text in the input field.")]
    [AddComponentMenu("")]
    public class EndEdit : EventHandler
    {   
        [Tooltip("The UI Input Field that the user can enter text into")]
        public InputField targetInputField;
        
        public virtual void Start()
        {
            targetInputField.onEndEdit.AddListener(OnEndEdit);
        }
        
        protected virtual void OnEndEdit(string text)
        {
            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (targetInputField != null)
            {
                return targetInputField.name;
            }

            return "None";
        }
    }
}
