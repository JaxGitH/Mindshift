
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Lightbug.Utilities
{

    [CustomPropertyDrawer(typeof(CustomClassDrawer), true)]
    public class CustomClassDrawerEditor : PropertyDrawer
    {

        Color fontColor = new Color(0.15f, 0.15f, 0.15f);
        Color arrowColor = new Color(0.15f, 0.15f, 0.15f, 0.75f);

        GUIStyle textStyle = new GUIStyle();

        const float TitleHeight = 19;
        const float PostTitleSpace = 0;
        const float RightSpace = 1;
        const float ArrowMargin = 5;
        const float isEnabledWidth = 20;

#if UNITY_6000_0_OR_NEWER
        [System.Obsolete]
#endif
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float space = 0;

            if (property.isExpanded)
            {
                space = 1.2f * EditorGUI.GetPropertyHeight(property) + TitleHeight;
            }
            else
            {
                space = TitleHeight;
            }

            return space;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SetColors();

            textStyle.normal.textColor = fontColor;
            textStyle.alignment = TextAnchor.MiddleLeft;


            int initialIndent = EditorGUI.indentLevel;
            float initialfieldWidth = EditorGUIUtility.fieldWidth;
            float initialLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.fieldWidth = 60;

            Rect referenceRect = position;
            referenceRect.height = TitleHeight;


            Rect backgroundRect = position;
            backgroundRect.position = referenceRect.position;
            backgroundRect.width -= RightSpace;

            GUI.color = new Color(1f, 1f, 1f, 0.6f);
            GUI.Box(backgroundRect, GUIContent.none, EditorStyles.helpBox); // (GUIStyle)"IN ThumbnailShadow" );  
            GUI.color = Color.white;

            Rect titleRect = referenceRect;
            titleRect.width -= isEnabledWidth;
            titleRect.x += 7f;

            // if( GUI.Button( titleRect , GUIContent.none , EditorStyles.label ) )
            // 	property.isExpanded = !property.isExpanded;
            property.isExpanded = true;

            GUI.Label(titleRect, property.displayName, textStyle);



            // Rect arrowRect = referenceRect;

            // arrowRect.width = 0.5f * referenceRect.height;
            // arrowRect.height = arrowRect.width;
            // arrowRect.x += ArrowMargin;
            // arrowRect.y += referenceRect.height / 2 - arrowRect.height / 2;


            // Texture arrowTexture = Resources.Load<Texture>("Icons/whiteArrowFilledIcon");

            // if( property.isExpanded )
            // 	GUIUtility.RotateAroundPivot( 90 , arrowRect.center );		

            // GUI.color = arrowColor;
            // GUI.DrawTexture( arrowRect , arrowTexture );
            // GUI.color = Color.white;

            // if( property.isExpanded )
            // 	GUIUtility.RotateAroundPivot( -90 , arrowRect.center );




            if (property.isExpanded)
            {
                EditorGUI.indentLevel = 1;

                SerializedProperty itr = property.Copy();

                bool enterChildren = true;


                Rect childRect = referenceRect;
                childRect.y += 2 * EditorGUIUtility.singleLineHeight + PostTitleSpace;
                childRect.height = EditorGUIUtility.singleLineHeight;
                childRect.width -= 10;


                while (itr.NextVisible(enterChildren))
                {
                    enterChildren = false;

                    if (SerializedProperty.EqualContents(itr, property.GetEndProperty()))
                        break;


                    EditorGUI.PropertyField(childRect, itr);

                    childRect.y += 1.2f * EditorGUI.GetPropertyHeight(itr, null, false);
                }


            }


            EditorGUI.indentLevel = initialIndent;
            EditorGUIUtility.fieldWidth = initialfieldWidth;
            EditorGUIUtility.labelWidth = initialLabelWidth;


            EditorGUI.EndProperty();
        }


        void SetColors()
        {
            if (EditorGUIUtility.isProSkin)
            {
                fontColor = new Color(0.75f, 0.75f, 0.75f);
                arrowColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);
            }
            else
            {
                fontColor = Color.black;
                arrowColor = new Color(0.15f, 0.15f, 0.15f, 0.75f);
            }
        }



    }

}

#endif
