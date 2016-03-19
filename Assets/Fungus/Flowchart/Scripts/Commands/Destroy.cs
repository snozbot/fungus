using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Destroy", 
	             "Destroys a specified game object in the scene.")]
	[AddComponentMenu("")]
	public class Destroy : Command, ISerializationCallbackReceiver
	{	
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("targetGameObject")] public GameObject targetGameObjectOLD;
		#endregion

		[Tooltip("Reference to game object to destroy")]
		public GameObjectData _targetGameObject;

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

		//
		// ISerializationCallbackReceiver implementation
		//

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (targetGameObjectOLD != null)
			{
				_targetGameObject.Value = targetGameObjectOLD;
				targetGameObjectOLD = null;
			}
		}
	}

}