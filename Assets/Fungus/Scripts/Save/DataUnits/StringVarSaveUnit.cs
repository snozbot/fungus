namespace Fungus
{
    public class StringVarSaveUnit : VariableSaveUnit<StringVariable, string>
    {
        public override void SetFrom(StringVariable variable)
        {
            contents = variable.Value;
        }
    }
}