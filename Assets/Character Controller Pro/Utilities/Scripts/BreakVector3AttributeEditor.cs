#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Lightbug.Utilities
{
    [CustomPropertyDrawer(typeof(BreakVector3Attribute))]
    public class BreakVector3AttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            var at = attribute as BreakVector3Attribute;

            float x = EditorGUI.FloatField(fieldRect, at.XLabel, property.vector3Value.x);

            fieldRect.y += fieldRect.height + 2f;
            float y = EditorGUI.FloatField(fieldRect, at.YLabel, property.vector3Value.y);

            fieldRect.y += fieldRect.height + 2f;
            float z = EditorGUI.FloatField(fieldRect, at.ZLabel, property.vector3Value.z);

            property.vector3Value = new Vector3(x, y, z);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 3f * (EditorGUIUtility.singleLineHeight + 2f);
        }
    }
}

#endif
