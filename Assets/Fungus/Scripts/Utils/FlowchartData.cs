// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Serializable container for encoding the state of a Flowchart's variables.
    /// </summary>
    [System.Serializable]
    public class FlowchartData
    {
        [System.Serializable]
        public class StringToJsonPair 
        {
            public string key, json;
        }

        [System.Serializable]
        public class BlockData
        {
            public string blockName = string.Empty;
            public int commandIndex = -1;
            public ExecutionState executionState = ExecutionState.Idle;
        }

        [SerializeField] protected string flowchartName;
        [SerializeField] protected List<StringToJsonPair> varPairs = new List<StringToJsonPair>();
        [SerializeField] protected List<BlockData> blockDatas = new List<BlockData>();

        #region Public methods

        /// <summary>
        /// Gets or sets the name of the encoded Flowchart.
        /// </summary>
        public string FlowchartName { get { return flowchartName; } set { flowchartName = value; } }

        /// <summary>
        /// Encodes the data in a Flowchart into a structure that can be stored by the save system.
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
                    flowchartData.varPairs.Add(new StringToJsonPair()
                    {
                        key = v.Key,
                        json = v.GetValueAsJson()
                    });
                }
            }

            var blocks = flowchart.GetComponents<Block>();
            foreach(var block in blocks)
            {
                if (block.IsSavingAllowed)
                {
                    flowchartData.blockDatas.Add(new BlockData()
                    {
                        blockName = block.BlockName,
                        commandIndex = block.ActiveCommandIndex,
                        executionState = block.State
                    });
                }
            }

            return flowchartData;
        }

        /// <summary>
        /// Decodes a FlowchartData object and uses it to restore the state of a Flowchart in the scene.
        /// </summary>
        public static void Decode(FlowchartData flowchartData, Flowchart flowchart = null)
        {
            if (flowchart == null)
            {
                var go = GameObject.Find(flowchartData.FlowchartName);
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

            for (int i = 0; i < flowchartData.varPairs.Count; i++)
            {
                var v = flowchart.GetVariable(flowchartData.varPairs[i].key);

                if(v != null)
                {
                    v.SetValueFromJson(flowchartData.varPairs[i].json);
                }
            }

            foreach (var item in flowchartData.blockDatas)
            {
                var block = flowchart.FindBlock(item.blockName);

                if(block != null)
                {
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

                        //TODO this should be cached until all loading is complete
                        if (!block.IsExecuting())
                        {
                            block.Stop();
                            flowchart.ExecuteBlock(block, item.commandIndex);
                        }
                    }
                }
            }
        }

        #endregion
    }
}