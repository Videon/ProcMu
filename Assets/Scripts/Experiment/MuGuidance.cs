using System.Collections;
using UnityEngine;

public class MuGuidance : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Transform guidanceTransform;

    [SerializeField] private Vector3 targetPosition;

    [SerializeField] float guidanceIntensity;

    [SerializeField, Tooltip("The time in seconds it takes the guidance system to go from 0 to 1 intensity.")]
    private float guidanceTime = 120f;

    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (playerTransform == null) return;
        if (guidanceTransform == null) return;
        StartCoroutine(UpdateGuidance());
    }

    // Update is called once per frame
    private IEnumerator UpdateGuidance()
    {
        while (true)
        {
            guidanceIntensity = Mathf.Clamp(elapsedTime, 0f, guidanceTime) / guidanceTime;
            guidanceTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, guidanceIntensity);
            yield return new WaitForFixedUpdate();
        }
    }

    public void UpdateTargetPosition(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }

    public void UpdateElapsedTime(float elapsedTime)
    {
        this.elapsedTime = elapsedTime;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        if (guidanceTransform == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(playerTransform.position, targetPosition);
        Gizmos.DrawSphere(guidanceTransform.position, 2f);
    }
#endif
}