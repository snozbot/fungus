// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Requests SaveManager load a recent save, be it Auto, User, or Regardless of type.
    /// </summary>
    [CommandInfo("Save",
                 "Load Latest Auto Save",
                 "Requests SaveManager load a recent save, be it Auto, User, or Regardless of type.")]
    public class LoadMostRecentSave : Command
    {
        public enum SaveType
        {
            Auto,
            User,
            Any,
        }

        [SerializeField] protected SaveType saveType = SaveType.Any;

        public override void OnEnter()
        {
            SaveManager.SavePointMeta save = null;

            switch (saveType)
            {
                case SaveType.Auto:
                    save = FungusManager.Instance.SaveManager.CollectAutoSaves().LastOrDefault();
                    break;

                case SaveType.User:
                    save = FungusManager.Instance.SaveManager.CollectUserSaves()
                        .OrderByDescending(x => x.savePointLastWritten.Ticks).FirstOrDefault();
                    break;

                case SaveType.Any:
                    save = FungusManager.Instance.SaveManager.SaveMetas
                        .OrderByDescending(x => x.savePointLastWritten.Ticks).FirstOrDefault();
                    break;

                default:
                break;
            }

            if (save != null)
            {
                FungusManager.Instance.SaveManager.Load(save);
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