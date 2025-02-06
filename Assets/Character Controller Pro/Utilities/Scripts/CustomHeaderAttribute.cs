using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.Utilities
{

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class CustomHeaderAttribute : PropertyAttribute
    {
        public enum HeaderColor
        {
            HighContrastLight = 0,
            HighContrastDark,
            DarkGray,
            LightGray,
        }

        public TextAlignment m_textAlignment;
        public string m_text;
        public bool m_colorThemeByProSkin = true;
        public bool m_filledBackground = true;
        public HeaderColor m_colorTheme;

        public CustomHeaderAttribute(string text)
        {
            m_text = text;
            m_textAlignment = TextAlignment.Left;
            m_colorThemeByProSkin = true;
            m_filledBackground = true;
        }

        public CustomHeaderAttribute(string text, TextAlignment textAlignment)
        {
            m_text = text;
            m_textAlignment = textAlignment;
            m_colorThemeByProSkin = true;
            m_filledBackground = true;
        }

        public CustomHeaderAttribute(string text, TextAlignment textAlignment, bool filledBackground)
        {
            m_text = text;
            m_textAlignment = textAlignment;
            m_colorThemeByProSkin = true;
            m_filledBackground = filledBackground;
        }

        public CustomHeaderAttribute(string text, TextAlignment textAlignment, bool filledBackground, HeaderColor colorTheme)
        {
            m_text = text;
            m_textAlignment = textAlignment;
            m_colorThemeByProSkin = false;
            m_colorTheme = colorTheme;
            m_filledBackground = filledBackground;
        }





    }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
    public class CustomHeaderAttributeEditor : DecoratorDrawer
    {
        const float BorderWidth = 1;
        const float PreSpace = 15;
        const float PostSpace = 5;
        const float TitleHeight = 20;
        const float TextMargin = 5;

        GUIStyle m_style = new GUIStyle();
        Texture2D m_backgroundTexture = new Texture2D(1, 1);

        Color m_backgroundColor;
        Color m_textColor;

        new CustomHeaderAttribute attribute = null;

        public override float GetHeight()
        {
            return PreSpace + TitleHeight + PostSpace;
        }

#if UNITY_6000_0_OR_NEWER
        [System.Obsolete]
#endif
        public override bool CanCacheInspectorGUI() => false;


        public override void OnGUI(Rect position)
        {
            attribute = base.attribute as CustomHeaderAttribute;

            if (attribute.m_colorThemeByProSkin)
                SetColorsBySkin();
            else
                SetColors(attribute.m_colorTheme);

            if (attribute.m_filledBackground)
                DrawFilledHeader(position, attribute.m_text);
            else
                DrawHorizontalLineHeader(position, attribute.m_text);


        }

        void DrawFilledHeader(Rect position, string text)
        {

            TextAnchor textAnchor = GetTextAnchor();

            DrawFilledBackground(position);
            DrawText(position, text, textAnchor, m_textColor, true);


        }



        void DrawHorizontalLineHeader(Rect position, string text)
        {
            TextAnchor textAnchor = GetTextAnchor();

            DrawText(position, text, textAnchor, m_backgroundColor, true);
            DrawLines(position, text, attribute.m_textAlignment);

        }

        TextAnchor GetTextAnchor()
        {
            switch (attribute.m_textAlignment)
            {
                case TextAlignment.Left:

                    return TextAnchor.MiddleLeft;

                case TextAlignment.Center:

                    return TextAnchor.MiddleCenter;

                case TextAlignment.Right:

                    return TextAnchor.MiddleRight;

                default:

                    return TextAnchor.MiddleCenter;

            }
        }

        void DrawText(Rect position, string text, TextAnchor textAnchor, Color textColor, bool bold)
        {
            // GUI Style
            m_style.alignment = textAnchor;

            if (bold)
                m_style.fontStyle = FontStyle.Bold;

            m_style.normal.textColor = textColor;
            m_backgroundTexture.SetPixel(1, 1, m_backgroundColor);
            m_backgroundTexture.Apply();


            Rect textRect = position;
            textRect.height = TitleHeight;

            switch (textAnchor)
            {
                case TextAnchor.MiddleLeft:

                    textRect.x += TextMargin;
                    break;

                case TextAnchor.MiddleRight:

                    textRect.x -= TextMargin;
                    break;

                default:
                    break;
            }

            textRect.y += PreSpace;
            GUI.Label(textRect, text, m_style);

        }

        void DrawFilledBackground(Rect position)
        {
            Rect backgroundTextureRect = position;
            backgroundTextureRect.x = 0;
            backgroundTextureRect.y += PreSpace;
            backgroundTextureRect.height = TitleHeight;
            backgroundTextureRect.width = EditorGUI.IndentedRect(position).width + 18f;
            GUI.DrawTexture(backgroundTextureRect, m_backgroundTexture);
        }


        void DrawLines(Rect position, string text, TextAlignment textAlignment)
        {
            GUIContent textGUIContent = new GUIContent(text);
            Vector2 textPixelSize = m_style.CalcSize(textGUIContent);

            Rect rightLineRect;
            Rect leftLineRect;

            switch (textAlignment)
            {
                case TextAlignment.Left:

                    rightLineRect = position;
                    rightLineRect.x += textPixelSize.x + 2 * TextMargin;
                    rightLineRect.y += PreSpace + TitleHeight / 2 - BorderWidth / 2;
                    rightLineRect.width = position.width - textPixelSize.x - 3 * TextMargin;
                    rightLineRect.height = BorderWidth;
                    GUI.DrawTexture(rightLineRect, m_backgroundTexture);

                    break;

                case TextAlignment.Center:

                    leftLineRect = position;
                    leftLineRect.x += TextMargin;
                    leftLineRect.y += PreSpace + TitleHeight / 2 - BorderWidth / 2;
                    leftLineRect.width = (position.width - textPixelSize.x) / 2 - 2 * TextMargin;
                    leftLineRect.height = BorderWidth;
                    GUI.DrawTexture(leftLineRect, m_backgroundTexture);

                    rightLineRect = position;
                    rightLineRect.x += (position.width - textPixelSize.x) / 2 + textPixelSize.x + TextMargin;
                    rightLineRect.y += PreSpace + TitleHeight / 2 - BorderWidth / 2;
                    rightLineRect.width = leftLineRect.width;
                    rightLineRect.height = BorderWidth;
                    GUI.DrawTexture(rightLineRect, m_backgroundTexture);

                    break;
                case TextAlignment.Right:

                    leftLineRect = position;
                    leftLineRect.x += TextMargin;
                    leftLineRect.y += PreSpace + TitleHeight / 2 - BorderWidth / 2;
                    leftLineRect.width = position.width - textPixelSize.x - 3 * TextMargin;
                    leftLineRect.height = BorderWidth;
                    GUI.DrawTexture(leftLineRect, m_backgroundTexture);

                    break;
            }



        }


        void SetColorsBySkin()
        {
            bool isPro = EditorGUIUtility.isProSkin;

            if (isPro)
                SetColors(CustomHeaderAttribute.HeaderColor.DarkGray);
            else
                SetColors(CustomHeaderAttribute.HeaderColor.LightGray);
        }


        void SetColors(CustomHeaderAttribute.HeaderColor color)
        {

            switch (color)
            {
                case CustomHeaderAttribute.HeaderColor.HighContrastLight:

                    m_textColor = Color.black;
                    m_backgroundColor = Color.white;
                    break;
                case CustomHeaderAttribute.HeaderColor.HighContrastDark:

                    m_textColor = Color.white;
                    m_backgroundColor = Color.black;
                    break;
                case CustomHeaderAttribute.HeaderColor.DarkGray:

                    m_textColor = new Color(0.95f, 0.95f, 0.95f);
                    m_backgroundColor = new Color(0.16f, 0.16f, 0.16f);
                    break;

                case CustomHeaderAttribute.HeaderColor.LightGray:

                    m_textColor = new Color(0.1f, 0.1f, 0.1f);
                    m_backgroundColor = 0.25f * Color.white;
                    break;

                default:
                    m_textColor = Color.black;
                    m_backgroundColor = Color.white;
                    break;

            }
        }


    }

#endif

}
