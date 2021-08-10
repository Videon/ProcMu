using System;
using System.Collections;
using UnityEngine;

public class ExperimentAction : MonoBehaviour
{
    protected ExperimentConductor conductor;

    public void RegisterConductor(ExperimentConductor conductor)
    {
        this.conductor = conductor;
    }

    public virtual IEnumerator Run()
    {
        Debug.LogWarning("Run not overriden!");
        yield break;
    }

    public virtual void SubmitInput(int input)
    {
        throw new NotImplementedException();
    }
}