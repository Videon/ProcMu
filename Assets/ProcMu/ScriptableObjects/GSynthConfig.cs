using ProcMu.UnityScripts;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    [CreateAssetMenu(fileName = "gsc_", menuName = "ProcMu/Game synth configuration", order = 1)]
    public class GSynthConfig : ScriptableObject
    {
        public double[] config = new double[32];

        #region General config

        /// <summary> Note velocity. Range 0, 1 </summary>
        public double Velocity
        {
            get => config[5];
            set => config[5] = value;
        }

        /// <summary> Note wavetable. </summary>
        public Wavetable Wavetable
        {
            get => (Wavetable) ((int) config[6]);
            set => config[6] = (double) value;
        }

        /// <summary>Noise amplitude. Range 0, 1 </summary>
        public double Noise
        {
            get => config[7];
            set => config[7] = value;
        }

        #endregion

        #region Filter config

        /// <summary> Range 0, 20000 </summary>
        public double ffreq
        {
            get => config[8];
            set => config[8] = value;
        } //LP filter frequency

        /// <summary>LP filter resonance. Range 0.5, 25 </summary>
        public double fres
        {
            get => config[9];
            set => config[9] = value;
        }

        /// <summary>LP filter envelope amount. Range 0, 20000 </summary>
        public double fenv_amt
        {
            get => config[10];
            set => config[10] = value;
        }

        /// <summary>LP filter envelope attack time. Range 0, 10 </summary>
        public double fenv_att
        {
            get => config[11];
            set => config[11] = value;
        }

        /// <summary>LP filter envelope decay time. Range 0, 10 </summary>
        public double fenv_dec
        {
            get => config[12];
            set => config[12] = value;
        }

        /// <summary>LP filter envelope sustain level. Range 0, 1 </summary>
        public double fenv_sus
        {
            get => config[13];
            set => config[13] = value;
        }

        /// <summary>LP filter envelope release time. Range 0, 10 </summary>
        public double fenv_rel
        {
            get => config[14];
            set => config[14] = value;
        }

        #endregion

        #region Amplitude config

        /// <summary>Amp envelope attack time. Range 0, 10 </summary>
        public double aenv_att
        {
            get => config[15];
            set => config[15] = value;
        }

        /// <summary>Amp envelope decay time. Range 0, 10 </summary>
        public double aenv_dec
        {
            get => config[16];
            set => config[16] = value;
        }

        /// <summary>Amp envelope sustain level. Range 0, 1 </summary>
        public double aenv_sus
        {
            get => config[17];
            set => config[17] = value;
        }

        /// <summary>Amp envelope release time. Range 0, 10 </summary>
        public double aenv_rel
        {
            get => config[18];
            set => config[18] = value;
        }

        #endregion

        #region Additional config

        /// <summary> Auto pan stereo width. Range 0, 1 </summary>
        public double width
        {
            get => config[19];
            set => config[19] = value;
        } //stereo auto pan width

        /// <summary> Reverb fx amount. Range 0, 1 </summary>
        public double rev_amt
        {
            get => config[20];
            set => config[20] = value;
        }

        /// <summary> Reverb fx room size. Range 0, 1 </summary>
        public double rev_roomsize
        {
            get => config[21];
            set => config[21] = value;
        }

        /// <summary> Reverb fx frequency damping. Range 0, 1 </summary>
        public double rev_damp
        {
            get => config[22];
            set => config[22] = value;
        }

        #endregion
    }
}