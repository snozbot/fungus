// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

public enum FollowTypes
{
    FollowDamp,
    FollowSpring,
    FollowBounceOut,
    FollowLinear,
    Disable
}
namespace Fungus
{
    /// <summary>
    /// Follows target gameObject.
    /// </summary>
    [CommandInfo("LeanTween",
                 "Follow",
                 "Follows an object based on it's position, color or scale")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class FollowLean : Command
    {
        [Tooltip("Follow ease types")]
        [SerializeField] protected FollowTypes followType;

        [Tooltip("What type of property to follow")]
        [SerializeField] protected LeanProp followProperty = LeanProp.position;

        [Tooltip("GameObject that will follow target gameObject")]
        [SerializeField] protected TransformData objectToFollow;

        [Tooltip("Target gameObject to follow")]
        [SerializeField] protected TransformData objectFollowing;

        [Tooltip("Set offset to object that's following")]
        [SerializeField] protected Vector3Data setOffset = new Vector3Data(Vector3.zero);

        [Tooltip("Duration")]
        [SerializeField] protected FloatData smooth = new FloatData(1f);
        public override void OnEnter()
        {
            if(objectFollowing.Value != null && objectToFollow.Value != null)
            {
                switch(followType)
                {
                    case FollowTypes.FollowDamp:
                        LeanTween.followDamp(objectFollowing.Value, objectToFollow.Value, followProperty, smooth.Value)
                        .setOffset(setOffset);
                    break;
                    case FollowTypes.FollowSpring:
                        LeanTween.followSpring(objectFollowing.Value, objectToFollow.Value, followProperty, smooth.Value)
                        .setOffset(setOffset);
                    break;
                    case FollowTypes.FollowBounceOut:
                        LeanTween.followBounceOut(objectFollowing.Value, objectToFollow.Value, followProperty, smooth.Value)
                        .setOffset(setOffset);
                    break;
                    case FollowTypes.FollowLinear:
                        LeanTween.followLinear(objectFollowing.Value, objectToFollow.Value, followProperty, smooth.Value)
                        .setOffset(setOffset);
                    break;
                }
            }

            Continue();
        }

        public override string GetSummary()
        {
            string objFol = string.Empty;
            string objToFol = string.Empty;

            if (objectToFollow.Value == null)
            {
                return objToFol = "Error: No game object to be followed";
            }
            if (objectFollowing.Value == null)
            {
                return objFol = "Error: No game object for following";
            }
            return objFol + " " + objToFol;
        }
        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to FollowTypes
            followType = FollowTypes.FollowDamp;
        }
        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }
        public override bool HasReference(Variable variable)
        {
            return variable == smooth.floatRef || objectFollowing.transformRef || objectToFollow.transformRef || setOffset.vector3Ref;
        }
    }
}