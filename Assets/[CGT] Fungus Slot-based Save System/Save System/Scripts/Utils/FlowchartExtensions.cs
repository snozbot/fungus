using BaseFungus = Fungus;

//todo remove

namespace Fungus.SaveSystem
{
    public static class FlowchartExtensions
    {
        /// <summary>
        /// Sets the value of a variable in the flowchart (if it exists) to the value that was passed.
        /// </summary>
        public static void SetVariable<TBase, TVarType>(this Flowchart flowchart, string key, TBase value)
        where TVarType : BaseFungus.VariableBase<TBase>

        {
            var variable = flowchart.GetVariable<TVarType>(key);
            if (variable != null)
            {
                variable.Value = value;
            }
        }
    }
}