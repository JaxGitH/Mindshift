using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Demo
{

    /// <summary>
    /// This class represents a movement action, used by the ActionBasedPlatform component.
    /// </summary>
    [System.Serializable]
    public class MovementAction
    {
        [SerializeField]
        bool enabled = true;

        [SerializeField]
        bool useWorldCoordinates = true;

        [SerializeField]
        bool infiniteDuration = false;

        [Min(0f)]
        [SerializeField]
        float cycleDuration = 2f;

        [SerializeField]
        bool waitAtTheEnd = true;

        [Min(0f)]
        [SerializeField]
        float waitDuration = 1f;

        [SerializeField]
        Vector3 direction = Vector3.up;

        [Min(0f)]
        [SerializeField]
        float speed = 2f;

        Transform transform = null;

        Vector3 initialLocalDirection;

        public void Initialize(Transform transform)
        {
            this.transform = transform;

            initialLocalDirection = transform.InverseTransformVectorUnscaled(direction);
        }

        Vector3 actionVector = Vector3.zero;
        public Vector3 ActionVector
        {
            get
            {
                return actionVector;
            }
        }


        public void Tick(float dt, ref Vector3 position)
        {
            if (!enabled)
                return;

            time += dt;

            if (isWaiting)
            {
                if (time >= waitDuration)
                {
                    time = 0f;
                    isWaiting = false;
                }

                actionVector = Vector3.zero;
            }
            else
            {
                if (!infiniteDuration && time >= cycleDuration)
                {
                    time = 0;

                    if (useWorldCoordinates)
                        direction = -direction;
                    else
                        initialLocalDirection = -initialLocalDirection;

                    if (waitAtTheEnd)
                        isWaiting = true;
                }

                if (isWaiting)
                    actionVector = Vector3.zero;
                else
                    actionVector = CustomUtilities.Multiply(
                        useWorldCoordinates ? direction : initialLocalDirection,
                        speed,
                        dt
                    );
            }

            position += actionVector;
        }

        public void ResetTimer()
        {
            time = 0f;
        }

        float time = 0f;
        bool isWaiting = false;
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(MovementAction))]
    public class MovementActionDrawer : PropertyDrawer
    {
        SerializedProperty enabled = null;
        SerializedProperty useWorldCoordinates = null;
        SerializedProperty infiniteDuration = null;
        SerializedProperty cycleDuration = null;
        SerializedProperty waitAtTheEnd = null;
        SerializedProperty waitDuration = null;
        SerializedProperty direction = null;
        SerializedProperty speed = null;

        float size = 0f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            enabled = property.FindPropertyRelative("enabled");
            useWorldCoordinates = property.FindPropertyRelative("useWorldCoordinates");
            infiniteDuration = property.FindPropertyRelative("infiniteDuration");
            cycleDuration = property.FindPropertyRelative("cycleDuration");
            waitAtTheEnd = property.FindPropertyRelative("waitAtTheEnd");
            waitDuration = property.FindPropertyRelative("waitDuration");
            direction = property.FindPropertyRelative("direction");
            speed = property.FindPropertyRelative("speed");

            // ----------------------------------------------------------------------------------
            Rect fieldRect = position;
            fieldRect.height = EditorGUIUtility.singleLineHeight;

            Rect backgroundRect = position;
            GUI.Box(backgroundRect, "", EditorStyles.helpBox);

            fieldRect.y += 0.25f * fieldRect.height;

            fieldRect.x += 5f;
            fieldRect.width = 20f;
            EditorGUI.PropertyField(fieldRect, enabled, GUIContent.none);

            fieldRect.x += 20f;
            fieldRect.width = position.width;
            EditorGUI.LabelField(fieldRect, "Movement", EditorStyles.boldLabel);

            fieldRect.x = position.x + 20f;
            fieldRect.y += 1.5f * fieldRect.height;
            fieldRect.width = position.width - 25;

            if (enabled.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, useWorldCoordinates);
                fieldRect.y += fieldRect.height;

                EditorGUI.PropertyField(fieldRect, infiniteDuration);
                fieldRect.y += fieldRect.height;

                if (!infiniteDuration.boolValue)
                {
                    EditorGUI.PropertyField(fieldRect, cycleDuration);

                    fieldRect.y += fieldRect.height;

                }

                fieldRect.y += fieldRect.height;

                EditorGUI.PropertyField(fieldRect, waitAtTheEnd);
                fieldRect.y += fieldRect.height;

                if (waitAtTheEnd.boolValue)
                {

                    EditorGUI.PropertyField(fieldRect, waitDuration);

                    fieldRect.y += fieldRect.height;

                }

                fieldRect.y += fieldRect.height;

                EditorGUI.PropertyField(fieldRect, direction);
                fieldRect.y += fieldRect.height;

                EditorGUI.PropertyField(fieldRect, speed);
                fieldRect.y += fieldRect.height;

                fieldRect.y += 0.5f * fieldRect.height;

            }



            size = fieldRect.y - position.y;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return size;
        }

#if UNITY_6000_0_OR_NEWER
        [System.Obsolete]
#endif
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;
    }

#endif

}