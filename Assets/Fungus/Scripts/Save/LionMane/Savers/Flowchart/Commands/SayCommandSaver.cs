using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    public class SayCommandSaver : CommandSaver, ICommandSaver
    {
        public override ISaveUnit CreateSaveFrom(object input)
        {
            if (IsValid(input))
                return CreateSaveFrom(input as Say);
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

        public override IList<ICommandSaveUnit> CreateSavesFrom(Block block)
        {
            IList<Say> sayCommands = block.GetCommandsOfType<Say>();
            List<object> commandsToPass = new List<object>();
            commandsToPass.AddRange(sayCommands);
            IList<ISaveUnit> results = CreateSavesFrom(commandsToPass);

            // We can't simply cast the above list for ICommandSaveUnits, so we have to do it the long way
            List<ICommandSaveUnit> resultsToReturn = new List<ICommandSaveUnit>();
            foreach (var resultEl in results)
            {
                if (resultEl is ICommandSaveUnit)
                    resultsToReturn.Add(resultEl as ICommandSaveUnit);
            }

            return resultsToReturn;
        }

    }
}