using System;
using System.Collections.Generic;
using Experiment;
using UnityEngine;

public class DataHandler : Singleton<DataHandler>
{
    private List<RoundData> roundDatas = new List<RoundData>();
    private List<SurveyData> surveyDatas = new List<SurveyData>();

    public void SubmitRoundData(string name, float[] roundData)
    {
        roundDatas.Add(new RoundData()
        {
            roundId = name,
            roundTime = roundData
        });
    }

    public void SubmitSurveyData(string name, int[] surveyData)
    {
        surveyDatas.Add(new SurveyData()
        {
            surveyId = name,
            answers = surveyData
        });
    }

    public void SaveAllData(string filename)
    {
        ExperimentData ed = new ExperimentData()
        {
            experimentType = ExperimentManager.Instance.experimentMode.ToString(),
            roundDatas = this.roundDatas,
            surveyDatas = this.surveyDatas
        };
        JsonHelper.SaveData(ed, filename); //Saves data locally
    }

    public ExperimentData GetExperimentData()
    {
        ExperimentData ed = new ExperimentData()
        {
            experimentType = ExperimentManager.Instance.experimentMode.ToString(),
            roundDatas = this.roundDatas,
            surveyDatas = this.surveyDatas
        };

        return ed;
    }

    public void ClearAllData()
    {
        roundDatas.Clear();
        surveyDatas.Clear();
    }
}

[Serializable]
public class RoundData
{
    /// <summary> Name of data set. </summary>
    public string roundId;

    /// <summary> How much time was taken to finish round. </summary>
    public float[] roundTime;
}

[Serializable]
public class SurveyData
{
    public string surveyId;
    public int[] answers;
}

[Serializable]
public class ExperimentData
{
    public string experimentType;
    public List<RoundData> roundDatas;
    public List<SurveyData> surveyDatas;
}