using ProcMu.UnityScripts;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    [CreateAssetMenu(fileName = "gsc_", menuName = "ProcMu/Game synth configuration", order = 1)]
    public class GSynthConfig : ScriptableObject
    {
        #region General config

        public double velocity;
        public Wavetable wavetable;
        public double noise;

        #endregion

        #region Filter config

        public double ffreq; //LP filter frequency
        public double fres; //LP filter resonance

        public double fenv_amt; //filter envelope amount

        public double fenv_att; //filter attack
        public double fenv_dec; //filter decay
        public double fenv_sus; //filter sustain
        public double fenv_rel; //filter release

        #endregion

        #region Amplitude config

        public double aenv_att; //amp attack
        public double aenv_dec; //amp decay
        public double aenv_sus; //amp sustain
        public double aenv_rel; //amp release

        #endregion
    }
}