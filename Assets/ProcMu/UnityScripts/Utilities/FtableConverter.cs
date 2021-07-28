using ProcMu.ScriptableObjects;
using UnityEngine;

namespace ProcMu.UnityScripts.Utilities
{
    public static class FtableConverter
    {
        public static double[] GlobalGenerateBars(MuConfig mc, float intensity)
        {
            double[] output = new double[64];

            for (int i = 0; i < output.Length; i++)
            {
                if (intensity <= 0.5f) output[i] = mc.activeBars0[i] ? 1 : -1;
                else output[i] = mc.activeBars1[i] ? 1 : -1;
            }

            return output;
        }

        public static double[] EucRthGenerateConfig(MuConfig mc, float intensity)
        {
            //Filling EucRth config
            double[] eucrthConfig = new double[8];
            for (int i = 0; i < 4; i++)
            {
                eucrthConfig[2 * i] = mc.eucrth_sampleSelection[i];
                eucrthConfig[2 * i + 1] = Mathf.RoundToInt(Random.Range(
                    (float) Mathf.RoundToInt(
                        Mathf.Lerp(mc.eucrth_minImpulses0[i], mc.eucrth_minImpulses1[i], intensity)),
                    (float) Mathf.RoundToInt(
                        Mathf.Lerp(mc.eucrth_maxImpulses0[i], mc.eucrth_maxImpulses1[i], intensity))
                ));
            }

            return eucrthConfig;
        }


        public static double[] ChordsGenerateConfig(MuConfig mc, float intensity)
        {
            double[] output = new double[8];

            //pulses
            output[0] = Mathf.RoundToInt(Random.Range(
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.chords_minImpulses0, mc.chords_minImpulses1, intensity)),
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.chords_maxImpulses0, mc.chords_maxImpulses1, intensity))
            ));
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = 0;
            output[6] = 0;
            output[7] = 0;

            return output;
        }

        public static double[] ChordsGenerateNotes(MuConfig mc, float intensity)
        {
            double[] output = new double[16];
            double[] notes = Chords.MakeChord(mc.Scale,
                Mathf.RoundToInt(Mathf.Lerp(mc.chords_minOct0, mc.chords_minOct1, intensity)),
                Mathf.RoundToInt(Mathf.Lerp(mc.chords_maxOct0, mc.chords_maxOct1, intensity)),
                mc.chordMode);
            notes.CopyTo(output, 0);

            for (int i = notes.Length; i < output.Length; i++)
                output[i] = -1;

            return output;
        }

        public static double[] SnhMelGenerateConfig(MuConfig mc, float intensity)
        {
            double[] output = new double[8];

            //pulses
            output[0] = Mathf.RoundToInt(Random.Range(
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_minImpulses0, mc.snhmel_minImpulses1, intensity)),
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_maxImpulses0, mc.snhmel_maxImpulses1, intensity))
            ));

            //occurence
            output[1] = Random.Range(
                Mathf.Lerp(mc.snhmel_minOccurence0, mc.snhmel_minOccurence1, intensity),
                Mathf.Lerp(mc.snhmel_maxOccurence0, mc.snhmel_maxOccurence1, intensity)
            );

            output[2] = Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_minOct0, mc.snhmel_minOct1, intensity)); //min oct
            output[3] = Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_maxOct0, mc.snhmel_maxOct1, intensity)); //max oct

            output[4] = (int) mc.snhmel_melodycurve; //melody (lfo) curve
            output[5] = (int) mc.snhmel_melodymode; //melody mode

            return output;
        }

        public static double[] SnhBasGenerateConfig(MuConfig mc, float intensity)
        {
            double[] output = new double[8];

            //pulses
            output[0] = Mathf.RoundToInt(Random.Range(
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.snhbas_minImpulses0, mc.snhbas_minImpulses1, intensity)),
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.snhbas_maxImpulses0, mc.snhbas_maxImpulses1, intensity))
            ));

            //occurence
            output[1] = Random.Range(
                Mathf.Lerp(mc.snhbas_minOccurence0, mc.snhbas_minOccurence1, intensity),
                Mathf.Lerp(mc.snhbas_maxOccurence0, mc.snhbas_maxOccurence1, intensity)
            );

            output[2] = Mathf.RoundToInt(Mathf.Lerp(mc.snhbas_minOct0, mc.snhbas_minOct1, intensity)); //min oct
            output[3] = Mathf.RoundToInt(Mathf.Lerp(mc.snhbas_maxOct0, mc.snhbas_maxOct1, intensity)); //max oct

            output[4] = (int) mc.snhbas_melodycurve; //melody (lfo) curve
            output[5] = (int) mc.snhbas_melodymode; //melody mode

            return output;
        }

        public static double[] GSynthGenerateConfig(GSynthConfig config)
        {
            double[] output = new double[32];
            output[0] = -1;
            output[1] = -1;
            output[2] = -1;
            output[3] = -1;
            output[4] = -1;
            output[5] = config.Velocity;
            output[6] = (int) config.Wavetable;
            output[7] = config.Noise; //noise amount
            output[8] = config.ffreq; //filter frequency
            output[9] = config.fres; //filter resonance

            output[10] = config.fenv_amt; //filter envelope amount
            output[11] = config.fenv_att; //filter attack
            output[12] = config.fenv_dec; //filter decay
            output[13] = config.fenv_sus; //filter sustain
            output[14] = config.fenv_rel; //filter release

            //amp variables
            output[15] = config.aenv_att; //amp attack
            output[16] = config.aenv_dec; //amp decay
            output[17] = config.aenv_sus; //amp sustain
            output[18] = config.aenv_rel; //amp release

            //additional variables
            output[19] = config.width; //stereo auto pan width

            output[20] = config.rev_amt; //reverb amount
            output[21] = config.rev_roomsize; //reverb room size
            output[22] = config.rev_damp; //reverb damp

            return output;
        }
    }
}