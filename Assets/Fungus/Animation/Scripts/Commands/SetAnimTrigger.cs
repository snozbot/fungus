using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Animation", 
	             "Set Anim Trigger", 
	             "Sets a trigger parameter on an Animator component to control a Unity animation")]
	[AddComponentMenu("")]
	public class SetAnimTrigger : Command, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("animator")] public Animator animatorOLD;
		[HideInInspector] [FormerlySerializedAs("parameterName")] public string parameterNameOLD;
		#endregion

		[Tooltip("Reference to an Animator component in a game object")]
		public AnimatorData _animator;

		[Tooltip("Name of the trigger Animator parameter that will have its value changed")]
		public StringData _parameterName;

		public override void OnEnter()
		{
			if (_animator.Value != null)
			{
				_animator.Value.SetTrigger(_parameterName.Value);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (_animator.Value == null)
			{
				return "Error: No animator selected";
			}

			return _animator.Value.name + " (" + _parameterName.Value + ")";
		}

		public override Color GetButtonColor()
		{
			return new Color32(170, 204, 169, 255);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public void OnBeforeSerialize()
		{}

		public void OnAfterDeserialize()
		{
			if (animatorOLD != null)
			{
				_animator.Value = animatorOLD;
				animatorOLD = null;
			}

			if (parameterNameOLD != null)
			{
				_parameterName.Value = parameterNameOLD;
				parameterNameOLD = null;
			}
		}
	}

}