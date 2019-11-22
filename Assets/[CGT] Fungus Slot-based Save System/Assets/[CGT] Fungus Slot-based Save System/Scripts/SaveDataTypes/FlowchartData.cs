using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

using BaseFungus = Fungus;

namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// Contains much of the state of a Flowchart.
    /// </summary>
    public class FlowchartData : SaveData
    {
        #region Fields
        [SerializeField] protected string flowchartName;
        [SerializeField] protected FlowchartVariables vars =            new FlowchartVariables();
        [SerializeField] protected List<BlockData> blocks =             new List<BlockData>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the encoded Flowchart.
        /// </summary>
        public string FlowchartName         { get { return flowchartName; } set { flowchartName = value; } }
        public FlowchartVariables Vars      { get { return vars; } set { vars = value; } }
        public List<BlockData> Blocks       { get { return blocks; } set { blocks = value; } }
        #endregion

        #region Constructors
        public FlowchartData()                                          { }

        public FlowchartData(Flowchart flowchart)
        {
            SetFrom(flowchart);
        }
        #endregion

        #region Public methods
        
        /// <summary>
        /// Makes the FlowchartData instance hold the state of only the passed Flowchart.
        /// </summary>
        public virtual void SetFrom(Flowchart flowchart)
        {
            Clear(); // Get rid of any old state data first

            FlowchartName =                     flowchart.name;
            SetVariablesFrom(flowchart);
            SetBlocksFrom(flowchart);
        }

        /// <summary>
        /// Clears all state this FlowchartData has.
        /// </summary>
        public override void Clear()
        {
            flowchartName =                     string.Empty;
            ClearVariables();
            ClearBlocks();
        }

        #region Static Methods
        public static FlowchartData CreateFrom(Flowchart flowchart)
        {
            return new FlowchartData(flowchart);
        }
        #endregion

        #region Helpers

        protected virtual void ClearVariables()
        {
            vars.Clear();
        }

        protected virtual void ClearBlocks()
        {
            blocks.Clear();
        }

        protected virtual void SetVariablesFrom(Flowchart flowchart)
        {
            for (int i = 0; i < flowchart.Variables.Count; i++) 
            {
                var variable =                  flowchart.Variables[i];

                TrySetVariable<string, StringVar, StringVariable>(variable, vars.Strings);
                TrySetVariable<int, IntVar, IntegerVariable>(variable, vars.Ints);
                TrySetVariable<float, FloatVar, FloatVariable>(variable, vars.Floats);
                TrySetVariable<bool, BoolVar, BooleanVariable>(variable, vars.Bools);
                TrySetVariable<Color, ColorVar, ColorVariable>(variable, vars.Colors);
                TrySetVariable<Vector2, Vec2Var, Vector2Variable>(variable, vars.Vec2s);
                TrySetVariable<Vector3, Vec3Var, Vector3Variable>(variable, vars.Vec3s);
            }
        
        }

        /// <summary>
        /// Adds the passed variable to the passed list if it can be cast to the correct type.
        /// 
        /// TBase: Base type encapsulated by the variable
        /// 
        /// TSVarType: This save system's serializable container for the variable
        /// TNSVariableType: Fungus's built-in flowchart-only container for the variable
        /// </summary>
        protected virtual void TrySetVariable<TBase, TSVarType, TNSVariableType>(BaseFungus.Variable varToSet, 
                                                                                IList<TSVarType> varList) 
        where TSVarType: Var<TBase>, new()
        where TNSVariableType: BaseFungus.VariableBase<TBase>
        {
            var fungusBaseVar =                    varToSet as TNSVariableType;

            if (fungusBaseVar != null)
            {
                var toAdd  =                       new TSVarType();
                toAdd.Key =                        fungusBaseVar.Key;
                toAdd.Value =                      fungusBaseVar.Value;
                varList.Add(toAdd);
            }
        }

        protected virtual void SetBlocksFrom(Flowchart flowchart)
        {
            // Register data for the blocks the flowchart is executing
            var executingBlocks =               flowchart.GetExecutingBlocks();
            for (int i = 0; i < executingBlocks.Count; i++)
            {
                BlockData newBlockData =        new BlockData(executingBlocks[i]);
                blocks.Add(newBlockData);
            }
        }

        #endregion

        #endregion
    }

}