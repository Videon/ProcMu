using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentSetupAction : ExperimentAction
{
    public override IEnumerator Run()
    {
        ExperimentManager.Instance.experimentMode = (ExperimentMode) Random.Range(0, 3); //todo fetch from Azure

        yield break;
    }
}