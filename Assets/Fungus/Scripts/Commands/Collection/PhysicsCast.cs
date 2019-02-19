using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// 
    /// </summary>
    [CommandInfo("Physics",
                    "Cast",
                    "Find all gameobjects hit by given physics shape cast")]
    [AddComponentMenu("")]
    public class PhysicsCast : CollectionBaseCommand
    {
        public enum CastType
        {
            Box,
            Capsule,
            Ray,
            Sphere,
        }

        [SerializeField]
        protected CastType castType = CastType.Ray;

        [Tooltip("Starting point/origin or centre of shape")]
        [SerializeField]
        protected Vector3Data position1;

        [Tooltip("")]
        [SerializeField]
        protected Vector3Data direction;

        [Tooltip("")]
        [SerializeField]
        protected FloatData maxDistance = new FloatData(float.PositiveInfinity);

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
                RaycastHit[] resHits = null;

                switch (castType)
                {
                    case CastType.Ray:
                        resHits = Physics.RaycastAll(position1.Value, direction.Value, maxDistance.Value, layerMask.value, queryTriggerInteraction);
                        break;
                    case CastType.Sphere:
                        resHits = Physics.SphereCastAll(position1.Value, radius.Value, direction.Value, maxDistance.Value, layerMask.value, queryTriggerInteraction);
                        break;
                    case CastType.Box:
                        resHits = Physics.BoxCastAll(position1.Value, boxHalfExtends.Value, direction.Value, boxOrientation.Value, maxDistance.Value, layerMask.value, queryTriggerInteraction);
                        break;
                    case CastType.Capsule:
                        resHits = Physics.CapsuleCastAll(position1.Value, capsulePosition2.Value, radius.Value, direction.Value, maxDistance.Value, layerMask.value, queryTriggerInteraction);
                        break;
                    default:
                        break;
                }

                PutCollidersIntoGameObjectCollection(resHits);
            }

            Continue();
        }

        protected void PutCollidersIntoGameObjectCollection(RaycastHit[] resHits)
        {
            if (resHits != null)
            {
                var col = collection.Value;
                for (int i = 0; i < resHits.Length; i++)
                {
                    col.Add(resHits[i].collider.gameObject);
                }
            }
        }

        public override bool HasReference(Variable variable)
        {
            return variable == direction.vector3Ref ||
                variable == maxDistance.floatRef ||
                variable == position1.vector3Ref ||
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

            return castType.ToString() + ", store in " + collection.Value.name;
        }
    }
}