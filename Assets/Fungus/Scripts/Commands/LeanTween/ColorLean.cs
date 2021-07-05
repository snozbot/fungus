// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Changes a game object's scale to a specified value over time.
    /// </summary>
    [CommandInfo("LeanTween",
                 "UI Image Color",
                 "Changes UI's Image color property to a specified value over time. Can be useful to Fade an Image as well")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ColorLean : Command
    {
        [Tooltip("Red value")]
        [SerializeField]
        protected GameObjectData _targetObject;
        [Tooltip("Red value")]
        [SerializeField]
        protected ColorData toColor;
        [Tooltip("Duration")]
        [SerializeField]
        protected FloatData _duration;
        [Tooltip("The shape of the easing curve applied to the animation")]
        [SerializeField]
        protected LeanTweenType easeType = LeanTweenType.easeInOutQuad;
        [Tooltip("The type of loop to apply once the animation has completed")]
        [SerializeField]
        protected LeanTweenType loopType = LeanTweenType.once;
        [Tooltip("Wait Until Finished")]
        [SerializeField]
        protected bool waitUntilFinished;
        public override void OnEnter()
        {
            if(_targetObject.Value != null)
            {
                var go = _targetObject.Value.GetComponent<Image>();
                if(go != null)
                {
                    LeanTween.value(_targetObject, go.color, toColor, _duration.Value)
                        .setOnComplete(()=>
                        {
                            if(waitUntilFinished)
                            Continue();
                        }).setOnUpdate( (Color val)=>{
                            go.color = val;
                        }).setLoopType(loopType).setEase(easeType);
                }
                else
                {
                    Debug.LogWarning("No Image Component Detected");
                }
            }

            if(!waitUntilFinished)
            Continue();
        }
        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "Error: No game object selected";
            }

            return _targetObject.Value.name + " = ";
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }
        public override bool HasReference(Variable variable)
        {
            return variable == _duration.floatRef || _targetObject.gameObjectRef || toColor.colorRef;
        }
    }
}