using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Fungus manager singleton. Manages access to all Fungus singletons in a consistent manner.
    /// </summary>
    [RequireComponent(typeof(CameraManager))]
    [RequireComponent(typeof(MusicManager))]
    public sealed class FungusManager : MonoBehaviour
    {
        static FungusManager instance;
        static bool applicationIsQuitting = false;
        static object _lock = new object();

        void Awake()
        {
            CameraManager = GetComponent<CameraManager>();
            MusicManager = GetComponent<MusicManager>();
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        void OnDestroy () 
        {
            applicationIsQuitting = true;
        }

        #region Public methods

        /// <summary>
        /// Gets the camera manager singleton instance.
        /// </summary>
        public CameraManager CameraManager { get; private set; }

        /// <summary>
        /// Gets the music manager singleton instance.
        /// </summary>
        public MusicManager MusicManager { get; private set; }

        /// <summary>
        /// Gets the FungusManager singleton instance.
        /// </summary>
        public static FungusManager Instance
        {
            get
            {
                if (applicationIsQuitting) 
                {
                    Debug.LogWarning("FungusManager.Instance() was called while application is quitting. Returning null instead.");
                    return null;
                }

                lock (_lock)
                {
                    if (instance == null)
                    {
                        var go = new GameObject();
                        go.name = "FungusManager";
                        DontDestroyOnLoad(go);
                        instance = go.AddComponent<FungusManager>();
                    }

                    return instance;
                }
            }
        }

        #endregion
    }
}