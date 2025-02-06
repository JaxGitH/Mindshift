#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

    [CustomEditor(typeof(CharacterActionsAsset))]
    public class CharacterActionsAssetEditor : Editor
    {
        const string CharacterActionsFileName = "CharacterActions";
        const string TemplateFileName = "template-character-actions";

        SerializedProperty boolActions = null;
        SerializedProperty floatActions = null;
        SerializedProperty vector2Actions = null;


        void OnEnable()
        {
            boolActions = serializedObject.FindProperty("boolActions");
            floatActions = serializedObject.FindProperty("floatActions");
            vector2Actions = serializedObject.FindProperty("vector2Actions");
        }

        public override void OnInspectorGUI()
        {
            CustomUtilities.DrawScriptableObjectField((CharacterActionsAsset)target);

            serializedObject.Update();

            EditorGUILayout.PropertyField(boolActions, true);
            EditorGUILayout.PropertyField(floatActions, true);
            EditorGUILayout.PropertyField(vector2Actions, true);

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray);

            EditorGUILayout.HelpBox(
                "Click the button to replace the original \"CharacterActions.cs\" file. This can be useful if you need to create custom actions, without modifing the code. ",
                MessageType.Info
            );

            if (GUILayout.Button("Create actions"))
            {       
                bool result = EditorUtility.DisplayDialog(
                    "Create actions",
                    "Warning: This will replace the original \"CharacterActions\" file. Are you sure you want to continue?", "Yes", "No");

                if (result)
                {
                    string characterActionsPath = null;
                    string templatePath = null;

                    string[] characterActionsResults = AssetDatabase.FindAssets(CharacterActionsFileName + " t:script");
                    if (characterActionsResults.Length != 0)
                        characterActionsPath = AssetDatabase.GUIDToAssetPath(characterActionsResults[0]);

                    string[] templateResults = AssetDatabase.FindAssets(TemplateFileName);

                    if (templateResults.Length != 0)
                        templatePath = AssetDatabase.GUIDToAssetPath(templateResults[0]);

                    CreateCSharpClass(characterActionsPath, templatePath);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CreateCSharpClass(string characterActionsPath, string templatePath)
        {
            if (characterActionsPath == null || templatePath == null)
                return;

            string output = GenerateOutput(CharacterActionsFileName, templatePath);

            FileStream fileStream = File.Open(characterActionsPath, FileMode.Truncate, FileAccess.ReadWrite);

            StreamWriter file = new StreamWriter(fileStream);

            file.Write(output);
            file.Close();

            AssetDatabase.Refresh();
        }


        string GenerateOutput(string className, string templatePath)
        {            
            StreamReader reader = new StreamReader(templatePath);

            string output = reader.ReadToEnd();
            reader.Close();

            output = Regex.Replace(output, @"@\s*struct-name\s*@", CharacterActionsFileName);

            // -----------------------------------------------------------------------------------------------------------------------------------
            // Bool Actions ----------------------------------------------------------------------------------------------------------------------
            // -----------------------------------------------------------------------------------------------------------------------------------

            string definitionsString = "";
            string resetString = "";
            string newString = "";
            string setValueString = "";
            string copyValueString = "";
            string updateString = "";

            for (int i = 0; i < boolActions.arraySize; i++)
            {
                string actionName = boolActions.GetArrayElementAtIndex(i).stringValue;
                if (actionName.IsNullOrEmpty())
                    continue;

                string[] words = actionName.Split(' ');

                string variableName = "@";
                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];

                    if (j == 0)
                        variableName += System.Char.ToLowerInvariant(word[0]) + word.Substring(1).ToLower();
                    else
                        variableName += System.Char.ToUpperInvariant(word[0]) + word.Substring(1).ToLower();
                }


                definitionsString += "\tpublic BoolAction " + variableName + ";\n";
                resetString += "\t\t" + variableName + ".Reset();\n";
                newString += "\t\t" + variableName + " = new BoolAction();\n" +
                    "\t\t" + variableName + ".Initialize();\n\n";
                setValueString += "\t\t" + variableName + ".value = inputHandler.GetBool( \"" + actionName + "\" );\n";
                copyValueString += "\t\t" + variableName + ".value = characterActions." + variableName.Substring(1) + ".value;\n";
                updateString += "\t\t" + variableName + ".Update( dt );\n";
            }

            // Write bool actions
            output = Regex.Replace(output, @"@\s*bool-actions-definitions\s*@", definitionsString);
            output = Regex.Replace(output, @"@\s*bool-actions-reset\s*@", resetString);
            output = Regex.Replace(output, @"@\s*bool-actions-new\s*@", newString);
            output = Regex.Replace(output, @"@\s*bool-actions-setValue\s*@", setValueString);
            output = Regex.Replace(output, @"@\s*bool-actions-copyValue\s*@", copyValueString);
            output = Regex.Replace(output, @"@\s*bool-actions-update\s*@", updateString);

            // -----------------------------------------------------------------------------------------------------------------------------------
            // Float Actions ---------------------------------------------------------------------------------------------------------------------
            // -----------------------------------------------------------------------------------------------------------------------------------

            definitionsString = "";
            resetString = "";
            newString = "";
            setValueString = "";
            copyValueString = "";
            updateString = "";
                        
            for (int i = 0; i < floatActions.arraySize; i++)
            {
                string actionName = floatActions.GetArrayElementAtIndex(i).stringValue;
                if (actionName.IsNullOrEmpty())
                    continue;

                string[] words = actionName.Split(' ');

                string variableName = "@";
                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];

                    if (j == 0)
                        variableName += System.Char.ToLowerInvariant(word[0]) + word.Substring(1).ToLower();
                    else
                        variableName += System.Char.ToUpperInvariant(word[0]) + word.Substring(1).ToLower();
                }


                definitionsString += "\tpublic FloatAction " + variableName + ";\n";
                resetString += "\t\t" + variableName + ".Reset();\n";
                newString += "\t\t" + variableName + " = new FloatAction();\n";
                setValueString += "\t\t" + variableName + ".value = inputHandler.GetFloat( \"" + actionName + "\" );\n";
                copyValueString += "\t\t" + variableName + ".value = characterActions." + variableName.Substring(1) + ".value;\n";
            }

            // Write bool actions
            output = Regex.Replace(output, @"@\s*float-actions-definitions\s*@", definitionsString);
            output = Regex.Replace(output, @"@\s*float-actions-reset\s*@", resetString);
            output = Regex.Replace(output, @"@\s*float-actions-new\s*@", newString);
            output = Regex.Replace(output, @"@\s*float-actions-setValue\s*@", setValueString);
            output = Regex.Replace(output, @"@\s*float-actions-copyValue\s*@", copyValueString);

            // -----------------------------------------------------------------------------------------------------------------------------------
            // Vector2 Actions -------------------------------------------------------------------------------------------------------------------
            // -----------------------------------------------------------------------------------------------------------------------------------

            definitionsString = "";
            resetString = "";
            newString = "";
            setValueString = "";
            updateString = "";

            for (int i = 0; i < vector2Actions.arraySize; i++)
            {
                string actionName = vector2Actions.GetArrayElementAtIndex(i).stringValue;
                if (actionName.IsNullOrEmpty())
                    continue;

                string[] words = actionName.Split(' ');

                string variableName = "@";
                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];

                    if (j == 0)
                        variableName += System.Char.ToLowerInvariant(word[0]) + word.Substring(1).ToLower();
                    else
                        variableName += System.Char.ToUpperInvariant(word[0]) + word.Substring(1).ToLower();
                }


                definitionsString += "\tpublic Vector2Action " + variableName + ";\n";
                resetString += "\t\t" + variableName + ".Reset();\n";
                newString += "\t\t" + variableName + " = new Vector2Action();\n";
                setValueString += "\t\t" + variableName + ".value = inputHandler.GetVector2( \"" + actionName + "\" );\n";
                copyValueString += "\t\t" + variableName + ".value = characterActions." + variableName.Substring(1) + ".value;\n";

            }

            // Write bool actions
            output = Regex.Replace(output, @"@\s*vector2-actions-definitions\s*@", definitionsString);
            output = Regex.Replace(output, @"@\s*vector2-actions-reset\s*@", resetString);
            output = Regex.Replace(output, @"@\s*vector2-actions-new\s*@", newString);
            output = Regex.Replace(output, @"@\s*vector2-actions-setValue\s*@", setValueString);
            output = Regex.Replace(output, @"@\s*vector2-actions-copyValue\s*@", copyValueString);

            return output;

        }

    }

}

#endif
