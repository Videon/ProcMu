using System.Collections;
using System.Collections.Generic;
using ProcMu.UnityScripts;
using UnityEngine;

public class ProcMuParamSetterAction : ExperimentAction
{
    [SerializeField] private ProcMuMaster procmu;
    [SerializeField] float procmuIntensity = 0f;

    public override IEnumerator Run()
    {
        procmu.SetIntensity(procmuIntensity);
        yield break;
    }
}