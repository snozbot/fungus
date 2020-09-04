// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    public class SetCurrentSaveHandler : MonoBehaviour
    {
        protected ISaveHandler previousSaveHandler;

        public SaveDataItemSerializerComponent[] saveDataItemSerializerComponents;

        public void OnEnable()
        {
            previousSaveHandler = FungusManager.Instance.SaveManager.CurrentSaveHandler;

            var newHandler = DefaultSaveHandler.CreateDefaultWithSerializers();

            foreach (var item in saveDataItemSerializerComponents)
            {
                newHandler.SaveDataItemSerializers.Add(item.SaveDataItemSerializer);
            }

            FungusManager.Instance.SaveManager.CurrentSaveHandler = newHandler;
        }

        public void OnDisable()
        {
            var fm = FungusManager.Instance;
            if (fm != null)
                fm.SaveManager.CurrentSaveHandler = previousSaveHandler;
        }
    }
}