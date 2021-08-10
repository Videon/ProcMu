using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AzureTester))]
public class AzureTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AzureTester tester = (AzureTester) target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Submit round data")) tester.SubmitRoundData();

        if (GUILayout.Button("Submit survey data")) tester.SubmitSurveyData();

        if (GUILayout.Button("Upload")) tester.Upload();

        if (GUILayout.Button("Clear data")) tester.ClearData();
    }
}