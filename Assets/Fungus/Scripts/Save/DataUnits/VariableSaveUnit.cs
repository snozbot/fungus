namespace Fungus
{
    /// <summary>
    /// Base class for save units that contain the state of Fungus Flowchart variables.
    /// </summary>
    public class VariableSaveUnit : SaveUnit<VariableInfo> 
    {
        public VariableSaveUnit()
        {
            contents = new VariableInfo();
        }

        public VariableSaveUnit(Variable toSetFrom) : this()
        {
            SetFrom(toSetFrom);
        }

        public virtual void SetFrom(Variable variable)
        {
            contents.Name = variable.Key;
            contents.Type = variable.GetType();
            contents.Value = variable.GetValue().ToString();
        }

    }

}