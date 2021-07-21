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

        /// <summary> Contains active bar information for all instruments. 4 indices reserved per layer. </summary>
        public double[] activeBars = new double[128];

        public bool[] activeBars0 = new bool[64];
        public bool[] activeBars1 = new bool[64];

        #endregion

        #region EUCRTH parameters

        public int[] sampleSelection = new int[4];
        public int[] eucrth_minImpulses0 = new int[4];
        public int[] eucrth_maxImpulses0 = new int[4];
        public int[] eucrth_minImpulses1 = new int[4];
        public int[] eucrth_maxImpulses1 = new int[4];

        #endregion

        #region SNHMEL parameters

        public int snhmel_minImpulses0;
        public int snhmel_maxImpulses0;
        public int snhmel_minImpulses1;
        public int snhmel_maxImpulses1;

        public float snhmel_minOccurence0;
        public float snhmel_maxOccurence0;
        public float snhmel_minOccurence1;
        public float snhmel_maxOccurence1;

        public int snhmel_minOct0 = 0;
        public int snhmel_maxOct0 = 10;
        public int snhmel_minOct1;
        public int snhmel_maxOct1;

        public MelodyCurve snhmel_melodycurve;
        public MelodyMode snhmel_melodymode;

        public GSynthConfig snhmel_synthconfig;

        #endregion

        #region CHORDS parameters

        public int chords_minOct0;
        public int chords_maxOct0;
        public int chords_minOct1;
        public int chords_maxOct1;

        public int chords_minImpulses0;
        public int chords_maxImpulses0;
        public int chords_minImpulses1;
        public int chords_maxImpulses1;

        public ChordMode chordMode;

        public GSynthConfig chords_synthconfig;

        #endregion
    }
}