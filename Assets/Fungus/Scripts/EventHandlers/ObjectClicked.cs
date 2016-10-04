// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user clicks or taps on the clickable object.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Object Clicked",
                      "The block will execute when the user clicks or taps on the clickable object.")]
    [AddComponentMenu("")]
    public class ObjectClicked : EventHandler
    {   
        [Tooltip("Object that the user can click or tap on")]
        [SerializeField] protected Clickable2D clickableObject;

        #region Public members

        /// <summary>
        /// Called by the Clickable2D object when it is clicked.
        /// </summary>
        public virtual void OnObjectClicked(Clickable2D clickableObject)
        {
            if (clickableObject == this.clickableObject)
            {
                ExecuteBlock();
            }
        }

        public override string GetSummary()
        {
            if (clickableObject != null)
            {
                return clickableObject.name;
            }

            return "None";
        }

        #endregion
    }
}
