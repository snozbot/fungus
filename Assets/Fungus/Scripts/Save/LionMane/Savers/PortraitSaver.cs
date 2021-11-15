namespace Fungus.LionManeSaveSys
{
    public class PortraitSaver : DataSaver<PortraitSaveUnit, Character>
    {
        public override ISaveUnit CreateSaveFrom(object input)
        {
            if (IsValid(input))
                return PortraitSaveUnit.From(input as Character);
            else
                return PortraitSaveUnit.Null;
        }

        protected override bool IsValid(object input)
        {
            return input is Character;
        }

    }

}