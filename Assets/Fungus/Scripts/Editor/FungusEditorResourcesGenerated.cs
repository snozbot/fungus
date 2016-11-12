// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus.EditorUtils
{
    internal static partial class FungusEditorResources
    {
        private static readonly string[] resourceNames = new [] {
            "add",
            "add_small",
            "delete",
            "down",
            "duplicate",
            "up",
            "choice_node_off",
            "choice_node_on",
            "command_background",
            "connection_point",
            "event_node_off",
            "event_node_on",
            "play_big",
            "play_small",
            "process_node_off",
            "process_node_on",
        };

        public static Texture2D Add { get { return GetTexture("add"); } }
        public static Texture2D AddSmall { get { return GetTexture("add_small"); } }
        public static Texture2D Delete { get { return GetTexture("delete"); } }
        public static Texture2D Down { get { return GetTexture("down"); } }
        public static Texture2D Duplicate { get { return GetTexture("duplicate"); } }
        public static Texture2D Up { get { return GetTexture("up"); } }
        public static Texture2D ChoiceNodeOff { get { return GetTexture("choice_node_off"); } }
        public static Texture2D ChoiceNodeOn { get { return GetTexture("choice_node_on"); } }
        public static Texture2D CommandBackground { get { return GetTexture("command_background"); } }
        public static Texture2D ConnectionPoint { get { return GetTexture("connection_point"); } }
        public static Texture2D EventNodeOff { get { return GetTexture("event_node_off"); } }
        public static Texture2D EventNodeOn { get { return GetTexture("event_node_on"); } }
        public static Texture2D PlayBig { get { return GetTexture("play_big"); } }
        public static Texture2D PlaySmall { get { return GetTexture("play_small"); } }
        public static Texture2D ProcessNodeOff { get { return GetTexture("process_node_off"); } }
        public static Texture2D ProcessNodeOn { get { return GetTexture("process_node_on"); } }
    }
}
