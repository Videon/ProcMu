using System;
using ProcMu.ScriptableObjects;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class MusicZone : MonoBehaviour
    {
        public MuConfig Config;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}