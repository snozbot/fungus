using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveHelper : MonoBehaviour 
    {
        [SerializeField] protected string startScene = "";

        public virtual void Load(int slot)
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Load(slot, startScene);
        }

        public virtual void Save()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Save();
        }

        public virtual void Delete(int slot)
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Delete(slot);
        }

        public virtual void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}