#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Lightbug.CharacterControllerPro.Core
{
    public abstract class CharacterControllerProWindow : EditorWindow
    {
        protected GUIStyle titleStyle = new GUIStyle();
        protected GUIStyle subtitleStyle = new GUIStyle();
        protected GUIStyle descriptionStyle = new GUIStyle();


        protected virtual void OnEnable()
        {
            titleStyle.fontSize = 50;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.padding.top = 4;
            titleStyle.padding.bottom = 4;
            titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            subtitleStyle.fontSize = 18;
            subtitleStyle.alignment = TextAnchor.MiddleCenter;
            subtitleStyle.padding.top = 4;
            subtitleStyle.padding.bottom = 4;
            subtitleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            descriptionStyle.fontSize = 15;
            descriptionStyle.wordWrap = true;
            descriptionStyle.padding.left = 10;
            descriptionStyle.padding.right = 10;
            descriptionStyle.padding.top = 4;
            descriptionStyle.padding.bottom = 4;
            descriptionStyle.richText = true;
            descriptionStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        }
    }

}

#endif
