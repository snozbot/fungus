using UnityEngine;
using System.Collections.Generic;

namespace Fungus.LionManeSaveSys
{
    [System.Serializable]
    public class SaySaveUnit : SaveUnit, ICommandSaveUnit, System.IEquatable<SaySaveUnit>
    {

        public int ExecutionCount
        {
            get { return executionCount; }
            set { executionCount = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        [SerializeField]
        int index;

        public bool WasExecuting
        {
            get { return wasExecuting; }
            set { wasExecuting = value; }
        }

        public override string TypeName { get; set; } = "SayCommand";

        [SerializeField]
        bool wasExecuting;

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
            newUnit.index = command.CommandIndex;
            newUnit.wasExecuting = command.IsExecuting;

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