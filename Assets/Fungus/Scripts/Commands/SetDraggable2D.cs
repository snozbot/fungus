// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a Draggable2D component to be draggable / non-draggable.
    /// </summary>
    [CommandInfo("Sprite",
                 "Set Draggable 2D",
                 "Sets a Draggable2D component to be draggable / non-draggable.")]
    [AddComponentMenu("")]
    public class SetDraggable2D : Command
    {      
        [Tooltip("Reference to Draggable2D component on a gameobject")]
        [SerializeField] protected Draggable2D targetDraggable2D;

        [Tooltip("Set to true to enable the component")]
        [SerializeField] protected BooleanData activeState;

        #region Public members

        public override void OnEnter() 
        {
            if (targetDraggable2D != null)         
            {
                targetDraggable2D.DragEnabled = activeState.Value;     
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (targetDraggable2D == null)         
            {
                return "Error: No Draggable2D component selected";     
            }

            return targetDraggable2D.gameObject.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return activeState.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}