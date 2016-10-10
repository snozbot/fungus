// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the Renderer sorting layer of every child of a game object. Applies to all Renderers (including mesh, skinned mesh, and sprite).
    /// </summary>
    [CommandInfo("Sprite", 
                 "Set Sorting Layer", 
                 "Sets the Renderer sorting layer of every child of a game object. Applies to all Renderers (including mesh, skinned mesh, and sprite).")]
    [AddComponentMenu("")]
    public class SetSortingLayer : Command 
    {
        [Tooltip("Root Object that will have the Sorting Layer set. Any children will also be affected")]
        [SerializeField] protected GameObject targetObject;
        
        [Tooltip("The New Layer Name to apply")]
        [SerializeField] protected string sortingLayer;

        protected void ApplySortingLayer(Transform target, string layerName) 
        {
            var renderer = target.gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.sortingLayerName = layerName;
                Debug.Log(target.name);
            }

            var targetTransform = target.transform;
            foreach (Transform child in targetTransform)
            {
                ApplySortingLayer(child, layerName);
            }
        }       

        #region Public members

        public override void OnEnter()
        {
            if (targetObject != null)
            {
                ApplySortingLayer(targetObject.transform, sortingLayer);
            }
            
            Continue();
        }
        
        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "Error: No game object selected";
            }
            
            return targetObject.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}