using ProcMu.UnityScripts;
using UnityEngine;
using UnityEngine.Serialization;

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
        public int[] eucrth_minImpulses0 = new int[4];
        public int[] eucrth_maxImpulses0 = new int[4];
        public int[] eucrth_minImpulses1 = new int[4];
        public int[] eucrth_maxImpulses1 = new int[4];

        #endregion

        #region SNHMEL parameters

        public int snhmel_minOct = 0;
        public int snhmel_maxOct = 10;
        public double occurence = 2;

        #endregion

        #region CHORDS parameters

        public int chords_minOct;
        public int chords_maxOct;

        public int chords_minImpulses0;
        public int chords_maxImpulses0;
        public int chords_minImpulses1;
        public int chords_maxImpulses1;

        public ChordMode chordMode;

        public GSynthConfig chordsSynthConfig;

        #endregion
    }
}