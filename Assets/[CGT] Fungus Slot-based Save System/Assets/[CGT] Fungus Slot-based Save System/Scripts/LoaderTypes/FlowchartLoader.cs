using UnityEngine;
using System.Collections.Generic;
using Fungus;
using BaseFungus = Fungus;

namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// Can currently only decode these variables for Flowcharts: Bool, Int, Float, and String.
    /// </summary>
    public class FlowchartLoader : SaveLoader<FlowchartData>
    {
        /// <summary>
        /// Loads the state the passed FlowchartData has into the appropriate Flowchart in the scene.
        /// </summary>
        public override bool Load(FlowchartData data)
        {
            string errorMessage;
            if (!CanLoadData(data, out errorMessage))
            {
                Debug.LogError(errorMessage);
                return false;
            }

            // Restore the flowchart's state
            GameObject fcGo =               GameObject.Find(data.FlowchartName);
            Flowchart flowchart =           fcGo.GetComponent<Flowchart>();

            PreventInterruptions(flowchart);
            LoadVariables(data, ref flowchart);
            
            // Before any blocks are re-executed, the ones responding to the load should execute first.
            if (ProgressMarker.latestExecuted != null)
                SaveDataLoaded.NotifyEventHandlers(ProgressMarker.latestExecuted.Key);
            LoadExecutingBlocks(data, flowchart);
            return true;

        }

        protected virtual bool CanLoadData(FlowchartData data, out string errorMessage)
        {
            errorMessage =                                  null;
            string objNotFound =                            "Failed to find Flowchart object specified in save data";

            // Find the Game Object in the scene
            GameObject fcGo =                               null;
            Flowchart fc =                                  null;
            fcGo =                                          GameObject.Find(data.FlowchartName);
            
            // If possible, get the Flowchart component from it
            if (fcGo != null)
                fc =                                        fcGo.GetComponent<Flowchart>();
            
            if (fc == null) // Need the flowchart component to load into
                errorMessage =                              objNotFound;
            
            return fc != null;
        }

        protected virtual void LoadVariables(FlowchartData data, ref Flowchart flowchart)
        {
            FlowchartVariables vars =                       data.Vars;

            LoadVariables<string, StringVar, StringVariable>(flowchart, vars.Strings);
            LoadVariables<int, IntVar, IntegerVariable>(flowchart, vars.Ints);
            LoadVariables<float, FloatVar, FloatVariable>(flowchart, vars.Floats);
            LoadVariables<bool, BoolVar, BooleanVariable>(flowchart, vars.Bools);
            LoadVariables<Color, ColorVar, ColorVariable>(flowchart, vars.Colors);
            LoadVariables<Vector2, Vec2Var, Vector2Variable>(flowchart, vars.Vec2s);
            LoadVariables<Vector3, Vec3Var, Vector3Variable>(flowchart, vars.Vec3s);

            /*

            for (int i = 0; i < vars.Bools.Count; i++)
            {
                var boolVar =               vars.Bools[i];
                flowchart.SetBooleanVariable(boolVar.Key, boolVar.Value);
            }

            for (int i = 0; i < vars.Ints.Count; i++)
            {
                var intVar =                vars.Ints[i];
                flowchart.SetIntegerVariable(intVar.Key, intVar.Value);
            }

            for (int i = 0; i < vars.Floats.Count; i++)
            {
                var floatVar =              vars.Floats[i];
                flowchart.SetFloatVariable(floatVar.Key, floatVar.Value);
            }

            for (int i = 0; i < vars.Strings.Count; i++)
            {
                var stringVar =             vars.Strings[i];
                flowchart.SetStringVariable(stringVar.Key, stringVar.Value);
            }

            for (int i = 0; i < vars.Colors.Count; i++)
            {
                var colorVar =              vars.Colors[i];
                flowchart.SetVariable<Color, ColorVariable>(colorVar.Key, colorVar.Value);
            }

            for (int i = 0; i < vars.Vector2s.Count; i++)
            {
                var vec2Var =               vars.Vector2s[i];
                flowchart.SetVariable<Vector2, Vector2Variable>(vec2Var.Key, vec2Var.Value);
            }

            for (int i = 0; i < vars.Vector3s.Count; i++)
            {
                var vec3Var =               vars.Vector3s[i];
                flowchart.SetVariable<Vector3, Vector3Variable>(vec3Var.Key, vec3Var.Value);
            }
            */

        }

        /// <summary>
        /// Loads variables into the passed flowchart based on the type arguments.
        /// TBase: Base type that the serializable var containers are for
        /// TSVarType: This save system's serializable variable container
        /// TNSVarType Fungus's built-in not-as-serializable variable container
        /// </summary>
        protected virtual void LoadVariables<TBase, TSVarType, TNSVarType>(Flowchart toLoadInto, 
                                                                            IList<TSVarType> toLoadFrom)
        where TSVarType: Var<TBase>
        where TNSVarType: BaseFungus.VariableBase<TBase>
        {
            for (int i = 0; i < toLoadFrom.Count; i++)
            {
                var variable =                  toLoadFrom[i];
                toLoadInto.SetVariable<TBase, TNSVarType>(variable.Key, variable.Value);
            }
        }

        /// <summary>
        /// Keeps blocks like those with a Game Started event from interfering with the load process.
        /// </summary>
        protected virtual void PreventInterruptions(Flowchart flowchart)
        {
            var blocks =                        flowchart.GetComponents<Block>();
            
            for (int i = 0; i < blocks.Length; i++)
            {
                var block =                     blocks[i];

                // Getting rid of the Game Started event
                var hasGameStartedHandler =     block._EventHandler as GameStarted != null;

                if (hasGameStartedHandler)
                {
                    block._EventHandler =       null;
                }
            }
        }

        /// <summary>
        /// Makes the blocks in the flowchart pick up where they left off, when the original 
        /// FlowchartData was made.
        /// </summary>
        protected virtual void LoadExecutingBlocks(FlowchartData data, Flowchart flowchart)
        {
            flowchart.StopAllBlocks();
            for (int i = 0; i < data.Blocks.Count; i++)
            {
                var savedBlock =                data.Blocks[i];
                if (!savedBlock.WasExecuting)
                    continue;
                
                var fullBlockObj =              flowchart.FindBlock(savedBlock.BlockName);

                if (fullBlockObj == null)
                {
                    // Seems the user removed the block. Might as well let them know.
                    var messageFormat = 
                    @"Could not load state of block named {0} from flowchart named {1}; 
                    the former is not in the latter.";
                    var message =               string.Format(messageFormat, savedBlock.BlockName, flowchart.name);
                    Debug.LogWarning(message);
                    continue;
                }

                flowchart.ExecuteBlock(fullBlockObj, savedBlock.CommandIndex);
            }
        }

    }

}