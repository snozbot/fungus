namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// Creates Save Units for Fungus Color variables
    /// </summary>
    public class ColorVarSaver : VarSaver
    {
        protected override ContentType SetContentAs => ContentType.jsonString;
    }
}