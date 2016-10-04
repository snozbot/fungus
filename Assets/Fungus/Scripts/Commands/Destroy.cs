// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Destroys a specified game object in the scene.
    /// </summary>
    [CommandInfo("Scripting", 
                 "Destroy", 
                 "Destroys a specified game object in the scene.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Destroy : Command
    {   
        [Tooltip("Reference to game object to destroy")]
        [SerializeField] protected GameObjectData _targetGameObject;

        #region Public members

        public override void OnEnter()
        {
            if (_targetGameObject.Value != null)
            {
                Destroy(_targetGameObject.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (_targetGameObject.Value == null)
            {
                return "Error: No game object selected";
            }

            return _targetGameObject.Value.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
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