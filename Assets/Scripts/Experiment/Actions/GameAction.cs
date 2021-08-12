using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAction : ExperimentAction
{
    [SerializeField] private string gameName;
    [SerializeField] private bool showTarget; //Indicates whether player is told where to go

    [SerializeField] private Transform playerTransform;

    [SerializeField, Tooltip("Where player will be spawned at the beginning of each round.")]
    private Transform playerSpawnPoint;

    [SerializeField, Tooltip("How often all spawn points must be visited.")]
    private int cycles;

    [SerializeField] private Transform[] objectSpawnPoints;

    [SerializeField] private Transform goalObject;

    [SerializeField, Tooltip("Max amount of time the player has to find the object.")]
    private float timeLimit = 300f;

    [SerializeField] private MuGuidance guidance; //TODO send round target and elapsedtime to guidance.
    [SerializeField] private bool useGuidance = false;


    #region Private variables

    private float[] gameData; //Time taken to find the object per round (round is equivalent to index)

    private List<int> spawnIndices = new List<int>();

    private float elapsedTime = 0f;

    private int currentRound = 0;

    private bool waitingForInput = false;

    #endregion

    #region UI variables

    [SerializeField] private Text uiTextQuestion;
    [SerializeField] private Text uiTextAnswer;

    #endregion


    public override IEnumerator Run()
    {
        gameData = new float[objectSpawnPoints.Length * cycles];

        goalObject.GetComponent<ColliderEvent>().SetMessageTarget(gameObject);

        GenerateSpawnOrder();

        for (currentRound = 0; currentRound < spawnIndices.Count; currentRound++)
        {
            StartRound(currentRound);

            //Get current round number by checking saved rounds.
            int currentSet = DataHandler.Instance.GetExperimentData().roundDatas.Count + 1;
            uiTextAnswer.text = "Set " + currentSet + " - Round " + (currentRound + 1);

            if (showTarget)
                uiTextQuestion.text = "Go to the " + (ZoneNames) spawnIndices[currentRound] + " area!";
            yield return StartCoroutine(WaitForFinishRound());

            uiTextQuestion.text = "";
            uiTextAnswer.text = "";
        }

        goalObject.position = new Vector3(0, -20, 0);

        //Submits data of currently recorded rounds for saving/upload
        DataHandler.Instance.SubmitRoundData(gameName, gameData);
    }

    private void GenerateSpawnOrder()
    {
        spawnIndices.Clear();

        List<int> temp = new List<int>();

        for (int i = 0; i < cycles; i++)
        {
            for (int j = 0; j < objectSpawnPoints.Length; j++)
            {
                temp.Add(j);
            }
        }

        //TODO adjust randomization in such a way that there are no double successive occurences when cycles > 1
        int count = temp.Count;
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, temp.Count);
            spawnIndices.Add(temp[index]);
            temp.RemoveAt(index);
        }
    }

    public void FinishRound()
    {
        gameData[currentRound] = elapsedTime; //Save total round time to game data

        waitingForInput = false;

        if (useGuidance) guidance.StopGuidance();
    }

    private void StartRound(int spawnIndex)
    {
        conductor.MovePlayer(playerSpawnPoint.position);
        goalObject.position = objectSpawnPoints[spawnIndices[spawnIndex]].position;

        elapsedTime = 0; //Reset time counter

        if (useGuidance)
        {
            guidance.UpdateTargetPosition(objectSpawnPoints[spawnIndices[spawnIndex]].position);
            guidance.StartGuidance();
        }
    }

    private IEnumerator WaitForFinishRound()
    {
        waitingForInput = true;
        while (waitingForInput)
        {
            if (elapsedTime >= timeLimit) FinishRound(); //Finish round automatically if time limit exceeded.

            elapsedTime += Time.deltaTime;

            guidance.UpdateElapsedTime(elapsedTime);

            yield return new WaitForFixedUpdate();
        }
    }

    private enum ZoneNames
    {
        Yellow,
        Blue,
        Red,
        Green
    }
}