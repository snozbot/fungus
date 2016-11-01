// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEditor;
using System;

namespace Fungus.EditorUtils 
{
    internal static class FungusEditorResources 
    {

        static FungusEditorResources() {
            GenerateSpecialTextures();
            LoadResourceAssets();
        }

        private enum ResourceName 
        {
            command_background = 0,
            choice_node_off,
            choice_node_on,
            process_node_off,
            process_node_on,
            event_node_off,
            event_node_on,
            play_big,
            play_small
        }
        
        private static string[] s_LightSkin = {
            "command_background",
            "choice_node_off",
            "choice_node_on",
            "process_node_off",
            "process_node_on",
            "event_node_off",
            "event_node_on",
            "play_big",
            "play_small"
        };

        private static string[] s_DarkSkin = {
            "command_background",
            "choice_node_off",
            "choice_node_on",
            "process_node_off",
            "process_node_on",
            "event_node_off",
            "event_node_on",
            "play_big",
            "play_small"
        };

        public static Texture2D texCommandBackground 
        {
            get { return s_Cached[(int)ResourceName.command_background]; }
        }

        public static Texture2D texEventNodeOn
        {
            get { return s_Cached[(int)ResourceName.event_node_on]; }
        }
        
        public static Texture2D texEventNodeOff
        {
            get { return s_Cached[(int)ResourceName.event_node_off]; }
        }

        public static Texture2D texProcessNodeOn
        {
            get { return s_Cached[(int)ResourceName.process_node_on]; }
        }
        
        public static Texture2D texProcessNodeOff
        {
            get { return s_Cached[(int)ResourceName.process_node_off]; }
        }

        public static Texture2D texChoiceNodeOn
        {
            get { return s_Cached[(int)ResourceName.choice_node_on]; }
        }
        
        public static Texture2D texChoiceNodeOff
        {
            get { return s_Cached[(int)ResourceName.choice_node_off]; }
        }

        public static Texture2D texPlayBig
        {
            get { return s_Cached[(int)ResourceName.play_big]; }
        }

        public static Texture2D texPlaySmall
        {
            get { return s_Cached[(int)ResourceName.play_small]; }
        }

        public static Texture2D texItemSplitter { get; private set; }
        
        private static void GenerateSpecialTextures() 
        {
            var splitterColor = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.14f)
                    : new Color(0.59f, 0.59f, 0.59f, 0.55f)
                    ;
            texItemSplitter = CreatePixelTexture("(Generated) Item Splitter", splitterColor);
        }
        
        public static Texture2D CreatePixelTexture(string name, Color color) 
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
            tex.name = name;
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private static Texture2D[] s_Cached;
        
        public static void LoadResourceAssets() 
        {
            var skin = EditorGUIUtility.isProSkin ? s_DarkSkin : s_LightSkin;
            s_Cached = new Texture2D[skin.Length];
            
            for (int i = 0; i < s_Cached.Length; ++i)
            {
                s_Cached[i] = Resources.Load("Textures/" + skin[i]) as Texture2D;
            }
            
            s_LightSkin = null;
            s_DarkSkin = null;
        }
        
        private static void GetImageSize(byte[] imageData, out int width, out int height) 
        {
            width = ReadInt(imageData, 3 + 15);
            height = ReadInt(imageData, 3 + 15 + 2 + 2);
        }
        
        private static int ReadInt(byte[] imageData, int offset) 
        {
            return (imageData[offset] << 8) | imageData[offset + 1];
        }
    }
}