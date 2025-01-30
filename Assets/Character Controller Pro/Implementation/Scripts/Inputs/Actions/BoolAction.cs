using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Implementation
{

    /// <summary>
    /// This struct contains all the button states, which are updated frame by frame.
    /// </summary>
    [System.Serializable]
    public struct BoolAction
    {
        /// <summary>
        /// The action current value.
        /// </summary>
        public bool value;

        /// <summary>
        /// Returns true if the value transitioned from false to true (e.g. a button press).
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Returns true if the value transitioned from true to false (e.g. a button release).
        /// </summary>
        public bool Canceled { get; private set; }

        /// <summary>
        /// Elapsed time since the last "Started" flag.
        /// </summary>
        public float StartedElapsedTime { get; private set; }

        /// <summary>
        /// Elapsed time since the last "Canceled" flag.
        /// </summary>
        public float CanceledElapsedTime { get; private set; }

        /// <summary>
        /// The elapsed time since this action was set to true.
        /// </summary>
        public float ActiveTime { get; private set; }

        /// <summary>
        /// The elapsed time since this action was set to false.
        /// </summary>
        public float InactiveTime { get; private set; }

        /// <summary>
        /// The last "ActiveTime" value registered by this action (on Canceled).
        /// </summary>
        public float LastActiveTime { get; private set; }

        /// <summary>
        /// The last "InactiveTime" value registered by this action (on Started).
        /// </summary>
        public float LastInactiveTime { get; private set; }

        bool previousValue;
        bool previousStarted;
        bool previousCanceled;

        /// <summary>
        /// Initialize the values.
        /// </summary>
        public void Initialize()
        {
            StartedElapsedTime = Mathf.Infinity;
            CanceledElapsedTime = Mathf.Infinity;

            value = false;
            previousValue = false;
            previousStarted = false;
            previousCanceled = false;
        }

        /// <summary>
        /// Resets the action.
        /// </summary>
        public void Reset()
        {
            Started = false;
            Canceled = false;
        }

        /// <summary>
        /// Updates the fields based on the current button state.
        /// </summary>
        public void Update(float dt)
        {
            Started |= !previousValue && value;
            Canceled |= previousValue && !value;

            StartedElapsedTime += dt;
            CanceledElapsedTime += dt;

            if (Started)
            {
                StartedElapsedTime = 0f;

                if (!previousStarted)
                {
                    LastActiveTime = 0f;
                    LastInactiveTime = InactiveTime;
                }
            }

            if (Canceled)
            {
                CanceledElapsedTime = 0f;

                if (!previousCanceled)
                {
                    LastActiveTime = ActiveTime;
                    LastInactiveTime = 0f;
                }
            }


            if (value)
            {
                ActiveTime += dt;
                InactiveTime = 0f;
            }
            else
            {
                ActiveTime = 0f;
                InactiveTime += dt;
            }


            previousValue = value;
            previousStarted = Started;
            previousCanceled = Canceled;
        }
        
    }


    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // EDITOR ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(BoolAction))]
    public class BoolActionEditor : PropertyDrawer
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