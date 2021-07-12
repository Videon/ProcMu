using System.Collections;
using System.Collections.Generic;
using ProcMu.ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MuConfig))]
public class MuConfigEditor : Editor
{
    private MuConfig _muConfig;

    public override void OnInspectorGUI()
    {
        _muConfig = (MuConfig) target;

        DrawGeneralSettings();
        DrawEucRth();
        DrawSnhMel();

        EditorUtility.SetDirty(_muConfig); //TODO SET ONLY DIRTY WHEN A CHANGE WAS MADE
    }

    private void DrawGeneralSettings()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("General settings");
        _muConfig.bpm = EditorGUILayout.DoubleField("BPM", _muConfig.bpm);
        _muConfig.Scale = (MuScale) EditorGUILayout.ObjectField("Scale", _muConfig.Scale, typeof(MuScale), false);
        EditorGUILayout.EndVertical();
    }

    private void DrawEucRth()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Euclidean Rhythm settings");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(80f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        EditorGUILayout.LabelField("Intensity = 0 settings", GUILayout.Width(160f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        EditorGUILayout.LabelField("Intensity = 1 settings", GUILayout.Width(160f));
        EditorGUILayout.EndHorizontal();

        //Draw labels
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sample", GUILayout.Width(80f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        EditorGUILayout.LabelField("min pulses", GUILayout.Width(80f));
        EditorGUILayout.LabelField("max pulses", GUILayout.Width(80f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        EditorGUILayout.LabelField("min pulses", GUILayout.Width(80f));
        EditorGUILayout.LabelField("max pulses", GUILayout.Width(80f));
        EditorGUILayout.EndHorizontal();


        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _muConfig.sampleSelection[i] =
                EditorGUILayout.Popup(_muConfig.sampleSelection[i], MuSampleDb.instance.audioClipNames,
                    GUILayout.Width(80f));
            EditorGUILayout.LabelField("", GUILayout.Width(10f));

            _muConfig.minImpulses0[i] = EditorGUILayout.IntField(_muConfig.minImpulses0[i], GUILayout.Width(40f));
            EditorGUILayout.LabelField("", GUILayout.Width(40f));
            _muConfig.maxImpulses0[i] = EditorGUILayout.IntField(_muConfig.maxImpulses0[i], GUILayout.Width(40f));
            EditorGUILayout.LabelField("", GUILayout.Width(40f));
            EditorGUILayout.LabelField("", GUILayout.Width(10f));
            _muConfig.minImpulses1[i] = EditorGUILayout.IntField(_muConfig.minImpulses1[i], GUILayout.Width(40f));
            EditorGUILayout.LabelField("", GUILayout.Width(40f));
            _muConfig.maxImpulses1[i] = EditorGUILayout.IntField(_muConfig.maxImpulses1[i], GUILayout.Width(40f));
            EditorGUILayout.LabelField("", GUILayout.Width(40f));


            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSnhMel()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Sample And Hold Melody Settings");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Octave range", GUILayout.Width(120f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        EditorGUILayout.LabelField("Occurence (per minute)", GUILayout.Width(240f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        _muConfig.minOct = EditorGUILayout.IntField(_muConfig.minOct, GUILayout.Width(60f));
        _muConfig.maxOct = EditorGUILayout.IntField(_muConfig.maxOct, GUILayout.Width(60f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        _muConfig.occurence = EditorGUILayout.DoubleField(_muConfig.occurence, GUILayout.Width(60f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("min", GUILayout.Width(60f));
        EditorGUILayout.LabelField("max", GUILayout.Width(60f));
        EditorGUILayout.LabelField("", GUILayout.Width(10f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    //TODO Add drawing methods for all sound generator modules
}