#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Lightbug.CharacterControllerPro.Core
{
    public class AboutWindow : CharacterControllerProWindow
    {
        const float Width = 200f;
        const float Height = 100f;

        protected override void OnEnable()
        {
            this.position = new Rect((Screen.width - Width) / 2f, (Screen.height - Height) / 2f, Width, Height);
            this.maxSize = this.minSize = this.position.size;
            this.titleContent = new GUIContent("About");
        }

        void OnGUI()
        {
            EditorGUILayout.SelectableLabel("Version: 1.4.11", GUILayout.Height(15f));
            EditorGUILayout.SelectableLabel("Author : Juan Sálice (Lightbug)", GUILayout.Height(15f));
            EditorGUILayout.SelectableLabel("Email : lightbug14@gmail.com", GUILayout.Height(15f));
        }
    }

}

#endif
