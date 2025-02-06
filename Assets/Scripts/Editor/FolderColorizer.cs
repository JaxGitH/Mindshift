using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FolderColorizer : EditorWindow
{
    private static readonly string saveFilePath = Path.Combine(Application.dataPath, "Scripts/Editor/FolderColors.json");
    private static Dictionary<string, Color> folderColors = new Dictionary<string, Color>();
    private static string selectedFolderGuid;
    private static Color selectedColor = Color.white;

    [MenuItem("Assets/Set Folder Color", false, 1100)]
    private static void OpenColorPicker()
    {
        if (Selection.assetGUIDs.Length > 0)
        {
            selectedFolderGuid = Selection.assetGUIDs[0];
            string path = AssetDatabase.GUIDToAssetPath(selectedFolderGuid);

            if (AssetDatabase.IsValidFolder(path))
            {
                FolderColorizer window = GetWindow<FolderColorizer>("Set Folder Color");
                window.minSize = new Vector2(300, 200);

                if (folderColors.TryGetValue(selectedFolderGuid, out Color color))
                {
                    selectedColor = color;
                }
                else
                {
                    selectedColor = Color.white;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Selection", "Please select a folder to colorize.", "OK");
            }
        }
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(selectedFolderGuid))
        {
            EditorGUILayout.LabelField("No folder selected.", EditorStyles.boldLabel);
            return;
        }

        EditorGUILayout.LabelField("Select a color for the folder:", EditorStyles.boldLabel);
        selectedColor = EditorGUILayout.ColorField("Folder Color", selectedColor);

        if (GUILayout.Button("Apply Color"))
        {
            folderColors[selectedFolderGuid] = selectedColor;
            SaveFolderColors();
            AssetDatabase.Refresh();
            Close();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Use the Reset All Colors button to clear all folder colors.", MessageType.Info);

        if (GUILayout.Button("Reset All Colors"))
        {
            if (EditorUtility.DisplayDialog(
                    "Reset All Folder Colors",
                    "Are you sure you want to reset all folder colors? This action cannot be undone.",
                    "Yes", "No"))
            {
                ResetAllColors();
            }
        }
    }

    [InitializeOnLoadMethod]
    private static void Init()
    {
        LoadFolderColors();
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        // Check if the asset is a folder
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (AssetDatabase.IsValidFolder(path))
        {
            // Check if this folder has a color set
            if (folderColors.TryGetValue(guid, out Color color))
            {
                // Get the default folder icon
                Texture2D folderIcon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;

                // Determine if the folder is "small" (e.g., list view or minimal icon size)
                bool isSmallFolder = selectionRect.height < 20; // Example threshold for "small folder"

                if (folderIcon != null)
                {
                    // Adjust the icon rect based on whether it's a small folder
                    Rect iconRect;
                    if (isSmallFolder)
                    {
                        // Small folder (list view or minimal icon size)
                        iconRect = new Rect(
                            selectionRect.x,                  // Align left
                            selectionRect.y,              // Slight offset for small folder
                            selectionRect.height,         // Adjust size for small folder
                            selectionRect.height          // Adjust size for small folder
                        );
                    }
                    else
                    {
                        // Normal folder (grid view or larger icon size)
                        iconRect = new Rect(
                            selectionRect.x,                  // Align left
                            selectionRect.y,                  // Default alignment
                            selectionRect.width,              // Match selectionRect width
                            selectionRect.height - 14         // Slight height reduction for alignment
                        );
                    }

                    // Apply the color tint and draw the folder icon
                    GUI.color = color; // Apply tint
                    GUI.DrawTexture(iconRect, folderIcon, ScaleMode.ScaleToFit);
                    GUI.color = Color.white; // Reset color to avoid affecting other UI elements
                }

                // Draw the folder name manually, using separate handling for small and normal folders
                string folderName = Path.GetFileName(path);

                Rect labelRect;
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);

                int fontSize = Mathf.Clamp(Mathf.RoundToInt(selectionRect.height * 0.1f), 1, 10);

                if (isSmallFolder)
                {
                    // Label for small folders
                    labelRect = new Rect(
                        selectionRect.x,                // Offset slightly to the right
                        selectionRect.y,                     // Align with small folder icon
                        selectionRect.width,            // Adjust width for small size
                        16                                   // Default small label height
                    );
                    labelStyle.alignment = TextAnchor.MiddleLeft; // Left-align for small folders
                    labelStyle.fontSize = 10;                   // Optional: Adjust font size for small folders
                }
                else
                {
                    // Label for normal folders
                    labelRect = new Rect(
                        selectionRect.x,                           // Align horizontally with the folder icon
                        selectionRect.y,
                        //+ selectionRect.height - 16, // Position label below the icon
                        selectionRect.width,                       // Use the full width of the selection rect
                        selectionRect.height                                         // Default label height
                    );
                    labelStyle.alignment = TextAnchor.LowerLeft; // Center-align for normal folders
                    labelStyle.fontSize = 1;                      // Optional: Adjust font size for normal folders
                }

                // Draw the label
                EditorGUI.LabelField(labelRect, " ", labelStyle);
            }
        }
    }




    private static void SaveFolderColors()
    {
        string directory = Path.GetDirectoryName(saveFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        List<FolderColorData> colorData = new List<FolderColorData>();
        foreach (var pair in folderColors)
        {
            colorData.Add(new FolderColorData { guid = pair.Key, color = pair.Value });
        }

        string json = JsonUtility.ToJson(new FolderColorList { colors = colorData }, true);
        File.WriteAllText(saveFilePath, json);
    }

    private static void LoadFolderColors()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            FolderColorList colorList = JsonUtility.FromJson<FolderColorList>(json);

            folderColors.Clear();
            foreach (var colorData in colorList.colors)
            {
                folderColors[colorData.guid] = colorData.color;
            }
        }
    }

    private static void ResetAllColors()
    {
        folderColors.Clear();

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        AssetDatabase.Refresh();
    }

    [System.Serializable]
    private class FolderColorData
    {
        public string guid;
        public Color color;
    }

    [System.Serializable]
    private class FolderColorList
    {
        public List<FolderColorData> colors;
    }
}


