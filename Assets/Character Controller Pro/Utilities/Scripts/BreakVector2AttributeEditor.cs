#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Lightbug.Utilities
{
    [CustomPropertyDrawer(typeof(BreakVector2Attribute))]
    public class BreakVector2AttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            var at = attribute as BreakVector2Attribute;

            EditorGUI.BeginChangeCheck();
            
            float x = EditorGUI.FloatField(fieldRect, at.XLabel, property.vector2Value.x);

            fieldRect.y += fieldRect.height + 2f;
            float y = EditorGUI.FloatField(fieldRect, at.YLabel, property.vector2Value.y);

            property.vector2Value = new Vector2(x, y);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2f * (EditorGUIUtility.singleLineHeight + 2f);
        }
    }
}

#endif
