using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/**
	 * Legacy support for Run Script command
	 */
	[CommandInfo("Scripting", 
	             "Run Script", 
	             "Obsolete: use Run Flowchart instead")]
	public class RunScript : RunFlowchart
	{}

	[CommandInfo("Scripting", 
	             "Run Flowchart", 
	             "Start executing another Flowchart.")]
	[AddComponentMenu("")]
	public class RunFlowchart : Command
	{	
		[FormerlySerializedAs("targetScript")]
		[Tooltip("Reference to another Flowchart to execute")]
		public Flowchart targetFlowchart;

		[FormerlySerializedAs("targetSequence")]
		[Tooltip("Name of block to execute in target Flowchart")]
		public string targetBlockName;

		[FormerlySerializedAs("stopCurrentSequence")]
		[Tooltip("Stop executing the current block before executing the new Flowchart")]
		public bool stopCurrentBlock = true;
	
		public override void OnEnter()
		{
			if (targetFlowchart != null)
			{
				if (stopCurrentBlock)
				{
					Stop();
				}

				targetFlowchart.ExecuteBlock(targetBlockName);

				if (!stopCurrentBlock)
				{
					Continue();
				}
			}
			else
			{		
				Continue();
			}
		}

		public override string GetSummary()
		{
			if (targetFlowchart == null)
			{
				return "<Continue>";
			}

			return targetFlowchart.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}