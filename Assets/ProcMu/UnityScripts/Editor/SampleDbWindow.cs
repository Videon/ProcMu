using System;
using ProcMu.ScriptableObjects;
using UnityEditor;
using UnityEngine;


public class SampleDbWindow : EditorWindow
{
    private string assetPath = "/ProcMu/";

    private MuSampleDb _sampleDb;

    private bool _dbInit;

    [MenuItem("ProcMu/Sample DB Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SampleDbWindow));
    }

    private void OnEnable()
    {
        _sampleDb = Resources.Load<MuSampleDb>("procmu_sampledb");
        _dbInit = _sampleDb != null;
    }

    private void OnGUI()
    {
        if (!_dbInit) DrawEmptyDb();

        else
        {
            DrawSampleList();

            if (GUILayout.Button("Register Samples"))
                RegisterAssets();
        }
    }

    private void DrawEmptyDb()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("No sample database exists. Create new database?");
        if (GUILayout.Button("Create sample database")) CreateDbObject();
        EditorGUILayout.EndVertical();
    }

    private void DrawSampleList()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ftable", GUILayout.Width(60f));
        EditorGUILayout.LabelField("Sample name", GUILayout.Width(180f));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < _sampleDb.audioClips.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("#" + (900 + i), GUILayout.Width(60f));
            EditorGUILayout.LabelField(_sampleDb.audioClipNames[i], GUILayout.Width(180f));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void RegisterAssets()
    {
        string[] assetGuids =
            AssetDatabase.FindAssets("t:AudioClip", new[] {"Assets/ProcMu/CsoundScripts/Resources/samples"});

        AudioClip[] audioClips = new AudioClip[assetGuids.Length];

        for (int i = 0; i < audioClips.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
            audioClips[i] = (AudioClip) AssetDatabase.LoadAssetAtPath(assetPath, typeof(AudioClip));
        }

        _sampleDb.FillDb(audioClips);

        Debug.Log("Registered " + audioClips.Length + " audio assets!");
    }

    private void CreateDbObject()
    {
        AssetDatabase.CreateAsset(MuSampleDb.CreateInstance(typeof(MuSampleDb)),
            "Assets/ProcMu/Resources/procmu_sampledb.asset");
        _sampleDb = Resources.Load<MuSampleDb>("procmu_sampledb");
        _dbInit = _sampleDb != null;
    }
}