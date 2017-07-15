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
    /// </summary>
    [InitializeOnLoad]
    public class HierarchyIcons
    {
        // the fungus mushroom icon
        static Texture2D textureIcon;

        //sorted list of the GO instance IDs that have flowcharts on them
        static List<int> flowchartIDs = new List<int>();

        static HierarchyIcons()
        {
            //please don't move the fungus icon :(
            textureIcon = AssetDatabase.LoadAssetAtPath("Assets/Fungus/Textures/ScriptIcon.png", typeof(Texture2D)) as Texture2D;
            
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyIconCallback;
            EditorApplication.hierarchyWindowChanged += HierarchyChanged;
        }

        //track all gameobjectIds that have flowcharts on them
        static void HierarchyChanged()
        {
            flowchartIDs.Clear();

            var flowcharts = GameObject.FindObjectsOfType<Flowchart>();

            flowchartIDs = flowcharts.Select(x => x.gameObject.GetInstanceID()).Distinct().ToList();
            flowchartIDs.Sort();
        }

        //Draw icon if the isntance id is in our cached list
        static void HierarchyIconCallback(int instanceID, Rect selectionRect)
        {
            // place the icon to the left of the element
            Rect r = new Rect(selectionRect);
            r.x = 0;
            r.width = r.height;

            //GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            
            //binary search as it is much faster to cache and int bin search than GetComponent
            //  should be less GC too
            if (flowchartIDs.BinarySearch(instanceID) >= 0)
                GUI.Label(r, textureIcon);
        }
    }
}