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
	[ExecuteInEditMode]
	public class Wait : Command
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

		protected virtual void OnEnable()
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