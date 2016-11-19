using UnityEngine;
using System.Collections;

namespace Fungus
{
    public class SavePicker : MonoBehaviour
    {
        #region Public methods

        public virtual void Select(int slot)
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Load(slot);
        }

        public virtual void Delete(int slot)
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Delete(slot);
        }

        #endregion
    }
}