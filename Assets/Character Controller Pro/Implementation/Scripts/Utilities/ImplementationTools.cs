#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace Lightbug.CharacterControllerPro.Implementation
{

    /// <summary>
    /// Editor tools used by the "Implementation" part of Character Controller Pro.
    /// </summary>
    public class ImplementationTools : EditorWindow
    {
        const string TemplateTextFilePath = "Assets/Character Controller Pro/Implementation/Resources/template-character-state.txt";
        const string Namespace = "Lightbug.CharacterControllerPro.Implementation";

        string labelField = "";
        static UnityEngine.Object selectedObject = null;


        [MenuItem("Assets/Create/Character Controller Pro/Implementation/Character State")]
        static void Init()
        {
            selectedObject = Selection.activeObject;

            ImplementationTools window = ScriptableObject.CreateInstance<ImplementationTools>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 50);
            window.ShowPopup();
        }


        void OnGUI()
        {
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();

            GUILayout.Label("State Name ", EditorStyles.wordWrappedLabel);
            labelField = EditorGUILayout.TextField(labelField);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();


            if (GUILayout.Button("Create"))
            {

                string path = selectedObject != null ? AssetDatabase.GetAssetPath(selectedObject) : "Assets/";

                bool isAValidFolder = AssetDatabase.IsValidFolder(path);

                if (isAValidFolder)
                {
                    path = path + "/";

                }
                else
                {
                    // A file is selected, take the directory path
                    path = System.IO.Path.GetDirectoryName(path) + "/";

                    isAValidFolder = AssetDatabase.IsValidFolder(path);

                }


                if (!isAValidFolder)
                {
                    Debug.Log("Select a valid folder");
                    this.Close();
                }

                bool result = CreateState(labelField, path);

                if (result)
                    Debug.Log("State successfully created.");

                this.Close();

            }

            if (GUILayout.Button("Cancel"))
                this.Close();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        bool CreateState(string name, string path)
        {
            if (string.Equals(name, ""))
            {
                Debug.Log("Empty name");
                return false;
            }

            name = name.Replace(" ", "_");
            name = name.Replace("-", "_");

            string fullPath = path + name + ".cs";


            if (File.Exists(fullPath))
            {
                Debug.Log("File already exist!.");
                return false;
            }

            string output = ReplacePatterns(name, name);

            StreamWriter outfile = new StreamWriter(fullPath);
            outfile.Write(output);
            outfile.Close();

            AssetDatabase.Refresh();

            return true;

        }

        string ReplacePatterns(string className, string stateName)
        {
            StreamReader reader = new StreamReader(TemplateTextFilePath);

            string output = reader.ReadToEnd();
            reader.Close();

            output = Regex.Replace(output, @"@\s*namespace\s*@", Namespace);
            output = Regex.Replace(output, @"@\s*class\s*@", className);
            output = Regex.Replace(output, @"@\s*name\s*@", stateName);

            return output;

        }


    }

}

#endif