using UnityEngine;
using System.Collections.Generic;

namespace Fungus.LionManeSaveSys
{
    [System.Serializable]
    public struct SaySaveUnit : ISaveUnit<SaySaveUnit>, System.IEquatable<SaySaveUnit>
    {
        public SaySaveUnit Contents => this;
        object ISaveUnit.Contents => this;

        public int ExecutionCount
        {
            get { return executionCount; }
            set { executionCount = value; }
        }

        [SerializeField]
        int executionCount;

        public bool Equals(SaySaveUnit other)
        {
            return this.ExecutionCount == other.ExecutionCount;
        }

        public static SaySaveUnit From(Say command)
        {
            SaySaveUnit newUnit = new SaySaveUnit();
            newUnit.executionCount = command.ExecutionCount;
            return newUnit;
        }

        public static IList<SaySaveUnit> From(IList<Say> commands)
        {
            SaySaveUnit[] results = new SaySaveUnit[commands.Count];

            for (int i = 0; i < commands.Count; i++)
            {
                Say currentSay = commands[i];
                SaySaveUnit newUnit = From(currentSay);
                results[i] = newUnit;
            }

            return results;
        }
    }
}