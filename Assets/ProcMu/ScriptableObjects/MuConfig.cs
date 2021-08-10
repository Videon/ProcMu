using ProcMu.UnityScripts;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    [CreateAssetMenu(fileName = "muc_", menuName = "ProcMu/Music configuration", order = 1)]
    public class MuConfig : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary> Copy of Csound to Unity communication table, set by ProcMuMaster. Only needed for editor visualization.
        /// Parameters: 0 = update, 1 = active bar, 2 = active step</summary>
        public double[] comm = new double[4];
#endif

        #region Global parameters

        [SerializeField] public double bpm = 120;
        public MuScale Scale;

        /// <summary> Contains active bar information for all instruments. 4 indices reserved per layer. </summary>
        public bool[] activeBars0 = new bool[64];

        public bool[] activeBars1 = new bool[64];

        #endregion

        #region EUCRTH parameters

        public int[] eucrth_sampleSelection = new int[4];
        public int[] eucrth_minImpulses0 = new int[4];
        public int[] eucrth_maxImpulses0 = new int[4];
        public int[] eucrth_minImpulses1 = new int[4];
        public int[] eucrth_maxImpulses1 = new int[4];

        #endregion

        #region CHORDS parameters

        public double[] chords_config = new double[16];

        public int chords_minOct0
        {
            get => (int) chords_config[0];
            set => chords_config[0] = value;
        }

        public int chords_maxOct0
        {
            get => (int) chords_config[1];
            set => chords_config[1] = value;
        }

        public int chords_minOct1
        {
            get => (int) chords_config[2];
            set => chords_config[2] = value;
        }

        public int chords_maxOct1
        {
            get => (int) chords_config[3];
            set => chords_config[3] = value;
        }

        public int chords_minImpulses0
        {
            get => (int) chords_config[4];
            set => chords_config[4] = value;
        }

        public int chords_maxImpulses0
        {
            get => (int) chords_config[5];
            set => chords_config[5] = value;
        }

        public int chords_minImpulses1
        {
            get => (int) chords_config[6];
            set => chords_config[6] = value;
        }

        public int chords_maxImpulses1
        {
            get => (int) chords_config[7];
            set => chords_config[7] = value;
        }

        public ChordMode chordMode;

        public GSynthConfig chords_synthconfig;

        #endregion

        #region SNHMEL parameters

        public double[] snhmel_config = new double[16];

        public int snhmel_minImpulses0
        {
            get => (int) snhmel_config[0];
            set => snhmel_config[0] = value;
        }

        public int snhmel_maxImpulses0
        {
            get => (int) snhmel_config[1];
            set => snhmel_config[1] = value;
        }

        public int snhmel_minImpulses1
        {
            get => (int) snhmel_config[2];
            set => snhmel_config[2] = value;
        }

        public int snhmel_maxImpulses1
        {
            get => (int) snhmel_config[3];
            set => snhmel_config[3] = value;
        }

        public float snhmel_minOccurence0
        {
            get => (float) snhmel_config[4];
            set => snhmel_config[4] = value;
        }

        public float snhmel_maxOccurence0
        {
            get => (float) snhmel_config[5];
            set => snhmel_config[5] = value;
        }

        public float snhmel_minOccurence1
        {
            get => (float) snhmel_config[6];
            set => snhmel_config[6] = value;
        }

        public float snhmel_maxOccurence1
        {
            get => (float) snhmel_config[7];
            set => snhmel_config[7] = value;
        }

        public int snhmel_minOct0
        {
            get => (int) snhmel_config[8];
            set => snhmel_config[8] = value;
        }

        public int snhmel_maxOct0
        {
            get => (int) snhmel_config[9];
            set => snhmel_config[9] = value;
        }

        public int snhmel_minOct1
        {
            get => (int) snhmel_config[10];
            set => snhmel_config[10] = value;
        }

        public int snhmel_maxOct1
        {
            get => (int) snhmel_config[11];
            set => snhmel_config[11] = value;
        }

        public MelodyCurve snhmel_melodycurve;
        public MelodyMode snhmel_melodymode;

        public GSynthConfig snhmel_synthconfig;

        #endregion

        #region SNHBAS parameters

        public double[] snhbas_config = new double[16];

        public int snhbas_minImpulses0
        {
            get => (int) snhbas_config[0];

            set => snhbas_config[0] = value;
        }

        public int snhbas_maxImpulses0
        {
            get => (int) snhbas_config[1];

            set => snhbas_config[1] = value;
        }

        public int snhbas_minImpulses1
        {
            get => (int) snhbas_config[2];

            set => snhbas_config[2] = value;
        }

        public int snhbas_maxImpulses1
        {
            get => (int) snhbas_config[3];

            set => snhbas_config[3] = value;
        }

        public float snhbas_minOccurence0
        {
            get => (float) snhbas_config[4];

            set => snhbas_config[4] = value;
        }

        public float snhbas_maxOccurence0
        {
            get => (float) snhbas_config[5];

            set => snhbas_config[5] = value;
        }

        public float snhbas_minOccurence1
        {
            get => (float) snhbas_config[6];

            set => snhbas_config[6] = value;
        }

        public float snhbas_maxOccurence1
        {
            get => (float) snhbas_config[7];

            set => snhbas_config[7] = value;
        }

        public int snhbas_minOct0
        {
            get => (int) snhbas_config[8];

            set => snhbas_config[8] = value;
        }

        public int snhbas_maxOct0
        {
            get => (int) snhbas_config[9];

            set => snhbas_config[9] = value;
        }

        public int snhbas_minOct1
        {
            get => (int) snhbas_config[10];

            set => snhbas_config[10] = value;
        }

        public int snhbas_maxOct1
        {
            get => (int) snhbas_config[11];

            set => snhbas_config[11] = value;
        }

        public MelodyCurve snhbas_melodycurve;
        public MelodyMode snhbas_melodymode;

        public GSynthConfig snhbas_synthconfig;

        #endregion
    }
}