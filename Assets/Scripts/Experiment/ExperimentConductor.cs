using System.Collections;
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

    [SerializeField] private float timeBetweenActions = 1f;
    [SerializeField] private ExperimentAction[] experimentActions;

    #endregion

    #region Experiment metrics variables

    private float elapsedTime = 0f;

    #endregion

    public void StartExperiment()
    {
        for (int i = 0; i < experimentActions.Length; i++)
        {
            if (experimentActions[i] == null) continue;
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

            yield return new WaitForSeconds(timeBetweenActions);
        }
    }

    public void MovePlayer(Vector3 targetPosition)
    {
        //TODO MAY NEED TO DEACTIVATE SOME COMPONENTS ON PLAYER FOR THIS TO WORK
        playerTransform.GetComponent<CharacterController>().enabled = false;
        playerTransform.position = targetPosition;
        playerTransform.GetComponent<CharacterController>().enabled = true;
    }

    public void SetWaitForInput(bool awaitInput)
    {
        this.awaitInput = awaitInput;
    }

    public void SubmitInput(int inputValue)
    {
        //if (!awaitInput) return;
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