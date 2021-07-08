// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Fungus
{
    /// <summary>
    /// Set the interactable state of selectable objects.
    /// </summary>
    [CommandInfo("UI",
                 "Set Interactable",
                 "Set the interactable state of selectable objects.")]
    public class SetInteractable : Command
    {
        [Tooltip("List of objects to be affected by the command")]
        [SerializeField] protected List<GameObject> targetObjects = new List<GameObject>();

        [Tooltip("Controls if the selectable UI object be interactable or not")]
        [SerializeField] protected BooleanData interactableState = new BooleanData(true);

        [Tooltip("Whether or not this will apply to each child of each target object. The children are found recursively.")]
        [SerializeField] protected BooleanData applyToChildren = new BooleanData(false);

        #region Public members

        public override void OnEnter()
        {
            if (this.HasNothingToWorkWith)
            {
                Continue();
                return;
            }

            ApplyStateTo(targetObjects);

            Continue();
        }

        protected virtual bool HasNothingToWorkWith
        {
            get { return targetObjects.Count == 0 || AllTargetsAreNull; }
        }

        protected virtual bool AllTargetsAreNull
        {
            get
            {
                for (int i = 0; i < targetObjects.Count; i++)
                    if (targetObjects[i] != null)
                        return false;

                return true;
            }
        }

        protected virtual void ApplyStateTo(IList<GameObject> targetObjects)
        {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                GameObject target = targetObjects[i];
                List<Selectable> selectables = GetSelectablesFrom(target);
                ApplyStateTo(selectables);
            }
        }

        List<Selectable> GetSelectablesFrom(GameObject target)
        {
            List<Selectable> selectables = target.GetComponents<Selectable>().ToList();

            if (applyToChildren)
            {
                Selectable[] children = target.GetComponentsInChildren<Selectable>();
                selectables.AddRange(children);
            }

            return selectables;
        }

        protected virtual void ApplyStateTo(IList<Selectable> selectables)
        {
            for (int i = 0; i < selectables.Count; i++)
            {
                Selectable toSetStateOf = selectables[i];
                toSetStateOf.interactable = interactableState;
            }
        }

        public override string GetSummary()
        {
            string summary = null;

            if (this.HasNothingToWorkWith)
                summary = summaryForEmptyList;
            
            else if (this.HasOneTargetSelected)
                summary = SummaryForSingleTarget;
            
            else if (this.HasMultipleTargetsSelected)
                summary = SummaryForMultiTargets;

            return summary;
        }

        // Valid as in, non-null
        protected static string summaryForEmptyList = "Error: No valid Target Objects selected.";

        protected virtual bool HasOneTargetSelected
        {
            get { return targetObjects.Count == 1 && targetObjects[0] != null; }
        }

        protected virtual string SummaryForSingleTarget
        {
            get { return targetObjects[0].name + " = " + interactableState.Value; }
        }

        protected virtual bool HasMultipleTargetsSelected
        {
            get { return targetObjects.Count > 1; }
        }

        protected virtual string SummaryForMultiTargets
        {
            get
            {
                string summary = "";

                List<GameObject> nonNullTargets = GetNonNullTargets(); 
                // ^Lets us cut out the null checks in the for loop

                // By this point in the code, we should have at least 1 valid
                // target to work with. Thus...
                summary = string.Concat(summary, nonNullTargets[0].name); 
                // ^We can also cut out the checks for the first valid target

                for (int i = 1; i < nonNullTargets.Count; i++)
                {
                    GameObject target = nonNullTargets[i];
                    summary = string.Concat(summary, ", ", target.name);
                }

                summary = string.Concat(" = ", interactableState);
                // string.Concat is more performant than both regular string concatenation
                // and StringBuilder, so yeah.
                return summary;
            }
        }

        protected virtual List<GameObject> GetNonNullTargets()
        {
            List<GameObject> nonNulls = new List<GameObject>(targetObjects);
            nonNulls.RemoveAll((target) => target == null);
            return nonNulls;
        }

        public override Color GetButtonColor()
        {
            return new Color32(180, 250, 250, 255);
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            targetObjects.Add(null);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetObjects")
            {
                return true;
            }

            return false;
        }

        public override bool HasReference(Variable variable)
        {
            return interactableState.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}