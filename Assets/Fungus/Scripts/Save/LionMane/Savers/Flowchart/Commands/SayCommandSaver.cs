using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    public class SayCommandSaver : DataSaver, ICommandSaver
    {
        public override ISaveUnit CreateSaveFrom(object input)
        {
            if (IsValid(input))
                return CreateSaveFrom(input as Block);
            else
                throw new System.ArgumentException("");
        }

        protected override bool IsValid(object input)
        {
            return input is Say;
        }
        public virtual SaySaveUnit CreateSaveFrom(Say command)
        {
            SaySaveUnit newUnit = SaySaveUnit.From(command);
            return newUnit;
        }

        public virtual IList<ICommandSaveUnit> CreateSavesFrom(Block block)
        {
            IList<Say> sayCommands = block.GetCommandsOfType<Say>();
            List<object> commandsToPass = new List<object>();
            commandsToPass.AddRange(sayCommands);
            IList<ICommandSaveUnit> results = (IList<ICommandSaveUnit>) CreateSavesFrom(commandsToPass);
            return results;
        }
    }
}