using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveyAction : ExperimentAction
{
    [SerializeField] private SurveyQuestion[] surveyQuestions;

    private int lastInput = -1;
    private bool waitingForInput = false;
    private int questionIndex;

    private int[] surveyAnswers;

    public override IEnumerator Run()
    {
        surveyAnswers = new int[surveyQuestions.Length];

        for (questionIndex = 0; questionIndex < surveyQuestions.Length; questionIndex++)
        {
            //todo display question here
            Debug.Log("Showing question #" + questionIndex);
            yield return StartCoroutine(WaitForInput());
        }
    }

    private IEnumerator WaitForInput()
    {
        waitingForInput = true;
        while (waitingForInput)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    public override void SubmitInput(int value)
    {
        if (value >= surveyQuestions[questionIndex].answers.Length) return;

        if (value != lastInput) lastInput = value; //Set last input first for "press same key twice" function
        else waitingForInput = false;
    }
}