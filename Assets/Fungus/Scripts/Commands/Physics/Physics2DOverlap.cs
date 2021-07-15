// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [CommandInfo("Physics2D",
                 "Overlap2D",
                     "Find all gameobjects hit by given physics shape overlap")]
    [AddComponentMenu("")]
    public class Physics2DOverlap : CollectionBaseCommand
    {
        public enum Shape
        {
            Point,
            Area,
            Box,
            Circle,
            Capsule,
        }

        [Tooltip("")]
        [SerializeField]
        protected Shape shape = Shape.Box;

        [Tooltip("Starting point or centre of shape")]
        [SerializeField]
        protected Vector3Data position1;

        [Tooltip("AREA ONLY")]
        [SerializeField]
        protected Vector3Data areaEndPosition;

        [Tooltip("CAPSULE & Circle ONLY")]
        [SerializeField]
        protected FloatData radius = new FloatData(0.5f);

        [Tooltip("BOX & CAPSULE ONLY")]
        [SerializeField]
        protected Vector3Data shapeSize = new Vector3Data(Vector3.one * 0.5f);

        [Tooltip("BOX & CAPSULE ONLY")]
        [SerializeField]
        protected FloatData shapeAngle;

        [Tooltip("")]
        [SerializeField]
        protected LayerMask layerMask = ~0;

        [Tooltip("")]
        [SerializeField]
        protected FloatData minDepth = new FloatData(float.NegativeInfinity), maxDepth = new FloatData(float.PositiveInfinity);

        [SerializeField]
        protected CapsuleDirection2D capsuleDirection;

        public override void OnEnter()
        {
            var col = collection.Value;

            if (col != null)
            {
                Collider2D[] resColliders = null;

                switch (shape)
                {
                    case Shape.Area:
                        resColliders = Physics2D.OverlapAreaAll(position1.Value, areaEndPosition.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case Shape.Box:
                        resColliders = Physics2D.OverlapBoxAll(position1.Value, shapeSize.Value, shapeAngle.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case Shape.Circle:
                        resColliders = Physics2D.OverlapCircleAll(position1.Value, radius.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case Shape.Capsule:
                        resColliders = Physics2D.OverlapCapsuleAll(position1.Value, shapeSize.Value, capsuleDirection, shapeAngle.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case Shape.Point:
                        resColliders = Physics2D.OverlapPointAll(position1.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    default:
                    break;
                }

                PutCollidersIntoGameObjectCollection(resColliders);
            }

            Continue();
        }

        protected void PutCollidersIntoGameObjectCollection(Collider2D[] resColliders)
        {
            if (resColliders != null)
            {
                var col = collection.Value;
                for (int i = 0; i < resColliders.Length; i++)
                {
                    col.Add(resColliders[i].gameObject);
                }
            }
        }

        public override bool HasReference(Variable variable)
        {
            return variable == position1.vector3Ref ||
                variable == areaEndPosition.vector3Ref ||
                variable == radius.floatRef ||
                variable == shapeSize.vector3Ref ||
                variable == shapeAngle.floatRef ||
                variable == minDepth.floatRef ||
                variable == maxDepth.floatRef ||
                base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            //TODO we could support more than just GOs
            if (!(collection.Value is GameObjectCollection))
                return "Error: collection is not GameObjectCollection";

            return shape.ToString() + ", store in " + collection.Value.name;
        }

        public override bool IsPropertyVisible(string propertyName)
        {
            if (shape == Shape.Capsule && propertyName == "capsulePosition2")
                return true;

            if (shape == Shape.Area && propertyName == "areaEndPosition")
                return true;

            if ((shape == Shape.Capsule || shape == Shape.Circle) && propertyName == "radius")
                return true;

            if ((shape == Shape.Capsule || shape == Shape.Box) && propertyName == "shapeAngle")
                return true;

            if (shape == Shape.Box && (propertyName == "boxHalfExtends" || propertyName == "boxOrientation"))
                return true;

            return base.IsPropertyVisible(propertyName);
        }
    }
}