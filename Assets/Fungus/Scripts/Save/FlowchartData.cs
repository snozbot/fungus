// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Serializable container for encoding the state of a Flowchart's blocks status & variables.
    /// </summary>
    [System.Serializable]
    public class FlowchartData
    {
        /// <summary>
        /// Variable name and json value pair.
        /// </summary>
        [System.Serializable]
        public class StringPair
        {
            public string key, val;
        }

        /// <summary>
        /// Container for an individual blocks state during serialisation.
        /// </summary>
        [System.Serializable]
        public class BlockData
        {
            public BlockData(string name, int index, ExecutionState state, int executionCount, int previousActiveCommandIndex, int jumpToCommandIndex)
            {
                this.blockName = name;
                this.commandIndex = index;
                this.executionState = state;
                this.executionCount = executionCount;
                this.previousActiveCommandIndex = previousActiveCommandIndex;
                this.jumpToCommandIndex = jumpToCommandIndex;
            }

            public string blockName = string.Empty;
            public int commandIndex = -1;
            public ExecutionState executionState = ExecutionState.Idle;
            public int executionCount, previousActiveCommandIndex = -1, jumpToCommandIndex = -1;
        }

        [SerializeField] protected string flowchartName;
        [SerializeField] protected List<StringPair> varPairs = new List<StringPair>();
        [SerializeField] protected List<StringPair> visitorPairs = new List<StringPair>();
        [SerializeField] protected List<BlockData> blockDatas = new List<BlockData>();

        /// <summary>
        /// Information bundle for blocks that need to be executed.
        /// </summary>
        public class CachedBlockExecution
        {
            public Block block;
            public int commandIndex = -1;
        }

        [System.NonSerialized] protected List<CachedBlockExecution> cachedBlockExecutions = new List<CachedBlockExecution>();
        public List<CachedBlockExecution> CachedBlockExecutions { get { return cachedBlockExecutions; } }

        /// <summary>
        /// Gets or sets the name of the encoded Flowchart.
        /// </summary>
        public string FlowchartName { get { return flowchartName; } set { flowchartName = value; } }

        public void AddBlockData(BlockData bd)
        {
            blockDatas.Add(bd);
        }

        /// <summary>
        /// Encodes the data in a Flowchart into a structure that can be stored by the save system.
        ///
        /// Includes all variables that are IsSerialisable, regardless of access level and all blocks
        /// unless they report that they do not want to be serialised via IsSavingAllowed.
        /// </summary>
        public static FlowchartData Encode(Flowchart flowchart)
        {
            var flowchartData = new FlowchartData();

            flowchartData.FlowchartName = flowchart.name;

            for (int i = 0; i < flowchart.Variables.Count; i++)
            {
                var v = flowchart.Variables[i];

                if (v.IsSerialisable)
                {
                    flowchartData.varPairs.Add(new StringPair()
                    {
                        key = v.Key,
                        val = v.GetStringifiedValue()
                    });
                }
            }

            var blocks = flowchart.GetComponents<Block>();
            foreach (var block in blocks)
            {
                if (block.IsSavingAllowed)
                {
                    flowchartData.blockDatas.Add(new BlockData(block.BlockName,
                        block.ActiveCommandIndex,
                        block.State,
                        block.GetExecutionCount(),
                        block.PreviousActiveCommandIndex,
                        block.JumpToCommandIndex));

                    var cmds = block.CommandList;
                    foreach (var item in cmds)
                    {
                        item.VisitEncode(flowchartData);
                    }

                    if (block._EventHandler != null)
                    {
                        block._EventHandler.VisitEncode(flowchartData);
                    }
                }
            }

            return flowchartData;
        }

        /// <summary>
        /// Decodes a FlowchartData object and uses it to restore the state of a Flowchart in the scene.
        /// </summary>
        /// <param name="flowchart">if null finds flowcharts by saved name, if provided uses provided</param>
        /// <param name="cacheExecutions"> if true, adds them to the cachedExecution list rather than executing them as
        /// it moves through the data. Primarily useful for synchronoisation of blocks.</param>
        public void Decode(Flowchart flowchart = null, bool cacheExecutions = false)
        {
            if (flowchart == null)
            {
                var go = GameObject.Find(FlowchartName);
                if (go == null)
                {
                    Debug.LogError("Failed to find flowchart object specified in save data");
                    return;
                }

                flowchart = go.GetComponent<Flowchart>();
                if (flowchart == null)
                {
                    Debug.LogError("Failed to find flowchart object specified in save data");
                    return;
                }
            }

            //restore variable values
            for (int i = 0; i < varPairs.Count; i++)
            {
                var v = flowchart.GetVariable(varPairs[i].key);

                if (v != null)
                {
                    v.RestoreFromStringifiedValue(varPairs[i].val);
                }
            }

            //stop blocks if they are running and shouldn't be, cache or execute blocks at commands that they should be at
            foreach (var item in blockDatas)
            {
                var block = flowchart.FindBlock(item.blockName);

                if (block != null)
                {
                    block.SetExecutionCount(item.executionCount);
                    block.SetPreviousActiveCommandIndex(item.previousActiveCommandIndex);
                    block.SetJumpToCommandIndex(item.jumpToCommandIndex);

                    var cmds = block.CommandList;
                    foreach (var cmd in cmds)
                    {
                        cmd.VisitDecode(this);
                    }

                    if (block._EventHandler != null)
                    {
                        block._EventHandler.VisitDecode(this);
                    }

                    if (item.executionState == ExecutionState.Idle && block.State != ExecutionState.Idle)
                    {
                        //meant to be idle but isn't
                        block.Stop();
                    }
                    else if (item.executionState == ExecutionState.Executing)
                    {
                        //running the wrong thing
                        if (block.State != ExecutionState.Idle && block.ActiveCommandIndex != item.commandIndex)
                            block.Stop();

                        if (!block.IsExecuting())
                        {
                            //caching gives blocks that access others a better chance of running correctly
                            // but its use is up to the caller
                            if (cacheExecutions)
                            {
                                cachedBlockExecutions.Add(new CachedBlockExecution()
                                { block = block, commandIndex = item.commandIndex, });
                            }
                            else
                            {
                                flowchart.ExecuteBlock(block, item.commandIndex);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Simple loop through and run all cached blocks, typically called after all decodes are complete.
        /// </summary>
        /// <param name="cached"></param>
        public static void ProcessCachedExecutions(List<CachedBlockExecution> cached)
        {
            foreach (var item in cached)
            {
                item.block.GetFlowchart().ExecuteBlock(item.block, item.commandIndex);
            }
        }

        public virtual void AddToVisitorPairs(string key, string value)
        {
            visitorPairs.Add(new StringPair() { key = key, val = value });
        }

        public virtual bool TryGetVisitorValueByKey(string key, out string value)
        {
            var item = visitorPairs.Find(x => x.key == key);
            if (item != null)
            {
                value = item.val;
                return true;
            }
            value = string.Empty;
            return false;
        }
    }
}