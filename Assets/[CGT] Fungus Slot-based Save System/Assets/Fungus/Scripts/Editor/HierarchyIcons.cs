using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Fungus
{
    /// <summary>
    /// Static class that hooks into the hierachy changed and item gui callbacks to put
    /// a fungus icon infront of all GOs that have a flowchart on them
    /// 
    /// Reference; http://answers.unity3d.com/questions/431952/how-to-show-an-icon-in-hierarchy-view.html
    /// 
    /// TODO
    /// There is what appears like a bug but is currently out of our control. When Unity reloads the built scripts it fires
    /// InitializeOnLoad but doesn't then fire HierarchyChanged so icons disappear until a change occurs
    /// </summary>
    [InitializeOnLoad]
    public class HierarchyIcons
    {
        // the fungus mushroom icon
        static Texture2D TextureIcon { get { return Fungus.EditorUtils.FungusEditorResources.FungusMushroom; } }

        //sorted list of the GO instance IDs that have flowcharts on them
        static List<int> flowchartIDs = new List<int>();

        static bool initalHierarchyCheckFlag = true;

        static HierarchyIcons()
        {
            initalHierarchyCheckFlag = true;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyIconCallback;
#if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += HierarchyChanged;
#else
            EditorApplication.hierarchyWindowChanged += HierarchyChanged;
#endif
        }

        //track all gameobjectIds that have flowcharts on them
        static void HierarchyChanged()
        {
            flowchartIDs.Clear();

            if (EditorUtils.FungusEditorPreferences.hideMushroomInHierarchy)
                return;

            var flowcharts = GameObject.FindObjectsOfType<Flowchart>();

            flowchartIDs = flowcharts.Select(x => x.gameObject.GetInstanceID()).Distinct().ToList();
            flowchartIDs.Sort();
        }

        //Draw icon if the isntance id is in our cached list
        static void HierarchyIconCallback(int instanceID, Rect selectionRect)
        {
            if(initalHierarchyCheckFlag)
            {
                HierarchyChanged();
                initalHierarchyCheckFlag = false;
            }

            if (EditorUtils.FungusEditorPreferences.hideMushroomInHierarchy)
                return;

            // place the icon to the left of the element
            Rect r = new Rect(selectionRect);
            r.x = 0;
            r.width = r.height;

            //GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            //binary search as it is much faster to cache and int bin search than GetComponent
            //  should be less GC too
            if (flowchartIDs.BinarySearch(instanceID) >= 0)
                GUI.Label(r, TextureIcon);
        }
    }
}