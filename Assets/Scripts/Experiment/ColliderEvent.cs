using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent targetEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            targetEvent.Invoke();
    }
}