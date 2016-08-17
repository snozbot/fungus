/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Scripting", 
                 "Set Active", 
                 "Sets a game object in the scene to be active / inactive.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetActive : Command
    {
        [Tooltip("Reference to game object to enable / disable")]
        public GameObjectData _targetGameObject;

        [Tooltip("Set to true to enable the game object")]
        public BooleanData activeState;
    
        public override void OnEnter()
        {
            if (_targetGameObject.Value != null)
            {
                _targetGameObject.Value.SetActive(activeState.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetGameObject.Value == null)
            {
                return "Error: No game object selected";
            }

            return _targetGameObject.Value.name + " = " + activeState.GetDescription();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("targetGameObject")] public GameObject targetGameObjectOLD;

        protected virtual void OnEnable()
        {
            if (targetGameObjectOLD != null)
            {
                _targetGameObject.Value = targetGameObjectOLD;
                targetGameObjectOLD = null;
            }
        }

        #endregion
    }

}