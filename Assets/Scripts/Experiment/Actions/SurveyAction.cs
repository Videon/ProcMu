using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Action for displaying/saving a survey with up to 8 questions. </summary>
public class SurveyAction : ExperimentAction
{
    [SerializeField] private string surveyName;
    [SerializeField] private SurveyQuestion[] surveyQuestions;

    private int lastInput = -1;
    private bool waitingForInput = false;
    private int questionIndex;

    private int[] surveyAnswers;

    [SerializeField] private Text uiTextQuestion;
    [SerializeField] private Text uiTextAnswer;

    private int[] surveyData = new int[] {-1, -1, -1, -1, -1, -1, -1, -1}; //Survey answers as int array

    #region Survey data public fields

    public int a1
    {
        get => surveyData[0];
        set => surveyData[0] = value;
    }

    public int a2
    {
        get => surveyData[1];
        set => surveyData[1] = value;
    }

    public int a3
    {
        get => surveyData[2];
        set => surveyData[2] = value;
    }

    public int a4
    {
        get => surveyData[3];
        set => surveyData[3] = value;
    }

    public int a5
    {
        get => surveyData[4];
        set => surveyData[4] = value;
    }

    public int a6
    {
        get => surveyData[5];
        set => surveyData[5] = value;
    }

    public int a7
    {
        get => surveyData[6];
        set => surveyData[6] = value;
    }

    public int a8
    {
        get => surveyData[7];
        set => surveyData[7] = value;
    }

    #endregion

    public override IEnumerator Run()
    {
        surveyData = new int[surveyQuestions.Length];

        for (questionIndex = 0; questionIndex < surveyQuestions.Length; questionIndex++)
        {
            uiTextQuestion.text = surveyQuestions[questionIndex].question;

            DrawAnswers();
            yield return StartCoroutine(WaitForInput());

            ClearText();
            yield return new WaitForSeconds(1f);
        }

        //Submit survey data
        DataHandler.Instance.SubmitSurveyData(surveyName, surveyData);
    }

    private void DrawAnswers()
    {
        string answers = "";
        for (int i = 0; i < surveyQuestions[questionIndex].answers.Length; i++)
        {
            if (i == lastInput)
                answers += "<color=magenta>(" + (i + 1) + ") " + surveyQuestions[questionIndex].answers[i] +
                           "</color>\n";
            else
                answers += "(" + (i + 1) + ") " + surveyQuestions[questionIndex].answers[i] + "\n";
        }

        uiTextAnswer.text = answers;
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
        if (input >= surveyQuestions[questionIndex].answers.Length) return;

        if (input != lastInput)
        {
            lastInput = input; //Set last input first for "press same key twice" function
            DrawAnswers();
        }

        else
        {
            surveyData[questionIndex] = input;
            waitingForInput = false;

            lastInput = -1;
        }
    }

    private void ClearText()
    {
        uiTextQuestion.text = "";
        uiTextAnswer.text = "";
    }
}