using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Debug Log", 
	             "Writes a log message to the debug console.")]
	public class DebugLog : Command 
	{
		public enum DebugLogType
		{
			Info,
			Warning,
			Error
		}

		public DebugLogType logType;

		public StringData logMessage;

		public override void OnEnter ()
		{
			switch (logType)
			{
			case DebugLogType.Info:
				Debug.Log(logMessage.Value);
				break;
			case DebugLogType.Warning:
				Debug.LogWarning(logMessage.Value);
				break;
			case DebugLogType.Error:
				Debug.LogError(logMessage.Value);
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