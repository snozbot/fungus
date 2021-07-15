// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Sets a game object in the scene to be active / inactive.
    /// </summary>
    [CommandInfo("Scripting", 
                 "Set Active", 
                 "Sets a game object in the scene to be active / inactive.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetActive : Command
    {
        [Tooltip("Reference to game object to enable / disable")]
        [SerializeField] protected GameObjectData _targetGameObject;

        [Tooltip("Set to true to enable the game object")]
        [SerializeField] protected BooleanData activeState;
    
        #region Public members

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

        public override bool HasReference(Variable variable)
        {
            return _targetGameObject.gameObjectRef == variable || activeState.booleanRef == variable || 
                base.HasReference(variable);
        }

        #endregion

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