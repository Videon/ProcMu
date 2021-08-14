using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzureTester : MonoBehaviour
{
    [SerializeField] private JsonAzureTool azure;

    [SerializeField] private string roundDataName = "testData";
    [SerializeField] private float[] roundData;

    [SerializeField] private string surveyDataName = "testData";
    [SerializeField] private int[] surveyData;

    public void SubmitRoundData()
    {
        DataHandler.Instance.SubmitRoundData(roundDataName, roundData);
    }

    public void SubmitSurveyData()
    {
        DataHandler.Instance.SubmitSurveyData(surveyDataName, surveyData);
    }

    public void Upload()
    {
        Debug.Log("Testing...");

        StartCoroutine(TestUpload());
    }

    IEnumerator TestUpload()
    {
        string filename = DataUploaderAction.GetUserId() + "_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") +
                          "_test.json";
        yield return StartCoroutine(azure.UploadJson(DataHandler.Instance.GetExperimentData(), filename));
    }

    public void ClearData()
    {
        DataHandler.Instance.ClearAllData();
    }
}