using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets a game object in the scene to be active / inactive.
    /// </summary>
    [CommandInfo("Flow",
                 "Is Inactive",
                 "Checks if an object is inactive (disabled) in the scene.\n" +
                "If 'ignoreParent' is true, this command will only return the state of the object independent of the parent." +
                "\nLeave this as true unless you really need to check activeInHierarchy")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class IsInactive : Condition
    {
        [Tooltip("Reference of game object to check")]
        [SerializeField] protected GameObjectData targetObject;

        [Tooltip("If enabled, the state of the object itself will be returned. This means even if the parent is disabled, if the selected object is active, that will be returned.\n" +
            "Leave enabled unless you need the parents' state too")]
        [SerializeField] protected bool ignoreParent = true;

        protected override bool EvaluateCondition()
        {
            if (ignoreParent) return targetObject.Value.activeSelf == false;

            else return targetObject.Value.activeInHierarchy == false;
        }

        public override string GetSummary()
        {
            if (targetObject.Value == null) return "Error: no target object assigned!";

            string hierarchy = ignoreParent == true ? " in hierarchy, ignoring parent" : string.Empty;
            return "Is " + targetObject.Value.name + " inactive" + hierarchy + "?";
        }

        public override bool HasReference(Variable variable)
        {
            return targetObject.gameObjectRef == variable || base.HasReference(variable);
        }
    }
}