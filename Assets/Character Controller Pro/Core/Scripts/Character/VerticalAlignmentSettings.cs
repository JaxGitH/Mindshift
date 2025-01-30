using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Core
{

    [System.Serializable]
    public class VerticalAlignmentSettings
    {
        [Tooltip("By assigning this object, the character up direction will be automatically calculated based on it. " +
        "A null value means that the character up direction will be the one defined in the \"alignment direction\" field")]
        public Transform alignmentReference = null;

        [Tooltip("The mode defines how the up direction is calculated (alignment reference not null).")]
        public VerticalReferenceMode referenceMode = VerticalReferenceMode.Away;

        [Tooltip("The desired up direction (null alignment reference).")]
        public Vector3 alignmentDirection = Vector3.up;

        public enum VerticalReferenceMode
        {
            Towards,
            Away
        }

    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(VerticalAlignmentSettings))]
    public class VerticalAlignmentSettingsPropertyDrawer : PropertyDrawer
    {
        const float verticalSpaceMultiplier = 1.1f;

        SerializedProperty alignmentDirection = null;
        SerializedProperty alignmentReference = null;
        SerializedProperty referenceMode = null;

        void FindProperties(SerializedProperty property)
        {
            alignmentReference = property.FindPropertyRelative("alignmentReference");
            alignmentDirection = property.FindPropertyRelative("alignmentDirection");
            referenceMode = property.FindPropertyRelative("referenceMode");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            FindProperties(property);


            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            property.isExpanded = true;



            EditorGUI.PropertyField(fieldRect, alignmentReference);
            fieldRect.y += verticalSpaceMultiplier * fieldRect.height;

            if (alignmentReference.objectReferenceValue != null)
            {
                EditorGUI.PropertyField(fieldRect, referenceMode);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, alignmentDirection);

            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float output = 2f * verticalSpaceMultiplier * EditorGUIUtility.singleLineHeight;

            // if( property.isExpanded )
            // {
            //     FindProperties( property );

            //     if( alignmentReference.objectReferenceValue != null )
            //     {
            //         output += verticalSpaceMultiplier * EditorGUIUtility.singleLineHeight; 
            //     }
            // }


            return output;
        }
    }

#endif

}
