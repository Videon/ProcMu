using System.Collections;
using System.Collections.Generic;
using ProcMu.UnityScripts;
using UnityEngine;

public class InterpolatorTransformSetterAction : ExperimentAction
{
    [SerializeField] private Interpolator interpolator;
    [SerializeField] private Transform transformObject;

    public override IEnumerator Run()
    {
        interpolator.playerTransform = transformObject;
        yield break;
    }
}