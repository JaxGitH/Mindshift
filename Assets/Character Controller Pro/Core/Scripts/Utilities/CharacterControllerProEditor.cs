#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public class CharacterControllerProEditor : Editor
    {
        public static string GetAssetRootFolder()
        {
            // Find the assembly definition file
            string[] guids = AssetDatabase.FindAssets("com.lightbug.character-controller-pro");

            if (guids.Length > 0)
            {
                // Look for a parent directory called "Character Controller Pro"
                string asmdefFilePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                return GetParentPathByFile("Character Controller Pro", asmdefFilePath);
            }

            return null;
        }

        #region Utilities

        public static string GetParentPathByFile(string targetFolderName, string initialFilePath)
        {
            // Start by finding the assembly definition file
            string[] guids = AssetDatabase.FindAssets("com.lightbug.character-controller-pro");

            if (guids.Length > 1)
            {
                Debug.Log("Multiple \"com.lightbug.character-controller-pro\" assembly definition files found.");
            }
            else if (guids.Length > 0)
            {
                // ──────────────────────────────────────────────────────────────────────────
                // path = "FolderA/FolderB/file.txt"
                // Path.GetFileName(path) --> "file.txt"
                // Path.GetDirectoryName(path) --> "FolderA/FolderB"
                // ──────────────────────────────────────────────────────────────────────────

                string currentDirectory = Path.GetDirectoryName(initialFilePath);

                while (!string.IsNullOrEmpty(currentDirectory))
                {
                    string folderName = Path.GetFileName(currentDirectory);

                    if (folderName == targetFolderName)
                        return currentDirectory;
                    else
                        currentDirectory = Path.GetDirectoryName(currentDirectory);
                }
            }
            else   // No file? :(
            {
                Debug.Log("No assembly definition file found. Have you deleted the file?.");
            }

            Debug.Log($"Error: Parent folder \"{targetFolderName}\" not found");

            return null;
        }

        public static void MoveFolderToFolder(string sourceFolderPath, string destinationFolderPath)
        {
            // Source folder
            if (!Directory.Exists(sourceFolderPath))
            {
                Debug.Log($"Error: Source directory {sourceFolderPath} doesn't exist");
                return;
            }

            // Find the meta file
            string sourceMetaPath = sourceFolderPath + ".meta";
            if (!File.Exists(sourceMetaPath))
            {
                Debug.Log($"Error: Meta file {sourceMetaPath} doesn't exist");
                return;
            }

            // Destination folder
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            Directory.Move(sourceFolderPath, $"{destinationFolderPath}/{Path.GetFileName(sourceFolderPath)}");
            File.Move(sourceMetaPath, $"{destinationFolderPath}/{Path.GetFileName(sourceMetaPath)}");

            AssetDatabase.Refresh();
        }

        public static void MoveFileToFolder(string sourceFilePath, string destinationFolderPath)
        {
            // Source folder
            if (!File.Exists(sourceFilePath))
            {
                Debug.Log($"Error: Source file {sourceFilePath} doesn't exist");
                return;
            }

            // Find the meta file
            string sourceMetaPath = sourceFilePath + ".meta";
            if (!File.Exists(sourceMetaPath))
            {
                Debug.Log($"Error: Meta file {sourceMetaPath} doesn't exist");
                return;
            }

            // Destination folder
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            File.Move(sourceFilePath, $"{destinationFolderPath}/{Path.GetFileName(sourceFilePath)}");
            File.Move(sourceMetaPath, $"{destinationFolderPath}/{Path.GetFileName(sourceMetaPath)}");

            AssetDatabase.Refresh();
        }

        #endregion

        [MenuItem("Window/Character Controller Pro/Welcome", false, 0)]
        public static void WelcomeMessage()
        {
            WelcomeWindow window = EditorWindow.GetWindow<WelcomeWindow>(true, "Welcome");
        }

        [MenuItem("Window/Character Controller Pro/Documentation", false, 1)]
        public static void Documentation()
        {
            Application.OpenURL("https://lightbug14.gitbook.io/ccp/");
        }

        [MenuItem("Window/Character Controller Pro/API Reference", false, 2)]
        public static void APIReference()
        {
            Application.OpenURL("https://lightbug14.github.io/lightbug-web/character-controller-pro/Documentation/html/index.html");
        }

        [MenuItem("Window/Character Controller Pro/About", false, 100)]
        public static void About()
        {
            EditorWindow.GetWindow<AboutWindow>(true, "About");
        }
                
    
        /*public static void UseOldStructure(string rootPath)
        {
            if (!Directory.Exists($"{rootPath}/Main"))
            {
                Debug.Log("Error: The current folder structure is the old one.");
                return;
            }

            MoveFolderToFolder($"{rootPath}/Main/Core", $"{rootPath}");
            MoveFolderToFolder($"{rootPath}/Main/Documentation", $"{rootPath}");
            MoveFolderToFolder($"{rootPath}/Main/Implementation", $"{rootPath}");
            MoveFolderToFolder($"{rootPath}/Main/Utilities", $"{rootPath}");
            MoveFileToFolder($"{rootPath}/Main/com.lightbug.character-controller-pro.asmdef", $"{rootPath}");
            Directory.Delete($"{rootPath}/Main");
            File.Delete($"{rootPath}/Main.meta");

            AssetDatabase.Refresh();
        }

        [MenuItem("Window/Character Controller Pro/Use old (1.4.10 or lower) folder structure", false, 0)]
        static void UseOldStructure()
        {
            if (!IsCurrentStructureNew())
            {
                EditorUtility.DisplayDialog(
                    "Old folder structure detected",
                    "The current folder structure your project is using corresponds to the old one",
                    "Ok"
                    );
                return;
            }

            bool result = EditorUtility.DisplayDialog(
                "Folder structure",
                "This will" +
                "\n\nFrom 1.4.11 onwards, all the demo content locates outside the main assembly by separating the asset into \"Main\" and \"Demo\". " +
                "Do you want this tool to update this structure for you?",
                "Yes (recommended)",
                "No"
            );

            if (result)
            {
                var rootPath = GetAssetRootFolder();
                UseOldStructure(rootPath);
            }
        }*/

        /*static void UseNewStructure(string rootPath)
        {            
            if (Directory.Exists($"{rootPath}/Main"))
            {
                Debug.Log("Error: The current folder structure is the new one.");
                return;
            }

            MoveFolderToFolder($"{rootPath}/Core", $"{rootPath}/Main");
            MoveFolderToFolder($"{rootPath}/Documentation", $"{rootPath}/Main");
            MoveFolderToFolder($"{rootPath}/Implementation", $"{rootPath}/Main");
            MoveFolderToFolder($"{rootPath}/Utilities", $"{rootPath}/Main");
            MoveFileToFolder($"{rootPath}/com.lightbug.character-controller-pro.asmdef", $"{rootPath}/Main");
        }

        [MenuItem("Window/Character Controller Pro/Use new (1.4.11 or higher) folder structure", false, 0)]
        static void UseNewStructure()
        {
            var rootPath = GetAssetRootFolder();
            UseNewStructure(rootPath);
        }*/

        public static bool IsCurrentStructureNew(string rootPath) => Directory.Exists($"{rootPath}/Main");

        public static bool IsCurrentStructureNew() => IsCurrentStructureNew(GetAssetRootFolder());

    }

}

#endif
