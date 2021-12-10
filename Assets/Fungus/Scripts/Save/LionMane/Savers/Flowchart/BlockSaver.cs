using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    public class BlockSaver : DataSaver
    {
        public override ISaveUnit CreateSaveFrom(object input)
        {
            throw new System.NotImplementedException();
        }

        public virtual BlockSaveUnit CreateSaveFrom(Block block)
        {
            BlockSaveUnit blockSave = BlockSaveUnit.From(block);
            SaveExecutingCommandStatesFor(ref blockSave, block);

            throw new System.NotImplementedException();
        }

        protected virtual void SaveExecutingCommandStatesFor(ref BlockSaveUnit blockSave, Block block)
        {
            IList<StringPair> sayStates = GetSayCommandSaveStatesFrom(block);
        }

        protected virtual IList<StringPair> GetSayCommandSaveStatesFrom(Block block)
        {
            throw new System.NotImplementedException();
        }

        protected override bool IsValid(object input)
        {
            throw new System.NotImplementedException();
        }
    }
}