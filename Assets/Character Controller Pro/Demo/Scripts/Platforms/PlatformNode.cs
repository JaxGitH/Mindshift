using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

#if UNITY_EDITOR
    using UnityEditor;

#endif

    [System.Serializable]
    public class PlatformNode
    {

        public Vector3 position = Vector3.zero;
        public Vector3 eulerAngles = Vector3.zero;

        public AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Min(0f)]
        public float targetTime = 1;

        public void Initialize()
        {
            position = Vector3.zero;
            eulerAngles = Vector3.zero;
            movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
            rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

            targetTime = 1;
        }

    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(PlatformNode))]
    public class PlatformNodeDrawer : PropertyDrawer
    {
        SerializedProperty position = null;
        SerializedProperty eulerAngles = null;
        SerializedProperty movementCurve = null;
        SerializedProperty rotationCurve = null;
        SerializedProperty targetTime = null;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            this.position = property.FindPropertyRelative("position");
            this.eulerAngles = property.FindPropertyRelative("eulerAngles");
            this.movementCurve = property.FindPropertyRelative("movementCurve");
            this.rotationCurve = property.FindPropertyRelative("rotationCurve");
            this.targetTime = property.FindPropertyRelative("targetTime");

            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;


            EditorGUI.PropertyField(fieldRect, this.position);
            fieldRect.y += fieldRect.height;

            EditorGUI.PropertyField(fieldRect, this.eulerAngles);
            fieldRect.y += fieldRect.height;

            EditorGUI.PropertyField(fieldRect, this.movementCurve);
            fieldRect.y += fieldRect.height;

            EditorGUI.PropertyField(fieldRect, this.rotationCurve);
            fieldRect.y += fieldRect.height;

            EditorGUI.PropertyField(fieldRect, this.targetTime);
            fieldRect.y += fieldRect.height;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 5 * EditorGUIUtility.singleLineHeight;
        }

    }


#endif

}
