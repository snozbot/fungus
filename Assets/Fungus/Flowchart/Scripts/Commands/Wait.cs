using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Wait", 
	             "Waits for period of time before executing the next command in the block.")]
	[AddComponentMenu("")]
	public class Wait : Command, ISerializationCallbackReceiver 
	{
		[Tooltip("Duration to wait for")]
		public FloatData _duration = new FloatData(1);

		public override void OnEnter()
		{
			Invoke ("OnWaitComplete", _duration.Value);
		}

		void OnWaitComplete()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return _duration.Value.ToString() + " seconds";
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (durationOLD != default(float))
			{
				_duration.Value = durationOLD;
				durationOLD = default(float);
			}
		}

		#endregion
	}

}