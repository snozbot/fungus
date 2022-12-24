namespace Fungus.LionManeSaveSys
{
    public class VectorVarSaver : VarSaver
    {
        protected override ContentType SetContentAs
        {
            get { return ContentType.jsonString; }
        }
    }
}