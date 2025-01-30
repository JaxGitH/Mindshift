#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

    [CustomEditor(typeof(CharacterState), true), CanEditMultipleObjects]
    public class CharacterStateEditor : Editor
    {
        CharacterState monoBehaviour = null;
        GUIStyle style = new GUIStyle();

        bool infoFoldout = false;

        void OnEnable()
        {
            monoBehaviour = (CharacterState)target;

            style.wordWrap = true;
            style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        public override void OnInspectorGUI()
        {
            string info = monoBehaviour.GetInfo();


            if (!info.IsNullOrEmpty())
            {
                infoFoldout = EditorGUILayout.Foldout(infoFoldout, "Show Info");

                if (infoFoldout)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(info, style);

                    GUILayout.EndVertical();

                }

            }


            DrawDefaultInspector();
        }
    }

}

#endif
