namespace Fungus
{
    public class PortraitStateSaver : DataSaver
    {
        public override ISaveUnit CreateSaveFrom(object input)
        {
            if (IsValid(input))
                return new PortraitSaveUnit(input as Character);
            else
                return null;
        }

        protected override bool IsValid(object input)
        {
            return input is Character;
        }

        public virtual PortraitSaveUnit CreateSaveFrom(Character character)
        {
            return PortraitSaveUnit.From(character);
        }

    }

}