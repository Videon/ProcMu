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
        string filename = "test_" + Guid.NewGuid().ToString("N");
        yield return StartCoroutine(azure.UploadJson(DataHandler.Instance.GetExperimentData(), filename + ".json"));
    }

    public void ClearData()
    {
        DataHandler.Instance.ClearAllData();
    }
}