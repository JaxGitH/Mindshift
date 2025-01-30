#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{


    [CustomEditor(typeof(NodeBasedPlatform), true)]
    public class NodeBasedPlatformEditor : Editor
    {

        NodeBasedPlatform monoBehaviour;
        Transform transform;

        ReorderableList reorderableList = null;

        #region Properties


        SerializedProperty drawHandles = null;

        SerializedProperty move = null;
        SerializedProperty rotate = null;
        SerializedProperty actionsList = null;

        SerializedProperty sequenceType;
        SerializedProperty positiveSequenceDirection = null;

        SerializedProperty globalSpeedModifier = null;

        #endregion

        int deletedObjectIndex = -1;

        GUIStyle style = new GUIStyle();

        void OnEnable()
        {
            monoBehaviour = (NodeBasedPlatform)target;
            transform = monoBehaviour.GetComponent<Transform>();

            style.fontSize = 25;
            style.normal.textColor = Color.white;


            drawHandles = serializedObject.FindProperty("drawHandles");

            move = serializedObject.FindProperty("move");
            rotate = serializedObject.FindProperty("rotate");
            actionsList = serializedObject.FindProperty("actionsList");

            sequenceType = serializedObject.FindProperty("sequenceType");
            positiveSequenceDirection = serializedObject.FindProperty("positiveSequenceDirection");

            globalSpeedModifier = serializedObject.FindProperty("globalSpeedModifier");


            reorderableList = new ReorderableList(
                serializedObject,
                actionsList,
                true,
                false,
                false,
                false
            );

            //reorderableList.headerHeight = 0f;
            reorderableList.footerHeight = 0f;


            reorderableList.drawElementCallback += OnDrawElement;
            reorderableList.elementHeightCallback += OnElementHeight;
            reorderableList.onAddCallback += OnAddElement;

            reorderableList.drawHeaderCallback += OnDrawHeader;


        }

        void OnDisable()
        {
            reorderableList.drawElementCallback -= OnDrawElement;
            reorderableList.elementHeightCallback -= OnElementHeight;
            reorderableList.onAddCallback -= OnAddElement;
            reorderableList.drawHeaderCallback -= OnDrawHeader;
        }

        void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Nodes");
        }

        void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            GUI.Box(rect, "", EditorStyles.helpBox);

            SerializedProperty element = actionsList.GetArrayElementAtIndex(index);

            Rect fieldRect = rect;
            fieldRect.x += 4;
            fieldRect.width -= 8;
            fieldRect.height = EditorGUI.GetPropertyHeight(element);


            fieldRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(fieldRect, element);
            fieldRect.y += fieldRect.height + EditorGUIUtility.singleLineHeight;

            Rect buttonRect = fieldRect;
            buttonRect.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(buttonRect, "x"))
            {
                deletedObjectIndex = index;
            }

        }

        float OnElementHeight(int index)
        {
            SerializedProperty element = actionsList.GetArrayElementAtIndex(index);

            return EditorGUI.GetPropertyHeight(element) + 4 * EditorGUIUtility.singleLineHeight;
        }

        void OnAddElement(ReorderableList list)
        {
            actionsList.arraySize++;
            SerializedProperty element = actionsList.GetArrayElementAtIndex(actionsList.arraySize - 1);

            // PlatformNode node = monobehaviour.ActionsList[ monobehaviour.ActionsList.Count ];
            // node = new PlatformNode();

            // PlatformNode node = element.objectReferenceValue as PlatformNode;
            // node = new PlatformNode();
            // //node.Initialize();
            // //actionsList.
        }


        void OnSceneGUI()
        {
            if (!monoBehaviour.enabled)
                return;

            if (transform == null)
                transform = monoBehaviour.GetComponent<Transform>();

            if (!monoBehaviour.DrawHandles)
                return;

            for (int i = 0; i < monoBehaviour.ActionsList.Count; i++)
            {
                Vector3 position = monoBehaviour.UpdateInitialPosition ? transform.position : monoBehaviour.InitialPosition;

                DrawHandle(position, monoBehaviour.ActionsList[i]);
                DrawText(position, monoBehaviour.ActionsList[i], i);


                if (i > 0)
                {
                    //Line between nodes
                    Handles.color = new Color(1, 1, 1, 0.2f);
                    Handles.DrawDottedLine(position + monoBehaviour.ActionsList[i].position,
                    position + monoBehaviour.ActionsList[i - 1].position, 2);


                    Handles.color = new Color(1, 1, 1, 0.8f);

                    //Middle
                    Vector3 middle = ((position + monoBehaviour.ActionsList[i].position) + (position + monoBehaviour.ActionsList[i - 1].position)) / 2;
                    Vector3 direction = ((position + monoBehaviour.ActionsList[i].position) - (position + monoBehaviour.ActionsList[i - 1].position));
                    direction.Normalize();


                    float distance = ((position + monoBehaviour.ActionsList[i].position) - (position + monoBehaviour.ActionsList[i - 1].position)).magnitude;
                    float arrowSize = distance / 4;
                    Vector3 arrowPosition = middle - direction * arrowSize / 2;

                    if (direction != Vector3.zero)
                        DrawArrowCap(i, arrowPosition, Quaternion.LookRotation(direction, -Vector3.forward), arrowSize, EventType.Repaint);



                    if (monoBehaviour.sequenceType == NodeBasedPlatform.SequenceType.Loop)
                    {
                        if (i == (monoBehaviour.ActionsList.Count - 1))
                        {
                            Handles.color = new Color(1, 1, 1, 0.2f);
                            Handles.DrawDottedLine(position + monoBehaviour.ActionsList[i].position,
                                position + monoBehaviour.ActionsList[0].position, 2);
                        }
                    }
                }
            }

        }

        void DrawText(Vector3 referencePosition, PlatformNode currentAction, int index)
        {
            Vector3 TextPosition = referencePosition + currentAction.position;

            style.fontSize = 25;
            style.normal.textColor = Color.white;
            Handles.Label(TextPosition, index.ToString(), style);
        }

        void DrawHandle(Vector3 referencePosition, PlatformNode currentAction)
        {
            float radius = 0.5f;

            Handles.color = Color.white;

            Handles.DrawWireDisc(referencePosition + currentAction.position, -Vector3.forward, radius);


            Vector3[] lines = new Vector3[]{
            referencePosition + currentAction.position + Vector3.up * radius ,
            referencePosition + currentAction.position - Vector3.up * radius ,
            referencePosition + currentAction.position + Vector3.right * radius ,
            referencePosition + currentAction.position - Vector3.right * radius
        };

            Handles.DrawLines(lines);

            //Position Handle
            currentAction.position = Handles.PositionHandle(
                    referencePosition + currentAction.position, Quaternion.identity) - referencePosition;


        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomUtilities.DrawMonoBehaviourField<NodeBasedPlatform>(monoBehaviour);

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Debug Options", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.PropertyField(drawHandles);

            GUILayout.EndVertical();


            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Actions", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(move);
            EditorGUILayout.PropertyField(rotate);


            reorderableList.DoLayoutList();

            if (deletedObjectIndex != -1)
            {
                actionsList.DeleteArrayElementAtIndex(deletedObjectIndex);
                deletedObjectIndex = -1;
            }

            if (GUILayout.Button("Add Node", EditorStyles.miniButton))
            {
                monoBehaviour.ActionsList.Add(new PlatformNode());
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Properties", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(sequenceType);
            EditorGUILayout.PropertyField(positiveSequenceDirection);

            EditorGUILayout.PropertyField(globalSpeedModifier);


            GUILayout.EndVertical();



            serializedObject.ApplyModifiedProperties();

            //SceneView.RepaintAll();
        }

        void DrawArrowCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
        {
#if UNITY_5_6_OR_NEWER
            Handles.ArrowHandleCap(controlID, position, rotation, size, eventType);
#else
			Handles.ArrowCap( controlID , position , rotation , size );
#endif
        }
    }



}

#endif
