using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Com.MyCompany.MyGame
{
    [CustomEditor(typeof(CameraWork))]
    public class CameraWorkInspect : Editor
    {
        bool IsGunner;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            CameraWork myTarget = (CameraWork)target;
            EditorGUILayout.Space();
            IsGunner = EditorGUILayout.BeginToggleGroup("狙擊鏡", myTarget.IsGunner);
            myTarget.IsGunner = IsGunner;
            if (IsGunner == true)
            {
                myTarget.aimCenter = EditorGUILayout.Vector3Field("準心位置", myTarget.aimCenter);
                myTarget.aimsight = EditorGUILayout.FloatField("瞄準視野", myTarget.aimsight);
            }
            EditorGUILayout.EndToggleGroup();
        }
    }
}
