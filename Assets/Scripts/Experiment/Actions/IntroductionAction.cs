using System.Collections;
using UnityEngine;

/// <summary> Action for familiarizing the participant with the experiment environment. </summary>
public class IntroductionAction : ExperimentAction
{
    #region Area variables

    //Spawn point and barrier arrays must be of equal length!
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] barriers;

    #endregion

    [SerializeField, Tooltip("How much time the participant spends in an area in seconds.")]
    private float areaDuration = 60f;

    public override IEnumerator Run()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            conductor.MovePlayer(spawnPoints[i].position);
            barriers[i].SetActive(true);
            yield return StartCoroutine(ExploreArea());
            barriers[i].SetActive(false);
        }
    }

    private IEnumerator ExploreArea()
    {
        float elapsedTime = 0f;

        while (true)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= areaDuration) break;

            yield return new WaitForFixedUpdate();
        }
    }
}