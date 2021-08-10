using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PromptAction : ExperimentAction
{
    [SerializeField] private SurveyQuestion surveyQuestion;

    [SerializeField] private UnityEvent event1;
    [SerializeField] private UnityEvent event2;

    [SerializeField] private Text uiTextQuestion;
    [SerializeField] private Text uiTextAnswer;

    private bool waitingForInput = false;

    public override IEnumerator Run()
    {
        uiTextQuestion.text = surveyQuestion.question;

        string answers = "";
        for (int i = 0; i < surveyQuestion.answers.Length; i++)
        {
            answers += surveyQuestion.answers[i] + "\n";
        }

        uiTextAnswer.text = answers;
        yield return WaitForInput();

        ClearText();
    }

    private IEnumerator WaitForInput()
    {
        waitingForInput = true;
        while (waitingForInput)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    public override void SubmitInput(int input)
    {
        if (input == 100)
            event1.Invoke();
        else if (input == 200)
            event2.Invoke();
    }

    public void StopWaiting()
    {
        waitingForInput = false;
    }

    private void ClearText()
    {
        uiTextQuestion.text = "";
        uiTextAnswer.text = "";
    }
}