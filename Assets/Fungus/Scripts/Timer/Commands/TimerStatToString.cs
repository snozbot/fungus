using UnityEngine;
using System.Collections.Generic;

namespace Fungus.TimeSys
{
	[CommandInfo("Timer", "Timer Stat to String", "Puts the value of a timer stat into a String Variable with the specified min digit count")]
	public class TimerStatToString : TimerCommand
	{
		[SerializeField]
		TimeMeasurement stat;

		[Tooltip("The minimum amount of digits the output should have. Said output will be padded with 0s at the front as appropriate.")]
		[SerializeField]
		IntegerData minDigitCount = new IntegerData(2);

		[Tooltip("Where the converted string value will go.")]
		[SerializeField]
		[VariableProperty(typeof(StringVariable))]
		StringVariable output;

        public override void OnEnter()
        {
			bool nothingToOutputTo = output == null;

			if (nothingToOutputTo)
            {
				LetUserKnowThisCantDoItsThing();
				Continue();
				return;
            }

            base.OnEnter();
			
			UpdateStatDict();
			GenerateOutput();

			Continue();
        }

		protected virtual void LetUserKnowThisCantDoItsThing()
        {
			string messageFormat = "TimerStatsToString Command in Flowchart {0}, Block {1} has no output StringVariable to work with.";
			string message = string.Format(messageFormat, this.GetFlowchart().name, this.ParentBlock.BlockName);
			Debug.LogWarning(message);
        }

		protected virtual void UpdateStatDict()
        {
			// So we won't need to work with any ugly switch statements
			statDict[TimeMeasurement.Milliseconds] = timeRecorded.Milliseconds;
			statDict[TimeMeasurement.Seconds] = timeRecorded.Seconds;
			statDict[TimeMeasurement.Minutes] = timeRecorded.Minutes;
			statDict[TimeMeasurement.Hours] = timeRecorded.Hours;
			statDict[TimeMeasurement.Days] = timeRecorded.Days;
        }

		protected Dictionary<TimeMeasurement, int> statDict = new Dictionary<TimeMeasurement, int>();
		
		protected virtual void GenerateOutput()
        {
			int statValue = statDict[stat];
			string ensuresMinimumDigitCount = "D" + minDigitCount.Value;
			string resultValue = statValue.ToString(ensuresMinimumDigitCount);
			output.Value = resultValue;
        }

        public override string GetSummary()
        {
			string summary = "";

			if (TimerInputIsSet)
			{
				string timerName = timer.Key;
				string outputVarName = GetOutputVarName();
				summary = string.Format(summaryFormat, timerName, stat, minDigitCount.Value, outputVarName);
			}

			return summary;
        }

		protected virtual string GetOutputVarName()
        {
			// Helper for GetSummary
			string result = "<noOutputAssigned>";
			bool outputVarAvailable = output != null;

			if (TimerInputIsSet && outputVarAvailable)
			{
				string variableName = output.Key;
				result = variableName;
			}

			return result;
		}

		protected static string summaryFormat = "{0}, {1}, {2} digits, {3}";
    }
}