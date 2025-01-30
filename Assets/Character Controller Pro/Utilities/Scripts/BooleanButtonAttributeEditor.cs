#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Lightbug.Utilities
{
    [CustomPropertyDrawer(typeof(BooleanButtonAttribute))]
    public class BooleanButtonAttributeEditor : PropertyDrawer
    {
        const string ENABLED_STYLE_NAME = "flow node 2";
        const string DISABLED_STYLE_NAME = "flow node 0";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);


            var enabledStyle = new GUIStyle(EditorStyles.miniButton);
            enabledStyle.fontStyle = FontStyle.Bold;

            var disabledStyle = new GUIStyle(EditorStyles.miniButton);
            disabledStyle.fontStyle = FontStyle.Normal;
            var textColor = disabledStyle.normal.textColor;     textColor.a = 0.4f;
            disabledStyle.normal.textColor = textColor;

            var at = attribute as BooleanButtonAttribute;
            bool useLabel = at.Label != null;


            Rect fieldRect = position;

            if (useLabel)
            {
                fieldRect.width = EditorGUIUtility.labelWidth;
                EditorGUI.LabelField(fieldRect, at.Label);
                fieldRect.x += fieldRect.width;
                fieldRect.width = EditorGUIUtility.currentViewWidth - 36f - EditorGUIUtility.labelWidth;
            }

            fieldRect.width *= 0.5f;

            bool value = property.boolValue;
            if (at.FalseLabelFirst)
            {
                if (value)
                {
                    if (GUI.Button(fieldRect, at.FalseLabel, disabledStyle))
                        value = false;

                    fieldRect.x += fieldRect.width;

                    GUI.Button(fieldRect, at.TrueLabel, enabledStyle);
                }
                else
                {
                    GUI.Button(fieldRect, at.FalseLabel, enabledStyle);

                    fieldRect.x += fieldRect.width;

                    if (GUI.Button(fieldRect, at.TrueLabel, disabledStyle))
                        value = true;
                }
            }
            else
            {
                if (value)
                {
                    GUI.Button(fieldRect, at.TrueLabel, enabledStyle);

                    fieldRect.x += fieldRect.width;

                    if (GUI.Button(fieldRect, at.FalseLabel, disabledStyle))
                        value = false;
                }
                else
                {
                    if (GUI.Button(fieldRect, at.TrueLabel, disabledStyle))
                        value = true;

                    fieldRect.x += fieldRect.width;

                    GUI.Button(fieldRect, at.FalseLabel, enabledStyle);
                }
            }

            property.boolValue = value;

            EditorGUI.EndProperty();
        }

        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{

        //    return 1.2f * (EditorGUIUtility.singleLineHeight + 2f);
        //}
    }
}
#endif

