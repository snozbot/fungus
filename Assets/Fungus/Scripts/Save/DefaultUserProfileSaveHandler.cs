// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public class DefaultUserProfileSaveHandler : DefaultSaveHandler
    {
        public static ISaveHandler CreateDefaultWithSerializers()
        {
            return new DefaultUserProfileSaveHandler(
                new FungusPerProfileSaveDataItemSerializer());
        }

        public override int CurrentExpectedVersion => SaveSysConstants.CurrentProfileDataVersion;

        public DefaultUserProfileSaveHandler()
        {
        }

        public DefaultUserProfileSaveHandler(params ISaveDataItemSerializer[] handlers)
        {
            saveDataItemSerializers.AddRange(handlers);
        }
    }
}
