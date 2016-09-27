// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Defines a camera view point.
    /// The position and rotation are specified using the game object's transform, so this class only needs to specify the ortographic view size.
    /// </summary>
    [ExecuteInEditMode]
    public class View : MonoBehaviour
    {
        [Tooltip("Orthographic size of the camera view in world units.")]
        [SerializeField] protected float viewSize = 0.5f;

        [Tooltip("Aspect ratio of the primary view rectangle. (e.g. 4:3 aspect ratio = 1.333)")]
        [SerializeField] protected Vector2 primaryAspectRatio = new Vector2(4, 3);

        [Tooltip("Aspect ratio of the secondary view rectangle. (e.g. 2:1 aspect ratio = 2.0)")]
        [SerializeField] protected Vector2 secondaryAspectRatio = new Vector2(2, 1);

        protected virtual void Update()
        {
            // Disable scaling to avoid complicating the orthographic size calculations
            transform.localScale = new Vector3(1,1,1);
        }

        #region Public members

        /// <summary>
        /// Orthographic size of the camera view in world units.
        /// </summary>
        public virtual float ViewSize { get { return viewSize; } set { viewSize = value; } }

        /// <summary>
        /// Aspect ratio of the primary view rectangle. e.g. a 4:3 aspect ratio = 1.333.
        /// </summary>
        public virtual Vector2 PrimaryAspectRatio { get { return primaryAspectRatio; } set { primaryAspectRatio = value; } }

        /// <summary>
        /// Aspect ratio of the secondary view rectangle. e.g. a 2:1 aspect ratio = 2/1 = 2.0.
        /// </summary>
        public virtual Vector2 SecondaryAspectRatio { get { return secondaryAspectRatio; } set { secondaryAspectRatio = value; } }

        #endregion
    }
}