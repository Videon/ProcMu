using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExperimentStarter : MonoBehaviour
{
    [SerializeField] private JsonAzureTool azure;
    [SerializeField] private float probability1, probability2, probability3;

    [SerializeField] private ExperimentMode mode;

    [SerializeField] private ExperimentConductor[] experiments;

    // Start is called before the first frame update
    void Start()
    {
        float sum = probability1 + probability2 + probability3;
        float rnd = Random.Range(0, sum);
        if (rnd <= probability1) mode = (ExperimentMode) 0;
        else if (rnd <= probability1 + probability2) mode = (ExperimentMode) 1;
        else mode = (ExperimentMode) 2;

        ExperimentManager.Instance.experimentMode = mode;
        experiments[(int) mode].StartExperiment();
    }

    private void LoadProbabilities()
    {
        //float[] probabilities = azure.LoadData("probabilities");
    }
}