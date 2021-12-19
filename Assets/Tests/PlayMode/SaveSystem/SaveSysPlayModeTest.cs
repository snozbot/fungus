using NUnit.Framework;
using UnityEngine;

namespace SaveSystemTests
{
    /// <summary>
    /// Base class for all play mode unit tests we're setting up for the save system.
    /// </summary>
    public abstract class SaveSysPlayModeTest
    {
        /// <summary>
        /// Relative to a Resources folder
        /// </summary>
        protected virtual string PathToScene { get { return ""; } }
        protected GameObject scenePrefab, sceneObj;

        [SetUp]
        public virtual void SetUp()
        {
            scenePrefab = Resources.Load<GameObject>(PathToScene);
            sceneObj = Object.Instantiate<GameObject>(scenePrefab);
        }

        [TearDown]
        public virtual void Teardown()
        {
            Object.Destroy(sceneObj.gameObject);
        }
    }
}
