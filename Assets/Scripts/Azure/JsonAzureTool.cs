using System;
using System.Collections;
using System.IO;
using RESTClient;
using Azure.StorageServices;
using UnityEngine;

public class JsonAzureTool : MonoBehaviour
{
    [Header("Azure Storage Service")] [SerializeField]
    private string storageAccount;

    [SerializeField] private string accessKey;
    [SerializeField] private string container;

    private StorageServiceClient client;
    private BlobService blobService;

    // Use this for initialization
    void Start()
    {
        if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey))
        {
            Debug.LogError("Storage account and access key are required");
        }

        client = StorageServiceClient.Create(storageAccount, accessKey);
        blobService = client.GetBlobService();
    }

    public IEnumerator UploadJson(object data, string filename)
    {
        string text = JsonUtility.ToJson(data);
        yield return StartCoroutine(blobService.PutJsonBlob(PutJsonBlobComplete, text, container, filename
        ));
    }

    private void PutJsonBlobComplete(RestResponse response)
    {
        if (response.IsError)
        {
            Debug.LogError("Error putting blob: " + response.Content);
            return;
        }

        Debug.Log("Put blob status:" + response.StatusCode);
    }

    private void GetTextBlobComplete(RestResponse response)
    {
        if (response.IsError)
        {
            Debug.LogError(response.ErrorMessage + " Error getting blob:" + response.Content);
            return;
        }

        Debug.Log("Get blob:" + response.Content);
    }

    private object GetJsonBlobComplete(RestResponse response)
    {
        if (response.IsError)
        {
            Debug.LogError(response.ErrorMessage + " Error getting blob:" + response.Content);
            return null;
        }

        return JsonUtility.FromJson(response.Content, typeof(RoundData));
    }

    public RoundData LoadData(string filename)
    {
        RoundData rd = new RoundData();

        string json = File.ReadAllText(Application.persistentDataPath + "/analytics/" + filename + ".json");

        rd = JsonUtility.FromJson<RoundData>(json);

        return rd;
    }

    /// <summary>s Returns a guid as file name. </summary>
    private string GenerateFilename()
    {
        string filename = Guid.NewGuid().ToString("N");
        filename += "_" + DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss");

        return filename;
    }
}