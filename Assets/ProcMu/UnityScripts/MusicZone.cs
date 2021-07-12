using System;
using ProcMu.ScriptableObjects;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class MusicZone : MonoBehaviour
    {
        [SerializeField] private float radiusInner = 10f;
        [SerializeField] private float radiusOuter = 40f;


        public MuConfig Config;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusInner);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radiusOuter);
        }
    }
}