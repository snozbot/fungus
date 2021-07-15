// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    ///
    /// </summary>
    [CommandInfo("Physics",
                 "Overlap",
                     "Find all gameobjects hit by given physics shape overlap")]
    [AddComponentMenu("")]
    public class PhysicsOverlap : CollectionBaseCommand
    {
        public enum Shape
        {
            Box,
            Capsule,
            Sphere,
        }

        [Tooltip("")]
        [SerializeField]
        protected Shape shape = Shape.Box;

        [Tooltip("Starting point or centre of shape")]
        [SerializeField]
        protected Vector3Data position1;

        [Tooltip("CAPSULE ONLY; end point of the capsule")]
        [SerializeField]
        protected Vector3Data capsulePosition2;

        [Tooltip("CAPSULE & SPHERE ONLY")]
        [SerializeField]
        protected FloatData radius = new FloatData(0.5f);

        [Tooltip("BOX ONLY")]
        [SerializeField]
        protected Vector3Data boxHalfExtends = new Vector3Data(Vector3.one * 0.5f);

        [Tooltip("BOX ONLY")]
        [SerializeField]
        protected QuaternionData boxOrientation = new QuaternionData(Quaternion.identity);

        [Tooltip("")]
        [SerializeField]
        protected LayerMask layerMask = ~0;

        [Tooltip("")]
        [SerializeField]
        protected QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        public override void OnEnter()
        {
            var col = collection.Value;

            if (col != null)
            {
                Collider[] resColliders = null;

                switch (shape)
                {
                    case Shape.Box:
                        resColliders = Physics.OverlapBox(position1.Value, boxHalfExtends.Value, boxOrientation.Value, layerMask.value, queryTriggerInteraction);
                        break;

                    case Shape.Sphere:
                        resColliders = Physics.OverlapSphere(position1.Value, radius.Value, layerMask.value, queryTriggerInteraction);
                        break;

                    case Shape.Capsule:
                        resColliders = Physics.OverlapCapsule(position1.Value, capsulePosition2.Value, radius.Value, layerMask.value, queryTriggerInteraction);
                        break;

                    default:
                    break;
                }

                PutCollidersIntoGameObjectCollection(resColliders);
            }

            Continue();
        }

        protected void PutCollidersIntoGameObjectCollection(Collider[] resColliders)
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
                variable == capsulePosition2.vector3Ref ||
                variable == radius.floatRef ||
                variable == boxHalfExtends.vector3Ref ||
                variable == boxOrientation.quaternionRef ||
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

            if ((shape == Shape.Capsule || shape == Shape.Sphere) && propertyName == "radius")
                return true;

            if (shape == Shape.Box && (propertyName == "boxHalfExtends" || propertyName == "boxOrientation"))
                return true;

            return base.IsPropertyVisible(propertyName);
        }
    }
}