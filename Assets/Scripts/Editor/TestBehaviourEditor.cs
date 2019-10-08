using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

//[CustomEditor(typeof(TestBehaviour))]
public class TestBehaviourEditor : Editor

{
    private SerializedProperty isTrackingXAxisSerializedProperty;
    private SerializedProperty testDataSerializedProperty;

    AnimBool open;

    ReorderableList list;

    private void OnEnable()
    {
        testDataSerializedProperty = serializedObject.FindProperty("_testData");
        open = new AnimBool(false);
        open.valueChanged.AddListener(Repaint);

        list = new ReorderableList(testDataSerializedProperty.serializedObject, testDataSerializedProperty);
        list.drawHeaderCallback += DrawHeader;
        list.drawElementCallback += DrawElement;

    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var elementRect = rect;
        var arect = rect;
        var serElem = list.serializedProperty.GetArrayElementAtIndex(index);
        arect.height = EditorGUIUtility.singleLineHeight;
        arect.width = elementRect.width / 3;
        EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("name"), GUIContent.none);
        arect.x += arect.width;
        arect.width = elementRect.width / 3; ;
        EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("number"), GUIContent.none);
        arect.x += arect.width;
        arect.width = elementRect.width / 3; ;
        EditorGUI.PropertyField(arect, serElem.FindPropertyRelative("color"), GUIContent.none);
    }



    private void DrawHeader(Rect rect)
    {
        var headerRect = rect;
        var arect = rect;
        arect.height = EditorGUIUtility.singleLineHeight;
        arect.width = headerRect.width / 3;
        EditorGUI.LabelField(arect, "Name");
        arect.x += arect.width;
        arect.width = headerRect.width / 3;
        EditorGUI.LabelField(arect, "Number");
        arect.x += arect.width;
        arect.width = headerRect.width / 3;
        EditorGUI.LabelField(arect, "Color");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //serializedObject.Update();
        //m_target = (TestBehaviour)target;

        ////EditorGUILayout.PropertyField(isTrackingXAxisSerializedProperty,new GUIContent("isTrackingXAxis"));

        ////serializedObject.ApplyModifiedProperties();
        ////EditorGUI.BeginChangeCheck();
        //m_target.isTrackingXAxis = EditorGUILayout.Toggle("is Tracking X", m_target.isTrackingXAxis);
        //open.target = EditorGUILayout.Foldout(open.target, "open?");

        ////if (EditorGUI.EndChangeCheck())
        ////{
        ////    Debug.Log("GUI chage!");
        ////}

        //EditorGUI.BeginDisabledGroup(!m_target.isTrackingXAxis);
        //GUI.color = Color.gray;
        //if (EditorGUILayout.BeginFadeGroup(open.faded))
        //{
        //    EditorGUILayout.BeginVertical();
        //    GUILayout.Button("I'm Button");
        //    GUILayout.Button("I'm Button");
        //    GUILayout.Button("I'm Button");
        //    GUILayout.Button("I'm Button");
        //    GUILayout.Button("I'm Button");
        //    GUILayout.Button("I'm Button");

        //    EditorGUILayout.EndVertical();
        //}
        //EditorGUILayout.EndFadeGroup();
        //EditorGUI.EndDisabledGroup();



        //GUILayout.BeginHorizontal();
        //GUI.color = Color.gray;
        //if (GUILayout.Button("I'm Button"))
        //{
        //    Debug.Log("HW");
        //}
        //GUI.color = Color.red;
        //if (GUILayout.Button("I'm Button"))
        //{
        //    Debug.Log("HW");
        //}
        //GUI.color = Color.green;
        //if (GUILayout.Button("I'm Button"))
        //{
        //    Debug.Log("HW");
        //}
        //GUILayout.EndHorizontal();
        //GUILayout.BeginVertical();
        //GUI.color = Color.white;
        //m_target.isTrackingXAxis = EditorGUILayout.Toggle("is Tracking X", m_target.isTrackingXAxis);
        //m_target.xMargin = EditorGUILayout.FloatField("Margin", m_target.xMargin);
        //m_target.xSmooth = EditorGUILayout.FloatField("Smooth", m_target.xSmooth);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Min and Max", GUILayout.MaxWidth(100));
        //m_target.minMaxX.x = EditorGUILayout.FloatField("Min", m_target.minMaxX.x, GUILayout.ExpandWidth(false));
        //m_target.minMaxX.y = EditorGUILayout.FloatField("Max", m_target.minMaxX.y, GUILayout.ExpandWidth(false));
        //GUILayout.EndHorizontal();
        //m_target.xPos = EditorGUILayout.FloatField("Position", m_target.xPos);

        //GUILayout.EndVertical();
        
        this.serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        
    }


}
