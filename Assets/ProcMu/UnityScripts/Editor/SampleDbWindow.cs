using ProcMu.ScriptableObjects;
using UnityEditor;
using UnityEngine;


public class SampleDbWindow : EditorWindow
{
    private MuSampleDb _sampleDb;

    [MenuItem("ProcMu/Sample DB Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SampleDbWindow));
    }

    private void OnGUI()
    {
        _sampleDb = MuSampleDb.instance;

        DrawSampleList();

        if (GUILayout.Button("Register Samples"))
            RegisterAssets();
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

        _sampleDb.SaveAsset();

        Debug.Log("Registered " + audioClips.Length + " audio assets!");
    }
}