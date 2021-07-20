using ProcMu.UnityScripts;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    [CreateAssetMenu(fileName = "gsc_", menuName = "ProcMu/Game synth configuration", order = 1)]
    public class GSynthConfig : ScriptableObject
    {
        #region General config

        [Range(0, 1)] public double velocity;
        public Wavetable wavetable;
        [Range(0, 1)] public double noise;

        #endregion

        #region Filter config

        [Range(0, 20000)] public double ffreq; //LP filter frequency
        [Range(0.5f, 25)] public double fres; //LP filter resonance

        [Range(0, 20000)] public double fenv_amt; //filter envelope amount

        [Range(0, 10)] public double fenv_att; //filter attack
        [Range(0, 10)] public double fenv_dec; //filter decay
        [Range(0, 1)] public double fenv_sus; //filter sustain
        [Range(0, 10)] public double fenv_rel; //filter release

        #endregion

        #region Amplitude config

        [Range(0, 10)] public double aenv_att; //amp attack
        [Range(0, 10)] public double aenv_dec; //amp decay
        [Range(0, 1)] public double aenv_sus; //amp sustain
        [Range(0, 10)] public double aenv_rel; //amp release

        #endregion

        #region Additional config

        [Range(0, 1)] public double width;

        #endregion
    }
}