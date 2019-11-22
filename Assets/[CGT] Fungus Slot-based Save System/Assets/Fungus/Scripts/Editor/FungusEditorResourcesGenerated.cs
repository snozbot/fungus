// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus.EditorUtils
{
    internal partial class FungusEditorResources : ScriptableObject
    {
        [SerializeField] private EditorTexture add;
        [SerializeField] private EditorTexture add_small;
        [SerializeField] private EditorTexture delete;
        [SerializeField] private EditorTexture down;
        [SerializeField] private EditorTexture duplicate;
        [SerializeField] private EditorTexture fungus_mushroom;
        [SerializeField] private EditorTexture up;
        [SerializeField] private EditorTexture bullet_point;
        [SerializeField] private EditorTexture choice_node_off;
        [SerializeField] private EditorTexture choice_node_on;
        [SerializeField] private EditorTexture command_background;
        [SerializeField] private EditorTexture connection_point;
        [SerializeField] private EditorTexture event_node_off;
        [SerializeField] private EditorTexture event_node_on;
        [SerializeField] private EditorTexture play_big;
        [SerializeField] private EditorTexture play_small;
        [SerializeField] private EditorTexture process_node_off;
        [SerializeField] private EditorTexture process_node_on;

        public static Texture2D Add { get { return Instance.add.Texture2D; } }
        public static Texture2D AddSmall { get { return Instance.add_small.Texture2D; } }
        public static Texture2D Delete { get { return Instance.delete.Texture2D; } }
        public static Texture2D Down { get { return Instance.down.Texture2D; } }
        public static Texture2D Duplicate { get { return Instance.duplicate.Texture2D; } }
        public static Texture2D FungusMushroom { get { return Instance.fungus_mushroom.Texture2D; } }
        public static Texture2D Up { get { return Instance.up.Texture2D; } }
        public static Texture2D BulletPoint { get { return Instance.bullet_point.Texture2D; } }
        public static Texture2D ChoiceNodeOff { get { return Instance.choice_node_off.Texture2D; } }
        public static Texture2D ChoiceNodeOn { get { return Instance.choice_node_on.Texture2D; } }
        public static Texture2D CommandBackground { get { return Instance.command_background.Texture2D; } }
        public static Texture2D ConnectionPoint { get { return Instance.connection_point.Texture2D; } }
        public static Texture2D EventNodeOff { get { return Instance.event_node_off.Texture2D; } }
        public static Texture2D EventNodeOn { get { return Instance.event_node_on.Texture2D; } }
        public static Texture2D PlayBig { get { return Instance.play_big.Texture2D; } }
        public static Texture2D PlaySmall { get { return Instance.play_small.Texture2D; } }
        public static Texture2D ProcessNodeOff { get { return Instance.process_node_off.Texture2D; } }
        public static Texture2D ProcessNodeOn { get { return Instance.process_node_on.Texture2D; } }
    }
}
