// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [CommandInfo("Physics2D",
                 "Cast2D",
                     "Find all gameobjects hit by given physics shape overlap")]
    [AddComponentMenu("")]
    public class Physics2DCast : CollectionBaseCommand
    {
        public enum CastType
        {
            Box,
            Capsule,
            Circle,
            Line,
            Ray,
        }

        [Tooltip("")]
        [SerializeField]
        protected CastType castType = CastType.Ray;

        [Tooltip("Starting point or centre of shape")]
        [SerializeField]
        protected Vector3Data position1;

        [Tooltip("")]
        [SerializeField]
        protected Vector3Data direction;

        [Tooltip("")]
        [SerializeField]
        protected FloatData maxDistance = new FloatData(float.PositiveInfinity);

        [Tooltip("CAPSULE & Circle ONLY")]
        [SerializeField]
        protected FloatData radius = new FloatData(0.5f);

        [Tooltip("BOX & CAPSULE ONLY")]
        [SerializeField]
        protected Vector3Data shapeSize = new Vector3Data(Vector3.one * 0.5f);

        [Tooltip("BOX & CAPSULE ONLY")]
        [SerializeField]
        protected FloatData shapeAngle;

        [Tooltip("LINE ONLY")]
        [SerializeField]
        protected Vector3Data lineEnd;

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
                RaycastHit2D[] resHits = null;

                switch (castType)
                {
                    case CastType.Box:
                        resHits = Physics2D.BoxCastAll(position1.Value, shapeSize.Value, shapeAngle.Value, direction.Value, maxDistance.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case CastType.Capsule:
                        resHits = Physics2D.CapsuleCastAll(position1.Value, shapeSize.Value, capsuleDirection, shapeAngle.Value, direction.Value, maxDistance.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case CastType.Circle:
                        resHits = Physics2D.CircleCastAll(position1.Value, radius.Value, direction.Value, maxDistance.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case CastType.Line:
                        resHits = Physics2D.LinecastAll(position1.Value, lineEnd.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    case CastType.Ray:
                        resHits = Physics2D.RaycastAll(position1.Value, direction.Value, maxDistance.Value, layerMask.value, minDepth.Value, maxDepth.Value);
                        break;

                    default:
                    break;
                }

                PutCollidersIntoGameObjectCollection(resHits);
            }

            Continue();
        }

        protected void PutCollidersIntoGameObjectCollection(RaycastHit2D[] resColliders)
        {
            if (resColliders != null)
            {
                var col = collection.Value;
                for (int i = 0; i < resColliders.Length; i++)
                {
                    col.Add(resColliders[i].collider.gameObject);
                }
            }
        }

        public override bool HasReference(Variable variable)
        {
            return variable == position1.vector3Ref ||
                variable == radius.floatRef ||
                variable == shapeSize.vector3Ref ||
                variable == shapeAngle.floatRef ||
                variable == minDepth.floatRef ||
                variable == maxDepth.floatRef ||
                variable == direction.vector3Ref ||
                variable == maxDistance.floatRef ||
                variable == lineEnd.vector3Ref ||
                base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            //TODO we could support more than just GOs
            if (!(collection.Value is GameObjectCollection))
                return "Error: collection is not GameObjectCollection";

            return castType.ToString() + ", store in " + collection.Value.name;
        }

        public override bool IsPropertyVisible(string propertyName)
        {
            if (castType == CastType.Capsule && propertyName == "capsulePosition2")
                return true;

            if (castType == CastType.Line && propertyName == "lineEnd")
                return true;

            if ((castType == CastType.Capsule || castType == CastType.Circle) && propertyName == "radius")
                return true;

            if ((castType == CastType.Capsule || castType == CastType.Box) &&
                (propertyName == "shapeAngle" || propertyName == "shapeSize"))
                return true;

            return base.IsPropertyVisible(propertyName);
        }
    }
}