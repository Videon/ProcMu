using ProcMu.ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MuSampleDb))]
public class MuSampleDbEditor : Editor
{
    private MuSampleDb _muSampleDb;

    public override void OnInspectorGUI()
    {
        _muSampleDb = (MuSampleDb) target;

        DrawSampleList();

        if (GUILayout.Button("Register Samples"))
            RegisterAssets();
    }

    private void DrawSampleList()
    {
        EditorGUILayout.BeginVertical();

        for (int i = 0; i < _muSampleDb.audioClips.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("#" + 900 + i);
            EditorGUILayout.LabelField(_muSampleDb.audioClipNames[i]);
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

        _muSampleDb.FillDb(audioClips);

        EditorUtility.SetDirty(_muSampleDb);

        Debug.Log("Registered " + audioClips.Length + " audio assets!");
    }
}