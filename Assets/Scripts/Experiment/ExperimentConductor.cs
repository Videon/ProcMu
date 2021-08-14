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


    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    #region Input handling

    public void SetWaitForInput(bool awaitInput)
    {
        this.awaitInput = awaitInput;
    }

    private void SubmitInput(int inputValue)
    {
        //if (!awaitInput) return;
        activeAction.SubmitInput(inputValue);
    }

    public void OnConfirm()
    {
        SubmitInput(100);
    }

    public void OnAbort()
    {
        SubmitInput(200);
    }

    public void OnVote1()
    {
        SubmitInput(0);
    }

    public void OnVote2()
    {
        SubmitInput(1);
    }

    public void OnVote3()
    {
        SubmitInput(2);
    }

    public void OnVote4()
    {
        SubmitInput(3);
    }

    public void OnVote5()
    {
        SubmitInput(4);
    }

    public void OnVote6()
    {
        SubmitInput(5);
    }

    public void OnVote7()
    {
        SubmitInput(6);
    }

    public void OnVote8()
    {
        SubmitInput(7);
    }

    #endregion
}

public enum ExperimentMode
{
    GuidedKnown,
    GuidedUnknown,
    UnguidedControl
}