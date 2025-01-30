using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;

namespace Lightbug.CharacterControllerPro.Demo
{
    [CustomEditor(typeof(AISequenceBehaviour)), CanEditMultipleObjects]
    public class AISequenceBehaviourEditor : Editor
    {
        ReorderableList reorderableList = null;
        SerializedProperty actionSequence = null;

        void OnEnable()
        {
            actionSequence = serializedObject.FindProperty("actionSequence");

            reorderableList = new ReorderableList(
                serializedObject,
                actionSequence,
                true,
                true,
                true,
                true
            );

            reorderableList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;
            reorderableList.drawElementCallback += OnDrawElement;
            reorderableList.drawHeaderCallback += OnDrawHeader;
            reorderableList.elementHeightCallback += OnElementHeight;
        }

        void OnDisable()
        {
            reorderableList.drawElementCallback -= OnDrawElement;
            reorderableList.drawHeaderCallback -= OnDrawHeader;
            reorderableList.elementHeightCallback -= OnElementHeight;
        }

        void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Sequence");
        }

        void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = actionSequence.GetArrayElementAtIndex(index);

            SerializedProperty sequenceType = element.FindPropertyRelative("sequenceType");
            SerializedProperty duration = element.FindPropertyRelative("duration");
            SerializedProperty action = element.FindPropertyRelative("action");

            GUI.Box(rect, "", EditorStyles.helpBox);

            Rect fieldRect = rect;
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            fieldRect.x += 20;
            fieldRect.width -= 30;

            fieldRect.y += 0.5f * fieldRect.height;

            EditorGUI.PropertyField(fieldRect, sequenceType);
            fieldRect.y += 2 * fieldRect.height;

            if (sequenceType.enumValueIndex == (int)SequenceType.Duration)
                EditorGUI.PropertyField(fieldRect, duration);

            fieldRect.y += 2 * fieldRect.height;

            EditorGUI.PropertyField(fieldRect, action, true);
            fieldRect.y += fieldRect.height;
        }

        float OnElementHeight(int index)
        {
            SerializedProperty element = actionSequence.GetArrayElementAtIndex(index);
            SerializedProperty action = element.FindPropertyRelative("action");

            float actionHeight = action.isExpanded ? EditorGUI.GetPropertyHeight(action) : EditorGUIUtility.singleLineHeight;
            return 5 * EditorGUIUtility.singleLineHeight + actionHeight;

        }

        public override void OnInspectorGUI()
        {
            CustomUtilities.DrawMonoBehaviourField<AISequenceBehaviour>((AISequenceBehaviour)target);
            serializedObject.Update();

            GUILayout.Space(10);

            reorderableList.DoLayoutList();

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
