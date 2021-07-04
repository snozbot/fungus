// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// 
    /// </summary> 
    [CommandInfo("LeanTween",
                 "Alpha Canvas",
                 "Fade in or out a Canvas Group. 0 is the lowest, 1 is the highest")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class AlphaCanvas : Command
    {
        [Tooltip("Target game object to Pause or Resume LeanTweens on")]
        [SerializeField]
        protected GameObjectData _targetObject;
        [Tooltip("Target value to fade")]
        [SerializeField]
        protected float targetValue;
        [Tooltip("Fade duration")]
        [SerializeField]
        protected float duration;
        [Tooltip("Wait until finished")]
        [SerializeField]
        protected bool waitUntilFinished = false;
        protected CanvasGroup canvyG;

        public override void OnEnter()
        {
            if (_targetObject.Value != null)
            {
                canvyG = _targetObject.Value.GetComponent<CanvasGroup>();

                if(canvyG != null)
                {
                    float tmpVal = targetValue > 1f ? tmpVal = 1f : tmpVal = targetValue;

                    LeanTween.alphaCanvas(canvyG, tmpVal, duration)
                        .setOnComplete(()=>
                        {
                            Continue();
                        });
                }
            }

            if(!waitUntilFinished)
            Continue();
        }

        public override string GetSummary()
        {
            string canvyGerr = "";
            if (_targetObject.Value == null)
            {
                return "Error: No target object selected";
            }

            if(canvyG == null)
            {
                canvyG = _targetObject.Value.GetComponent<CanvasGroup>();
                if(canvyG == null)
                return canvyGerr = "No Canvas Group vailable";
            }

            return "Fade In/Out " + _targetObject.Value.name + " " + canvyGerr;
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _targetObject.gameObjectRef == variable;
        }
    }
}