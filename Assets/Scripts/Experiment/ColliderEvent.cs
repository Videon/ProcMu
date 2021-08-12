using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    [SerializeField] private GameObject gameActionObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            gameActionObject.SendMessage("FinishRound");
    }

    public void SetMessageTarget(GameObject go)
    {
        gameActionObject = go;
    }
}