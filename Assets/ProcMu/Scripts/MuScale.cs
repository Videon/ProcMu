using UnityEngine;

namespace ProcMu.CSUnity
{
    [CreateAssetMenu(fileName = "mus_", menuName = "ProcMu/Scale", order = 1)]
    public class MuScale : ScriptableObject
    {
        [SerializeField] public bool[] Scale = new bool[128];
    }
}