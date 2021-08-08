using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuGuidance : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Transform guidanceTransform;

    [SerializeField] private Vector3 targetPosition;

    [SerializeField] float guidanceIntensity;

    // Start is called before the first frame update
    void Start()
    {
        if (playerTransform == null) return;
        if (guidanceTransform == null) return;
        StartCoroutine(UpdateGuidancePosition());
    }

    // Update is called once per frame
    private IEnumerator UpdateGuidancePosition()
    {
        while (true)
        {
            guidanceTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, guidanceIntensity);
            yield return new WaitForFixedUpdate();
        }
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