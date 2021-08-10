using UnityEngine;

public class ExperimentStarter : MonoBehaviour
{
    private ExperimentMode mode;

    [SerializeField] private ExperimentConductor[] experiments;

    // Start is called before the first frame update
    void Start()
    {
        mode = (ExperimentMode) Random.Range(0, 3);
        ExperimentManager.Instance.experimentMode = mode;
        experiments[(int) mode].StartExperiment();
    }
}