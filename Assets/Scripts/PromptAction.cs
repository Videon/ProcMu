using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptAction : ExperimentAction
{
    [SerializeField, TextArea] private string text;

    public override IEnumerator Run()
    {
        return base.Run();
    }
}