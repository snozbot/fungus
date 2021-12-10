using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    public class SayCommandSaver : DataSaver<SaySaveUnit, Block>
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
            return input is Block;
        }

        public virtual SaySaveUnit CreateSaveFrom(Block block)
        {
            throw new System.NotImplementedException();
        }

    }
}