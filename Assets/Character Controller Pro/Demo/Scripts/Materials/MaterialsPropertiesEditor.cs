using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;

namespace Lightbug.CharacterControllerPro.Demo
{

    [CustomEditor(typeof(MaterialsProperties))]
    public class MaterialsPropertiesEditor : Editor
    {
        ReorderableList surfacesList = null;
        ReorderableList volumesList = null;

        SerializedProperty defaultSurface = null;
        SerializedProperty surfaceAccelerationMultiplier = null;
        SerializedProperty surfaceDecelerationMultiplier = null;
        SerializedProperty surfaceSpeedMultiplier = null;

        SerializedProperty defaultVolume = null;
        SerializedProperty volumeAccelerationMultiplier = null;
        SerializedProperty volumeDecelerationMultiplier = null;
        SerializedProperty volumeSpeedMultiplier = null;
        SerializedProperty volumeGravityPositiveMultiplier = null;
        SerializedProperty volumeGravityNegativeMultiplier = null;



        SerializedProperty surfaces = null;
        SerializedProperty volumes = null;


        void OnEnable()
        {
            defaultSurface = serializedObject.FindProperty("defaultSurface");
            surfaceAccelerationMultiplier = defaultSurface.FindPropertyRelative("accelerationMultiplier");
            surfaceDecelerationMultiplier = defaultSurface.FindPropertyRelative("decelerationMultiplier");
            surfaceSpeedMultiplier = defaultSurface.FindPropertyRelative("speedMultiplier");

            defaultVolume = serializedObject.FindProperty("defaultVolume");
            volumeAccelerationMultiplier = defaultVolume.FindPropertyRelative("accelerationMultiplier");
            volumeDecelerationMultiplier = defaultVolume.FindPropertyRelative("decelerationMultiplier");
            volumeSpeedMultiplier = defaultVolume.FindPropertyRelative("speedMultiplier");
            volumeGravityPositiveMultiplier = defaultVolume.FindPropertyRelative("gravityPositiveMultiplier");
            volumeGravityNegativeMultiplier = defaultVolume.FindPropertyRelative("gravityNegativeMultiplier");


            surfaces = serializedObject.FindProperty("surfaces");
            volumes = serializedObject.FindProperty("volumes");

            surfacesList = new ReorderableList(
                serializedObject, surfaces,
                true,
                false,
                true,
                true
            );

            volumesList = new ReorderableList(
                serializedObject, volumes,
                true,
                false,
                true,
                true
            );

            volumes.isExpanded = true;
            volumesList.elementHeight = 10 * EditorGUIUtility.singleLineHeight;
            volumesList.headerHeight = 0f;

            surfaces.isExpanded = true;
            surfacesList.elementHeight = 8 * EditorGUIUtility.singleLineHeight;
            surfacesList.headerHeight = 0f;

            volumesList.drawElementCallback += OnDrawElementVolumes;
            surfacesList.drawElementCallback += OnDrawElementSurfaces;
        }



        void OnDisable()
        {
            volumesList.drawElementCallback -= OnDrawElementVolumes;
            surfacesList.drawElementCallback -= OnDrawElementSurfaces;
        }


        void OnDrawElementVolumes(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect fieldRect = rect;
            fieldRect.height = EditorGUIUtility.singleLineHeight;



            SerializedProperty item = volumes.GetArrayElementAtIndex(index);
            item.isExpanded = true;

            SerializedProperty itr = item.Copy();

            EditorGUI.LabelField(fieldRect, itr.FindPropertyRelative("tagName").stringValue);


            itr.Next(true);

            fieldRect.y += 1.5f * fieldRect.height;

            //bool enterChildren = true;   

            EditorGUI.PropertyField(fieldRect, itr, false);

            int children = item.CountInProperty() - 1;
            for (int i = 0; i < children; i++)
            {
                EditorGUI.PropertyField(fieldRect, itr, false);
                itr.Next(false);
                fieldRect.y += fieldRect.height;
            }

        }

        void OnDrawElementSurfaces(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect fieldRect = rect;
            fieldRect.height = EditorGUIUtility.singleLineHeight;



            SerializedProperty item = surfaces.GetArrayElementAtIndex(index);
            item.isExpanded = true;

            SerializedProperty itr = item.Copy();

            EditorGUI.LabelField(fieldRect, itr.FindPropertyRelative("tagName").stringValue);

            itr.Next(true);

            fieldRect.y += 1.5f * fieldRect.height;

            //bool enterChildren = true;   

            EditorGUI.PropertyField(fieldRect, itr, false);

            int children = item.CountInProperty() - 1;
            for (int i = 0; i < children; i++)
            {
                EditorGUI.PropertyField(fieldRect, itr, false);
                itr.Next(false);
                fieldRect.y += fieldRect.height;
            }


        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomUtilities.DrawScriptableObjectField<MaterialsProperties>((MaterialsProperties)target);

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray, 8);
            EditorGUILayout.LabelField("Default material", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("A default material parameter corresponds to any ground or spatial volume without a specific \"material tag\". " +
            "A Surface affects grounded movement, while a Volume affects not grounded movement.", MessageType.Info);
            GUILayout.Space(10);

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray);

            EditorGUILayout.LabelField("Default surface", EditorStyles.boldLabel);
            CustomUtilities.DrawArrayElement(defaultSurface, null, true);

            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray);

            EditorGUILayout.LabelField("Default volume", EditorStyles.boldLabel);
            CustomUtilities.DrawArrayElement(defaultVolume, null, true);

            // --------------------------------------------------------------------------------------------------------

            GUILayout.Space(10);



            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray);


            EditorGUILayout.LabelField("Tagged materials", EditorStyles.boldLabel);
            GUILayout.Space(10);


            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray, 8);
            EditorGUILayout.LabelField("Surfaces", EditorStyles.boldLabel);

            CustomUtilities.DrawArray(surfaces, "tagName");


            CustomUtilities.DrawEditorLayoutHorizontalLine(Color.gray, 8);

            EditorGUILayout.LabelField("Volumes", EditorStyles.boldLabel);

            CustomUtilities.DrawArray(volumes, "tagName");




            serializedObject.ApplyModifiedProperties();
        }





    }

}

#endif
