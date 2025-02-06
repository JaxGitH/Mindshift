#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Lightbug.CharacterControllerPro.Core
{
    [CustomEditor(typeof(CharacterActor))]
    public class CharacterActorEditor : Editor
    {
        CharacterActor characterActor = null;

        void OnEnable()
        {
            characterActor = target as CharacterActor;
        }

        void OnDisable()
        {
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            if (characterActor.CharacterBody.Is2D)
            {
                if (Physics2D.simulationMode != SimulationMode2D.FixedUpdate)
                    EditorGUILayout.HelpBox("(Physics2D) Only FixedUpdate simulation mode is supported!", MessageType.Error);
            }
            else
            {
#if UNITY_6000_0_OR_NEWER
                if (Physics.simulationMode != SimulationMode.FixedUpdate)
                    EditorGUILayout.HelpBox("(Physics) Only FixedUpdate simulation mode is supported!", MessageType.Error);
#endif
            }

            EditorGUIUtility.labelWidth += 75;
            DrawDefaultInspector();
            EditorGUIUtility.labelWidth -= 75;
        }

        void OnSceneGUI()
        {
            if (!Application.isPlaying)
                return;

            if (Tools.current == Tool.Move)
            {
                Tools.hidden = true;

                EditorGUI.BeginChangeCheck();

                Vector3 targetPosition = Handles.PositionHandle(characterActor.transform.position, characterActor.transform.rotation);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(characterActor, "Change character position");
                    characterActor.Teleport(targetPosition);
                }
            }
            else if (Tools.current == Tool.Rotate)
            {
                Tools.hidden = true;

                EditorGUI.BeginChangeCheck();

                Quaternion targetRotation = Handles.RotationHandle(characterActor.transform.rotation, characterActor.transform.position);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(characterActor, "Change character rotation");
                    characterActor.Teleport(characterActor.Position, targetRotation);
                }
            }
            else
            {
                Tools.hidden = false;
            }
        }
    }
}

#endif
