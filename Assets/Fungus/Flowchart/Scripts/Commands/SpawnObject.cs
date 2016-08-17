/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    // This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
    // a Send Message command for sending messages to trigger block execution.

    [CommandInfo("Scripting", 
                 "Spawn Object", 
                 "Spawns a new object based on a reference to a scene or prefab game object.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SpawnObject : Command
    {
        [Tooltip("Game object to copy when spawning. Can be a scene object or a prefab.")]
        public GameObjectData _sourceObject;

        [Tooltip("Transform to use for position of newly spawned object.")]
        public TransformData _parentTransform;

        [Tooltip("Local position of newly spawned object.")]
        public Vector3Data _spawnPosition;

        [Tooltip("Local rotation of newly spawned object.")]
        public Vector3Data _spawnRotation;

        public override void OnEnter()
        {
            if (_sourceObject.Value == null)
            {
                Continue();
                return;
            }

            GameObject newObject = GameObject.Instantiate(_sourceObject.Value);
            if (_parentTransform.Value != null)
            {
                newObject.transform.parent = _parentTransform.Value;
            }

            newObject.transform.localPosition = _spawnPosition.Value;
            newObject.transform.localRotation = Quaternion.Euler(_spawnRotation.Value);

            Continue();
        }

        public override string GetSummary()
        {
            if (_sourceObject.Value == null)
            {
                return "Error: No source GameObject specified";
            }

            return _sourceObject.Value.name;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("sourceObject")] public GameObject sourceObjectOLD;
        [HideInInspector] [FormerlySerializedAs("parentTransform")] public Transform parentTransformOLD;
        [HideInInspector] [FormerlySerializedAs("spawnPosition")] public Vector3 spawnPositionOLD;
        [HideInInspector] [FormerlySerializedAs("spawnRotation")] public Vector3 spawnRotationOLD;

        protected virtual void OnEnable()
        {
            if (sourceObjectOLD != null)
            {
                _sourceObject.Value = sourceObjectOLD;
                sourceObjectOLD = null;
            }
            if (parentTransformOLD != null)
            {
                _parentTransform.Value = parentTransformOLD;
                parentTransformOLD = null;
            }
            if (spawnPositionOLD != default(Vector3))
            {
                _spawnPosition.Value = spawnPositionOLD;
                spawnPositionOLD = default(Vector3);
            }
            if (spawnRotationOLD != default(Vector3))
            {
                _spawnRotation.Value = spawnRotationOLD;
                spawnRotationOLD = default(Vector3);
            }
        }

        #endregion
    }

}