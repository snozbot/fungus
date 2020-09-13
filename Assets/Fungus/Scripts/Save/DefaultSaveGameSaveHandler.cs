// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public class DefaultSaveGameSaveHandler : DefaultSaveHandler
    {
        public static ISaveHandler CreateDefaultWithSerializers()
        {
            return new DefaultSaveGameSaveHandler(
                new FungusSystemSaveDataItemSerializer(),
                new GlobalVariableSaveDataItemSerializer(),
                new MenuSaveDataItemSerializer());
        }
        public override int CurrentExpectedVersion => FungusConstants.CurrentSaveGameDataVersion;

        public DefaultSaveGameSaveHandler() { }
        public DefaultSaveGameSaveHandler(params ISaveDataItemSerializer[] handlers)
        {
            saveDataItemSerializers.AddRange(handlers);
        }
    }
}
