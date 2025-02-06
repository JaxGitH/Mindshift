
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Lightbug.Utilities
{
    public enum HelpBoxMessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class HelpBoxAttribute : PropertyAttribute
    {

        public string Text;
        public HelpBoxMessageType MessageType;

        public HelpBoxAttribute(string text, HelpBoxMessageType messageType)
        {
            Text = text;
            MessageType = messageType;
        }


    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeEditor : DecoratorDrawer
    {
        new HelpBoxAttribute attribute = null;

        public override float GetHeight()
        {
            attribute = base.attribute as HelpBoxAttribute;
            var height = EditorStyles.helpBox.CalcHeight(new GUIContent(attribute.Text), EditorGUIUtility.currentViewWidth - 36f);
            return height + 4f;
        }

        public override void OnGUI(Rect position)
        {
            attribute = base.attribute as HelpBoxAttribute;
            EditorGUI.HelpBox(position, attribute.Text, (MessageType)attribute.MessageType);
        }

    }

#endif

}


