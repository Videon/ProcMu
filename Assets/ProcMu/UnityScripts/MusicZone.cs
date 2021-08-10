using ProcMu.ScriptableObjects;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class MusicZone : MonoBehaviour
    {
        [SerializeField] public float RadiusInner = 10f;
        [SerializeField] public float radiusOuter = 40f;

        public MuConfig Config;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, RadiusInner);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radiusOuter);
        }
    }
}