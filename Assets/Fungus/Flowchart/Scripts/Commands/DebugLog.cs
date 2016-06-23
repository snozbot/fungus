/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Debug Log", 
	             "Writes a log message to the debug console.")]
	[AddComponentMenu("")]
	public class DebugLog : Command 
	{
		public enum DebugLogType
		{
			Info,
			Warning,
			Error
		}

		[Tooltip("Display type of debug log info")]
		public DebugLogType logType;

		[Tooltip("Text to write to the debug log. Supports variable substitution, e.g. {$Myvar}")]
		public StringDataMulti logMessage;

		public override void OnEnter ()
		{
			Flowchart flowchart = GetFlowchart();
			string message = flowchart.SubstituteVariables(logMessage.Value);

			switch (logType)
			{
			case DebugLogType.Info:
				Debug.Log(message);
				break;
			case DebugLogType.Warning:
				Debug.LogWarning(message);
				break;
			case DebugLogType.Error:
				Debug.LogError(message);
				break;
			}

			Continue();
		}

		public override string GetSummary()
		{
			return logMessage.GetDescription();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}