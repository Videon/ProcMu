using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporterAction : ExperimentAction
{
    [SerializeField] private Transform targetTransform;

    public override IEnumerator Run()
    {
        conductor.MovePlayer(targetTransform != null ? targetTransform.position : Vector3.zero);
        yield break;
    }
}