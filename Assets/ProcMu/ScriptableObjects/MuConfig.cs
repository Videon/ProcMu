using ProcMu.UnityScripts;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    [CreateAssetMenu(fileName = "muc_", menuName = "ProcMu/Music configuration", order = 1)]
    public class MuConfig : ScriptableObject
    {
        #region Global parameters

        [SerializeField] public double bpm = 120;
        public MuScale Scale;

        #endregion

        #region EUCRTH parameters

        public int[] sampleSelection = new int[4];
        public int[] minImpulses0 = new int[4];
        public int[] maxImpulses0 = new int[4];
        public int[] minImpulses1 = new int[4];
        public int[] maxImpulses1 = new int[4];

        #endregion
    }
}