// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Set the interactable state of selectable objects.
    /// </summary>
    [CommandInfo("UI", 
                 "Set Interactable", 
                 "Set the interactable sate of selectable objects.")]
    public class SetInteractable : Command 
    {
        [Tooltip("List of objects to be affected by the command")]
        [SerializeField] protected List<GameObject> targetObjects = new List<GameObject>();

        [Tooltip("Controls if the selectable UI object be interactable or not")]
        [SerializeField] protected BooleanData interactableState = new BooleanData(true);

        #region Public members

        public override void OnEnter()
        {
            if (targetObjects.Count == 0)
            {
                Continue();
                return;
            }

            for (int i = 0; i < targetObjects.Count; i++)
            {
                var targetObject = targetObjects[i];
                var selectables = targetObject.GetComponents<Selectable>();
                for (int j = 0; j < selectables.Length; j++)
                {
                    var selectable = selectables[j];
                    selectable.interactable = interactableState.Value;
                }
            }
                
            Continue();
        }

        public override string GetSummary()
        {
            if (targetObjects.Count == 0)
            {
                return "Error: No targetObjects selected";
            }
            else if (targetObjects.Count == 1)
            {
                if (targetObjects[0] == null)
                {
                    return "Error: No targetObjects selected";
                }
                return targetObjects[0].name + " = " + interactableState.Value;
            }
            
            string objectList = "";
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var go = targetObjects[i];
                if (go == null)
                {
                    continue;
                }
                if (objectList == "")
                {
                    objectList += go.name;
                }
                else
                {
                    objectList += ", " + go.name;
                }
            }
            
            return objectList + " = " + interactableState.Value;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            targetObjects.Add(null);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetObjects")
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}