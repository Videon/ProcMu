using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExperimentConductor : MonoBehaviour
{
    #region Conductor variables

    [SerializeField] private Transform playerTransform;

    private bool awaitInput = false;
    private ExperimentAction activeAction;

    #endregion

    #region Experiment actions

    [SerializeField] private ExperimentAction[] experimentActions;

    #endregion

    #region Experiment parameter variables

    [SerializeField, Tooltip("How many objects must be collected before ending round 1.")]
    private int collectObjectsRound1 = 4;

    [SerializeField, Tooltip("How many objects must be collected before ending round 2.")]
    private int collectObjectsRound2 = 4;

    #endregion

    #region Experiment metrics variables

    private float elapsedTime = 0f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < experimentActions.Length; i++)
        {
            experimentActions[i].RegisterConductor(this);
        }

        StartCoroutine(RunExperiment());
    }

    private IEnumerator RunExperiment()
    {
        for (int i = 0; i < experimentActions.Length; i++)
        {
            activeAction = experimentActions[i];
            if (activeAction != null)
                yield return StartCoroutine(activeAction.Run());
        }
    }

    public void MovePlayer(Vector3 targetPosition)
    {
        //TODO MAY NEED TO DEACTIVATE SOME COMPONENTS ON PLAYER FOR THIS TO WORK
        playerTransform.position = targetPosition;
    }

    public void SetWaitForInput(bool awaitInput)
    {
        this.awaitInput = awaitInput;
    }

    public void SubmitInput(int inputValue)
    {
        if (!awaitInput) return;
        activeAction.SubmitInput(inputValue);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}

public enum ExperimentMode
{
    GuidedKnown,
    GuidedUnknown,
    UnguidedControl
}