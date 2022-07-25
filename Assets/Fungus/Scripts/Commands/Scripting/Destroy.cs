// This code is part of the Fungus library (https://github.com/snozbot/fungus)
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

        [Tooltip("Optional delay given to destroy")]
        [SerializeField]
        protected FloatData destroyInXSeconds = new FloatData(0);

        #region Public members

        public override void OnEnter()
        {
            if (_targetGameObject.Value != null)
            {
                if (destroyInXSeconds.Value != 0)
                    Destroy(_targetGameObject, destroyInXSeconds.Value);
                else
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

            return _targetGameObject.Value.name + (destroyInXSeconds.Value == 0 ? "" : " in " + destroyInXSeconds.Value.ToString());
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (_targetGameObject.gameObjectRef == variable || destroyInXSeconds.floatRef == variable)
                return true;

            return false;
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