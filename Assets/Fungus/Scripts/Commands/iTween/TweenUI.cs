// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;
using Fungus;

namespace Fungus
{
    /// <summary>
    /// Abstract base class for TweenUI commands.
    /// </summary>
    public abstract class TweenUI : Command 
    {
        [Tooltip("List of objects to be affected by the tween")]
        [SerializeField] protected List<GameObject> targetObjects = new List<GameObject>();
        
        [Tooltip("Type of tween easing to apply")]
        [SerializeField] protected LeanTweenType tweenType = LeanTweenType.easeOutQuad;
        
        [Tooltip("Wait until this command completes before continuing execution")]
        [SerializeField] protected BooleanData waitUntilFinished = new BooleanData(true);
        
        [Tooltip("Time for the tween to complete")]
        [SerializeField] protected FloatData duration = new FloatData(1f);

        protected virtual void ApplyTween()
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                var targetObject = targetObjects[i];
                if (targetObject == null)
                {
                    continue;
                }
                ApplyTween(targetObject);
            }

            if (waitUntilFinished)
            {
                LeanTween.value(gameObject, 0f, 1f, duration).setOnComplete(OnComplete);
            }
        }

        protected abstract void ApplyTween(GameObject go);

        protected virtual void OnComplete()
        {
            Continue();
        }

        protected virtual string GetSummaryValue()
        {
            return "";
        }

        #region Public members

        public override void OnEnter()
        {
            if (targetObjects.Count == 0)
            {
                Continue();
                return;
            }
            
            ApplyTween();

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add an empty slot by default. Saves an unnecessary user click.
            if (targetObjects.Count == 0)
            {
                targetObjects.Add(null);
            }
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
                return targetObjects[0].name + " = " + GetSummaryValue();
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
            
            return objectList + " = " + GetSummaryValue();
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetObjects")
            {
                return true;
            }

            return false;
        }

        public override bool HasReference(Variable variable)
        {
            return waitUntilFinished.booleanRef == variable || duration.floatRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}