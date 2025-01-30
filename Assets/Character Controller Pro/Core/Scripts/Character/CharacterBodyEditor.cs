using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace Lightbug.CharacterControllerPro.Core
{

    [CustomEditor(typeof(CharacterBody), true), CanEditMultipleObjects]
    public class CharacterBodyEditor : Editor
    {
        SerializedProperty is2D = null;
        SerializedProperty bodySize = null;

        CharacterBody monoBehaviour;

        void OnEnable()
        {
            monoBehaviour = (CharacterBody)target;

            is2D = serializedObject.FindProperty("is2D");
            bodySize = serializedObject.FindProperty("bodySize");
        }

        public override void OnInspectorGUI()  
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if (monoBehaviour.transform.localScale != Vector3.one)
                EditorGUILayout.HelpBox("Transform local scale must be <1,1,1> !!!", MessageType.Error);

            serializedObject.ApplyModifiedProperties();
        }
        
        CapsuleBoundsHandle capsuleHandle = new CapsuleBoundsHandle();

        void OnSceneGUI()
        {
            if (monoBehaviour == null)
                return;

            if (is2D.boolValue)
            {
                Transform handlesTransform = monoBehaviour.transform;

                // handlesTransform.rotation = Quaternion.identity;
                Handles.matrix = handlesTransform.localToWorldMatrix;

                capsuleHandle.radius = bodySize.vector2Value.x / 2f;
                capsuleHandle.height = bodySize.vector2Value.y;

                capsuleHandle.center = Vector3.up * capsuleHandle.height / 2f;

                capsuleHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;

                capsuleHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Y;
                capsuleHandle.DrawHandle();

                Handles.matrix = Matrix4x4.identity;
            }
            else
            {
                Handles.matrix = monoBehaviour.transform.localToWorldMatrix;

                capsuleHandle.radius = bodySize.vector2Value.x / 2f;
                capsuleHandle.height = bodySize.vector2Value.y;

                capsuleHandle.center = Vector3.up * capsuleHandle.height / 2f;

                capsuleHandle.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y | PrimitiveBoundsHandle.Axes.Z;


                capsuleHandle.heightAxis = CapsuleBoundsHandle.HeightAxis.Y;
                capsuleHandle.DrawHandle();

                Handles.matrix = Matrix4x4.identity;
            }

        }
    }

}

#endif
