using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.Utilities
{
    public class ExpandAttribute : PropertyAttribute
    {
        public ExpandAttribute() { }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ExpandAttribute))]
    public class ExpandAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }

#endif

}



