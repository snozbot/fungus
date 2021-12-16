using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    public class BlockSaver : DataSaver<BlockSaveUnit, Block>
    {
        [SerializeField]
        [Tooltip("Transforms that have the components saving specific types of Commands")]
        public List<Transform> hasCommandSavers;

        protected List<CommandSaver> commandSavers = new List<CommandSaver>();

        public virtual void Awake()
        {
            RegisterCommandSavers();
        }

        protected virtual void RegisterCommandSavers()
        {
            hasCommandSavers.RemoveAll(obj => obj == null);
            foreach (var commandSaverContainer in hasCommandSavers)
            {
                var saversFound = commandSaverContainer.GetComponentsInChildren<CommandSaver>();
                commandSavers.AddRange(saversFound);
            }
        }

        public override ISaveUnit CreateSaveFrom(object input)
        {
            if (IsValid(input))
                return CreateSaveFrom(input as Block);
            else
                throw new System.ArgumentException("BlockSavers can only create saves from Blocks.");
        }

        protected override bool IsValid(object input)
        {
            return input is Block;
        }

        public override BlockSaveUnit CreateSaveFrom(Block block)
        {
            BlockSaveUnit blockSave = BlockSaveUnit.From(block);
            SaveCommandStatesFor(ref blockSave, block);

            return blockSave;
        }

        protected virtual void SaveCommandStatesFor(ref BlockSaveUnit saveUnit, Block block)
        {
            for (int i = 0; i < commandSavers.Count; i++)
            {
                // We have savers create command states as appropriate from the contents of the
                // passed Block
                var currentSaver = commandSavers[i];
                IList<ICommandSaveUnit> commandStates = currentSaver.CreateSavesFrom(block);
                saveUnit.RegisterCommandStates(commandStates);
            }
        }


        
    }
}