namespace Fungus
{
    /// <summary>
    /// Created Save Units for Fungus variables that hold primitive values (bools, numerics, strings)
    /// </summary>
    public class PrimitiveVarSaver : VarSaver
    {
        protected override ContentType SetContentAs => ContentType.regularString;
    }
}