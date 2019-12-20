using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Helper functions for generating the markdown files for Fungus Commands and Events.
    /// </summary>
    public static class ExportReferenceDocs
    {
        private const string CommandRefDocPath = BaseDocPath + "command_ref/";
        private const string BaseDocPath = "./Docs/";

        [MenuItem("Tools/Fungus/Utilities/Export Reference Docs")]
        internal static void Export()
        {
            ExportCommandInfo();
            ExportEventHandlerInfo();

            FlowchartWindow.ShowNotification("Exported Reference Documentation");
        }

        private static void ExportCommandInfo()
        {
            // Dump command info
            List<System.Type> menuTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
            List<KeyValuePair<System.Type, CommandInfoAttribute>> filteredAttributes = BlockEditor.GetFilteredCommandInfoAttribute(menuTypes);
            filteredAttributes.Sort(BlockEditor.CompareCommandAttributes);

            // Build list of command categories
            List<string> commandCategories = new List<string>();
            foreach (var keyPair in filteredAttributes)
            {
                CommandInfoAttribute info = keyPair.Value;
                if (info.Category != "" &&
                    !commandCategories.Contains(info.Category))
                {
                    commandCategories.Add(info.Category);
                }
            }
            commandCategories.Sort();

            var sb = new System.Text.StringBuilder(@"# Command Reference

This is the reference documentation for all Fungus commands.

");
            // Output the commands in each category
            foreach (string category in commandCategories)
            {
                string markdown = "# " + category + " commands # {#" + category.ToLower() + "_commands}\n\n";
                markdown += "[TOC]\n";

                foreach (var keyPair in filteredAttributes)
                {
                    CommandInfoAttribute info = keyPair.Value;

                    if (info.Category == category ||
                        info.Category == "" && category == "Scripting")
                    {
                        markdown += "# " + info.CommandName + " # {#" + info.CommandName.Replace(" ", "") + "}\n";
                        markdown += info.HelpText + "\n\n";
                        markdown += "Defined in " + keyPair.Key.FullName + "\n";
                        markdown += GetPropertyInfo(keyPair.Key);
                    }
                }

                WriteFile(markdown, CommandRefDocPath, category.ToLower(), "_commands.md");
                sb.Append("* [").Append(category).Append("](").Append(category.ToLower()).AppendLine("_commands)");
            }

            sb.AppendLine();
            WriteFile(sb.ToString(), BaseDocPath, "", "command_reference.md");
        }

        private static void ExportEventHandlerInfo()
        {
            List<System.Type> eventHandlerTypes = EditorExtensions.FindDerivedTypes(typeof(EventHandler)).ToList();
            List<string> eventHandlerCategories = new List<string>();
            eventHandlerCategories.Add("Core");
            foreach (System.Type type in eventHandlerTypes)
            {
                EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);
                if (info != null &&
                    info.Category != "" &&
                    !eventHandlerCategories.Contains(info.Category))
                {
                    eventHandlerCategories.Add(info.Category);
                }
            }
            eventHandlerCategories.Sort();

            var sb = new System.Text.StringBuilder(@"# Event Handler Reference {#eventhandler_reference}

This is the reference documentation for all Fungus event handlers.

");

            // Output the commands in each category
            foreach (string category in eventHandlerCategories)
            {
                string markdown = "# " + category + " event handlers # {#" + category.ToLower() + "_events}\n\n";
                markdown += "[TOC]\n";

                foreach (System.Type type in eventHandlerTypes)
                {
                    EventHandlerInfoAttribute info = EventHandlerEditor.GetEventHandlerInfo(type);

                    if (info != null &&
                        (info.Category == category ||
                         (info.Category == "" && category == "Core")))
                    {
                        markdown += "# " + info.EventHandlerName + " # {#" + info.EventHandlerName.Replace(" ", "") + "}\n";
                        markdown += info.HelpText + "\n\n";
                        markdown += "Defined in " + type.FullName + "\n";
                        markdown += GetPropertyInfo(type);
                    }
                }

                WriteFile(markdown, CommandRefDocPath, category.ToLower(), "_events.md");
                sb.Append("* [").Append(category).Append("](").Append(category.ToLower()).AppendLine("_events)");
            }

            sb.AppendLine();
            WriteFile(sb.ToString(), BaseDocPath, "", "eventhandler_reference.md");
        }

        private static void WriteFile(string markdown, string path, string category, string suffix)
        {
            markdown += "Auto-Generated by Fungus.ExportReferenceDocs";

            string filePath = path + category.ToLower() + suffix;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, markdown);
        }

        private static string GetPropertyInfo(System.Type type)
        {
            string markdown = "";
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                TooltipAttribute attribute = (TooltipAttribute)Attribute.GetCustomAttribute(field, typeof(TooltipAttribute));
                if (attribute == null)
                {
                    continue;
                }

                // Change field name to how it's displayed in the inspector
                string propertyName = Regex.Replace(field.Name, "(\\B[A-Z])", " $1");
                if (propertyName.Length > 1)
                {
                    propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
                }
                else
                {
                    propertyName = propertyName.ToUpper();
                }

                markdown += propertyName + " | " + field.FieldType + " | " + attribute.tooltip + "\n";
            }

            if (markdown.Length > 0)
            {
                markdown = "\nProperty | Type | Description\n --- | --- | ---\n" + markdown + "\n";
            }

            return markdown;
        }

        [MenuItem("Tools/Fungus/Utilities/Convert Docs to GitHub MD")]
        internal static void ConvertAllToGHMD()
        {
            var files = Directory.GetFiles(BaseDocPath, "*.md", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                ConvertFileToGHMD(file);
            }

            FlowchartWindow.ShowNotification("Converted " + files.Length.ToString() + " to Github MD");
        }

        //strips anchor links and TOC, which are not supported on Github wiki mds
        private static void ConvertFileToGHMD(string fileLoc)
        {
            var sb = new System.Text.StringBuilder();
            var lines = File.ReadAllLines(fileLoc);

            foreach (var line in lines)
            {
                if (line.Length > 0 && line[0] == '#')
                {
                    var secondToken = line.IndexOf(" # ");
                    if (secondToken != -1)
                    {
                        var heading = line.Substring(0, secondToken);
                        sb.AppendLine(heading);
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
                else if (line == "[TOC]")
                {
                    //skip
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            File.WriteAllText(fileLoc, sb.ToString());
        }
    }
}