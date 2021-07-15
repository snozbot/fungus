// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PauseResume
{
    None,
    Pause,
    Resume,
    PauseAll,
    ResumeAll
}
namespace Fungus
{
    /// <summary>
    /// 
    /// </summary> 
    [CommandInfo("LeanTween",
                 "Pause Resume",
                 "Pause or Resume LeanTween on target gameObject")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class PauseResumeLeanTween : Command
    {
        [Tooltip("Set the state of target gameObject")]
        [SerializeField]
        protected PauseResume setTweenState;
        [Tooltip("Target game object to Pause or Resume LeanTweens on")]
        [SerializeField]
        protected GameObjectData _targetObject;

        public override void OnEnter()
        {
            if (_targetObject.Value != null)
            {
                switch(setTweenState)
                {
                    case PauseResume.Pause:
                    LeanTween.pause(_targetObject.Value);
                    break;
                    case PauseResume.Resume:
                    LeanTween.resume(_targetObject.Value);
                    break;
                    case PauseResume.PauseAll:
                    LeanTween.pauseAll();
                    break;
                    case PauseResume.ResumeAll:
                    LeanTween.resumeAll();
                    break;
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "Error: No target object selected";
            }

            return "Pause or Resume LeanTweens on " + _targetObject.Value.name;
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