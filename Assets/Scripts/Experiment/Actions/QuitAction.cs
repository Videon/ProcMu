using System.Collections;
using UnityEditor;
using UnityEngine;

public class QuitAction : ExperimentAction
{
    public override IEnumerator Run()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

        Application.Quit();

        yield break;
    }
}