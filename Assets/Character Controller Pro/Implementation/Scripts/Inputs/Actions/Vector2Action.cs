using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

    [System.Serializable]
    public struct Vector2Action
    {
        /// <summary>
        /// The action current value.
        /// </summary>
        public Vector2 value;

        /// <summary>
        /// Resets the action
        /// </summary>
        public void Reset()
        {
            value = Vector2.zero;
        }


        /// <summary>
        /// Returns true if the value is not equal to zero (e.g. When pressing a D-pad)
        /// </summary>
        public bool Detected
        {
            get
            {
                return value != Vector2.zero;
            }
        }

        /// <summary>
        /// Returns true if the x component is positive.
        /// </summary>
        public bool Right
        {
            get
            {
                return value.x > 0;
            }
        }

        /// <summary>
        /// Returns true if the x component is negative.
        /// </summary>
        public bool Left
        {
            get
            {
                return value.x < 0;
            }
        }

        /// <summary>
        /// Returns true if the y component is positive.
        /// </summary>
        public bool Up
        {
            get
            {
                return value.y > 0;
            }
        }

        /// <summary>
        /// Returns true if the y component is negative.
        /// </summary>
        public bool Down
        {
            get
            {
                return value.y < 0;
            }
        }


    }


    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR


    [CustomPropertyDrawer(typeof(Vector2Action))]
    public class Vector2ActionEditor : PropertyDrawer
    {


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty value = property.FindPropertyRelative("value");

            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            fieldRect.width = 100;

            EditorGUI.LabelField(fieldRect, label);

            fieldRect.x += 110;

            EditorGUI.PropertyField(fieldRect, value, GUIContent.none);


            EditorGUI.EndProperty();
        }



    }


#endif

}