using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeSpan = System.TimeSpan;
using System.Text;

namespace Fungus.PlaytimeSys
{
    [CommandInfo("Playtime", "Get Playtime Stats", "Lets you store the current playtime stats into Fungus Variables")]
    public class GetPlaytimeStats : PlaytimeCommand
    {
        [SerializeField]
        [VariableProperty(typeof(IntegerVariable))]
        protected IntegerVariable milliseconds, seconds, minutes, hours, days;

        [SerializeField]
        [VariableProperty(typeof(StringVariable))]
        protected StringVariable allAsString;

        [SerializeField]
        [Tooltip("Set this no higher than 7")]
        protected IntegerData millisecondDecimalPlaces = new IntegerData(2);

        protected TimeSpanFormats stringFormat = TimeSpanFormats.upToMinutes;

        public enum TimeSpanFormats
        {
            upToSeconds,
            upToMinutes,
            upToHours,
            upToDays,
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ApplyStatsToVars();

            Continue();
        }

        protected virtual void ApplyStatsToVars()
        {
            TimeSpan playtime = tracker.PlaytimeRecorded;

            ApplyNumericStatsFrom(ref playtime);
            ApplyStringsFrom(ref playtime);
        }

        protected virtual void ApplyNumericStatsFrom(ref TimeSpan playtime)
        {
            if (milliseconds != null)
                milliseconds.Value = playtime.Milliseconds;
            if (seconds != null)
                seconds.Value = playtime.Seconds;
            if (minutes != null)
                minutes.Value = playtime.Minutes;
            if (hours != null)
                hours.Value = playtime.Hours;
            if (days != null)
                days.Value = playtime.Days;
        }

        protected virtual void ApplyStringsFrom(ref TimeSpan playtime)
        {
            if (allAsString != null)
            {
                allAsString.Value = playtime.ToString();
            }
        }

        protected virtual string TimeSpanUpToSecondsFrom(ref TimeSpan playtime)
        {
            string milliseconds = GetMillisecondStringFrom(ref playtime);


            throw new System.NotImplementedException();
        }



        protected virtual string GetMillisecondStringFrom(ref TimeSpan playtime)
        {
            int milliseconds = playtime.Milliseconds;
            string rawString = milliseconds.ToString();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < millisecondDecimalPlaces; i++)
            {
                char decimalToAdd = rawString[0];
                result.Append(decimalToAdd);
            }

            return result.ToString();
        }
    }
}