//-----------------------------------------------------------------------
// <summary>
// Unity Editor tool that formats line endings to be consistent.
// </summary>
// <copyright file="LineEndingsEditMenu.cs" company="Tiaan.com">
// Copyright (c) 2014 Tiaan Geldenhuys
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------
namespace TiaanDotCom.Unity3D.EditorTools
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Implements menu items for the Unity Editor to perform
    /// end-of-line conversion and fix issues such as for the
    /// following: "There are inconsistent line endings in the
    /// 'Assets/.../*.cs' script. Some are Mac OS X (UNIX) and
    /// some are Windows. This might lead to incorrect line
    /// numbers in stacktraces and compiler errors."
    /// </summary>
    public static class LineEndingsEditMenu
    {
//        [MenuItem("Tools/Fungus/Utilities/EOL Conversion (Windows)")]
//        public static void ConvertLineEndingsToWindowsFormat()
//        {
//            ConvertLineEndings(false);
//        }

        [MenuItem("Tools/Fungus/Utilities/Convert to Mac Line Endings")]
        public static void ConvertLineEndingsToMacFormat()
        {
            ConvertLineEndings(true);
        }

        private static void ConvertLineEndings(bool isUnixFormat)
        {
            string title = string.Format(
                "EOL Conversion to {0} Format",
                (isUnixFormat ? "UNIX" : "Windows"));
            if (!EditorUtility.DisplayDialog(
                title,
                "This operation may potentially modify " +
                "many files in the current project! " +
                "Hopefully you have backups of everything. " +
                "Are you sure you want to proceed?",
                "Yes",
                "No"))
            {
                Debug.Log("EOL Conversion was not attempted.");
                return;
            }

            var fileTypes = new string[]
            {
                "*.txt",
                "*.cs",
                "*.js",
                "*.boo",
                "*.compute",
                "*.shader",
                "*.cginc",
                "*.glsl",
                "*.xml",
                "*.xaml",
                "*.json",
                "*.inc",
                "*.css",
                "*.htm",
                "*.html",
            };

            string projectAssetsPath = Application.dataPath;
            int totalFileCount = 0;
            var changedFiles = new List<string>();
            var regex = new Regex(@"(?<!\r)\n");
            const string LineEnd = "\r\n";
            var comparisonType = System.StringComparison.Ordinal;
            foreach (string fileType in fileTypes)
            {
                string[] filenames = Directory.GetFiles(
                    projectAssetsPath,
                    fileType,
                    SearchOption.AllDirectories);
                totalFileCount += filenames.Length;
                foreach (string filename in filenames)
                {
                    string originalText = File.ReadAllText(filename);
                    string changedText;
                    changedText = regex.Replace(originalText, LineEnd);
                    if (isUnixFormat)
                    {
                        changedText =
                            changedText.Replace(LineEnd, "\n");
                    }

                    bool isTextIdentical = string.Equals(
                        changedText, originalText, comparisonType);
                    if (!isTextIdentical)
                    {
                        changedFiles.Add(filename);
                        File.WriteAllText(
                            filename,
                            changedText,
                            System.Text.Encoding.UTF8);
                    }
                }
            }

            int changedFileCount = changedFiles.Count;
            int skippedFileCount = (totalFileCount - changedFileCount);
            string message = string.Format(
                "EOL Conversion skipped {0} " +
                "files and changed {1} files",
                skippedFileCount,
                changedFileCount);
            if (changedFileCount <= 0)
            {
                message += ".";
            }
            else
            {
                message += (":" + LineEnd);
                message += string.Join(LineEnd, changedFiles.ToArray());
            }

            Debug.Log(message);
            if (changedFileCount > 0)
            {
                // Recompile modified scripts.
                AssetDatabase.Refresh();
            }
        }
    }
}
