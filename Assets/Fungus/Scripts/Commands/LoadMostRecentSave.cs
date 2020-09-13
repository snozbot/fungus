// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEngine;
using static Fungus.SaveManager;

namespace Fungus
{
    /// <summary>
    /// Requests SaveManager load a recent save, be it Auto, User, or Regardless of type.
    /// </summary>
    [CommandInfo("Save",
                 "Load Latest Save",
                 "Requests SaveManager load a recent save, be it Auto, User, or Regardless of type.")]
    public class LoadMostRecentSave : Command
    {
        [SerializeField] protected SaveType saveType = SaveType.Any;

        public override void OnEnter()
        {
            SaveGameMetaData save = null;
            var saveMan = FungusManager.Instance.SaveManager;

            switch (saveType)
            {
                case SaveType.Auto:
                    save = saveMan.CollectAutoSaves().LastOrDefault();
                    break;

                case SaveType.Slot:
                    save = saveMan.CollectUserSaves()
                        .OrderByDescending(x => x.lastWritten.Ticks).FirstOrDefault();
                    break;

                case SaveType.Any:
                    save = saveMan.SaveFileManager.SaveMetas
                        .OrderByDescending(x => x.lastWritten.Ticks).FirstOrDefault();
                    break;

                default:
                break;
            }

            if (save != null)
            {
                saveMan.Load(save);
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            return saveType.ToString() + base.GetSummary();
        }
    }
}