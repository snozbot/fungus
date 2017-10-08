// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Spawns a new object based on a reference to a scene or prefab game object.
    /// </summary>
    [CommandInfo("Scripting", 
                 "Spawn Object", 
                 "Spawns a new object based on a reference to a scene or prefab game object.", 
        Priority = 10)]
    [CommandInfo("GameObject",
                 "Instantiate",
                 "Instantiate a game object")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SpawnObject : Command
    {
        [Tooltip("Game object to copy when spawning. Can be a scene object or a prefab.")]
        [SerializeField] protected GameObjectData _sourceObject;

        [Tooltip("Transform to use as parent during instantiate.")]
        [SerializeField] protected TransformData _parentTransform;

        [Tooltip("If true, will use the Transfrom of this Flowchart for the position and rotation.")]
        [SerializeField] protected BooleanData _spawnAtSelf = new BooleanData(false);

        [Tooltip("Local position of newly spawned object.")]
        [SerializeField] protected Vector3Data _spawnPosition;

        [Tooltip("Local rotation of newly spawned object.")]
        [SerializeField] protected Vector3Data _spawnRotation;



        [Tooltip("Optional variable to store the GameObject that was just created.")]
        [SerializeField]
        protected GameObjectData _newlySpawnedObject;

        #region Public members

        public override void OnEnter()
        {
            if (_sourceObject.Value == null)
            {
                Continue();
                return;
            }

            GameObject newObject = null;

            if (_parentTransform.Value != null)
            {
                newObject = GameObject.Instantiate(_sourceObject.Value,_parentTransform.Value);
            }
            else
            {
                newObject = GameObject.Instantiate(_sourceObject.Value);
            }

            if (!_spawnAtSelf.Value)
            {
                newObject.transform.localPosition = _spawnPosition.Value;
                newObject.transform.localRotation = Quaternion.Euler(_spawnRotation.Value);
            }
            else
            {
                newObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            _newlySpawnedObject.Value = newObject;

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

        public override bool HasReference(Variable variable)
        {
            if (_sourceObject.gameObjectRef == variable || _parentTransform.transformRef == variable ||
                _spawnAtSelf.booleanRef == variable || _spawnPosition.vector3Ref == variable ||
                _spawnRotation.vector3Ref == variable)
                return true;

            return false;
        }

        #endregion

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