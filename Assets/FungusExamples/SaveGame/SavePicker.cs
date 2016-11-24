using UnityEngine;
using System.Collections;

namespace Fungus
{
    public class SavePicker : MonoBehaviour
    {
        [SerializeField] protected string newGameDescription = "Playing";

        #region Public methods

        public virtual void Select(int slot)
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (saveManager.SlotExists(slot))
            {
                saveManager.Load(slot);
            }
            else
            {
                saveManager.LoadNewGame(slot, newGameDescription);
            }
        }

        public virtual void Delete(int slot)
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Delete(slot);
        }

        #endregion
    }
}