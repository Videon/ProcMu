using System;
using System.Collections;
using UnityEngine;

public class DataUploaderAction : ExperimentAction
{
    [SerializeField] private JsonAzureTool azure;

    // Start is called before the first frame update
    public override IEnumerator Run()
    {
        string filename = Guid.NewGuid().ToString("N");
        yield return StartCoroutine(azure.UploadJson(DataHandler.Instance.GetExperimentData(), filename + ".json"));
    }
}