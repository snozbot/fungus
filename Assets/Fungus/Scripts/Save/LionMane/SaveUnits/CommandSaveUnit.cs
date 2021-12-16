using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus.LionManeSaveSys
{
    [System.Serializable]
    public struct CommandSaveUnit : ISaveUnit<CommandSaveUnit>
    {
        public CommandSaveUnit Contents => this;
        object ISaveUnit.Contents => this;

        public string TypeName => "Command";

        [SerializeField]
        int indexInBlock; // Index 0 = first block, index 3 = fourth block, etc

        [SerializeField]
        string commandType;
        // ^You never know if the dev changes the orders of Commands between game versions, so knowing the
        // type can help ensure that the right state is restored to the right Commands.
        // This may be used as a sort of fallback for when indexInBlock risks executing the wrong Command
        // on load.

        [SerializeField]
        List<StringPair> fieldStates;
        // ^Like the Menu Command's Hide If Visited field. These pairs will need the name as the key,
        // the value as the... well, value

    }
}