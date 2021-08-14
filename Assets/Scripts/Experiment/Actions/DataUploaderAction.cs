using System;
using System.Collections;
using UnityEngine;

public class DataUploaderAction : ExperimentAction
{
    [SerializeField] private JsonAzureTool azure;

    // Start is called before the first frame update
    public override IEnumerator Run()
    {
        string filename = GetUserId() + "_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")+ ".json";
        yield return StartCoroutine(azure.UploadJson(DataHandler.Instance.GetExperimentData(), filename));
    }

    /// <summary> Returns a unique user id. If no id exists, a new id is created. </summary>
    public static string GetUserId()
    {
        string userId;

        //Check if user id exists in player prefs
        if (PlayerPrefs.HasKey("UserId"))
        {
            userId = PlayerPrefs.GetString("UserId");

            //Check if id is invalid, and generate new one.
            if (userId.Length < 32)
            {
                userId = Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString("UserId", userId);
                PlayerPrefs.Save();
            }
        }
        else
        {
            userId = Guid.NewGuid().ToString("N");
            PlayerPrefs.SetString("UserId", userId);
            PlayerPrefs.Save();
        }

        return PlayerPrefs.GetString("UserId");
    }
}