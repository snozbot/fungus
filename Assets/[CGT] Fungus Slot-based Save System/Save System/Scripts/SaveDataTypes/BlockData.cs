using UnityEngine;
using Fungus;


namespace Fungus.SaveSystem
{
    /// <summary>
    /// Save data for the Blocks in a Flowchart.
    /// </summary>
    [System.Serializable]
    public class BlockData : SaveData
    {
        #region Fields
        [SerializeField] string blockName;
        [SerializeField] int commandIndex =             -1;
        #endregion

        #region Properties

        public virtual string BlockName
        {
            get                                         { return blockName; }
            set                                         { blockName = value; }
        }

        /// <summary>
        /// Index of the command this was running when this data was made. A negative value means
        /// it wasn't running any commands.
        /// </summary>
        public virtual int CommandIndex
        {
            get                                         { return commandIndex; }
            set                                         { commandIndex = value; }
        }

        /// <summary>
        /// Whether or not this was executing any commands according to this data.
        /// </summary>
        /// <value></value>
        public virtual bool WasExecuting                { get { return commandIndex >= 0; } }
        #endregion

        #region Methods

        #region Constructors
        public BlockData() {}

        public BlockData(Block block)
        {
            SetFrom(block);
        }


        public BlockData(string blockName, int commandIndex = -1)
        {
            this.blockName =                            blockName;
            this.commandIndex =                         commandIndex;
        }

        #endregion

        public virtual void SetFrom(Block block)
        {
            blockName =                                 block.BlockName;
            var flowchart =                             block.GetFlowchart();
            if (block.ActiveCommand != null)
                commandIndex =                          block.CommandList.IndexOf(block.ActiveCommand);
        }

        #endregion
    }
}