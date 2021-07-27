using ProcMu.ScriptableObjects;
using UnityEngine.iOS;

namespace ProcMu.UnityScripts.Utilities
{
    public static class ProcMuUtils
    {
        /// <summary> Converts scale in boolean form to midi-style note number for use in Csound. </summary>
        public static double[] ConvertScale(bool[] input)
        {
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] ? 1 : -1;
            }

            return output;
        }

        /// <summary> Takes indexes of active notes in scale and puts them into an array. </summary>
        public static int[] ScaleActiveNotes(bool[] input)
        {
            int[] output = new int[input.Length];

            int index = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i])
                {
                    output[index] = i;
                    index++;
                }
            }

            //Fill remaining fields with -1 to indicate no more notes available.
            for (int i = index; i < output.Length - 1; i++)
            {
                output[i] = -1;
            }

            output[output.Length - 1] =
                index; //Last index indicates number of active notes. Needed for certain features in csound, e.g. lfo range to select notes.

            return output;
        }

        public static double[] Int2Double(int[] input)
        {
            double[] output = new double[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i];
            }

            return output;
        }

        /// <summary>
        /// Interpolates between two scales by finding common notes,
        /// removing notes from scaleA until only common notes remain (0.0-0.5),
        /// adding notes from scaleB until only it is played (0.5-1.0).
        /// Both scale arrays must be the same length!
        /// </summary>
        /// <param name="scaleA">Origin scale.</param>
        /// <param name="scaleB">Target scale. Must be same length as scaleA.</param>
        /// <param name="t">Interpolation value.</param>
        public static bool[] LerpScales(bool[] scaleA, bool[] scaleB, float t)
        {
            //Creating the intermediate scale
            bool[] output = new bool[scaleA.Length];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = scaleA[i] == scaleB[i];
            }

            if (t < 0.4f)
                return scaleA;
            if (t < 0.6f)
                return output;

            return scaleB;
        }

        public static void GenerateScale(Tonic t, Scale s, ref bool[] activeNotes)
        {
            activeNotes = new bool[activeNotes.Length]; //Reset array

            int offset = (int) t;

            int scaleCnt = 0;

            int[] scale = {0};

            switch (s)
            {
                case Scale.Lydian:
                    scale = new[] {2, 2, 2, 1, 2, 2, 1};
                    break;
                case Scale.Ionian_Major:
                    scale = new[] {2, 2, 1, 2, 2, 2, 1};
                    break;
                case Scale.Mixolydian:
                    scale = new[] {2, 2, 1, 2, 2, 1, 2};
                    break;
                case Scale.Dorian:
                    scale = new[] {2, 1, 2, 2, 2, 1, 2};
                    break;
                case Scale.Aeolian_Minor:
                    scale = new[] {2, 1, 2, 2, 1, 2, 2};
                    break;
                case Scale.Phrygian:
                    scale = new[] {1, 2, 2, 2, 1, 2, 2};
                    break;
                case Scale.Locrian:
                    scale = new[] {1, 2, 2, 1, 2, 2, 2};
                    break;
            }

            int index = offset;
            while (index < activeNotes.Length)
            {
                activeNotes[index] = true;
                index += scale[scaleCnt];
                scaleCnt++;
                scaleCnt %= scale.Length; //Performing mod operation to wrap index
            }
        }

        /// <summary> Comparer for use with Array.Sort, sorting arrays according to distance, in ascending order. </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns>1 if m1 is larger than m2, or equal, or null. -1 if m1 is smaller than m2 or m2 is null.</returns>
        public static int CompareMcDists(McDist m1, McDist m2)
        {
            //Checking for null will ensure that null values rise to the top of the array when sorting
            if (m1.Mc == null) return 1; //Move up if first parameter is null

            if (m2.Mc == null) return -1; //Move down if second parameter is null

            //Now that null cases have been handled, the actual value comparison follows
            if (m1.Dist < m2.Dist) return -1;

            return 1;
        }

        public static int CompareMcDistsInverse(McDist m1, McDist m2)
        {
            //Checking for null will ensure that null values rise to the top of the array when sorting
            if (m1.Mc == null) return 1; //Move up if first parameter is null

            if (m2.Mc == null) return -1; //Move down if second parameter is null

            //Now that null cases have been handled, the actual value comparison follows
            if (m1.Dist < m2.Dist) return 1;

            return -1;
        }

        /// <summary> Copies contents of one MuConfig to another. </summary>
        /// <param name="from"> Origin MuConfig. </param>
        /// <param name="to">Target MuConfig. </param>
        public static void CopyMuConfig(MuConfig from, MuConfig to)
        {
            //GLOBAL
            to.bpm = from.bpm;
            to.Scale = from.Scale;
            to.activeBars0 = from.activeBars0;
            to.activeBars1 = from.activeBars1;

            //EUCRTH
            to.sampleSelection = from.sampleSelection;
            to.eucrth_minImpulses0 = from.eucrth_minImpulses0;
            to.eucrth_maxImpulses0 = from.eucrth_maxImpulses0;
            to.eucrth_minImpulses1 = from.eucrth_minImpulses1;
            to.eucrth_maxImpulses1 = from.eucrth_maxImpulses1;

            //CHORDS
            to.chords_minOct0 = from.chords_minOct0;
            to.chords_maxOct0 = from.chords_maxOct0;
            to.chords_minOct1 = from.chords_minOct1;
            to.chords_maxOct1 = from.chords_maxOct1;

            to.chords_minImpulses0 = from.chords_minImpulses0;
            to.chords_maxImpulses0 = from.chords_maxImpulses0;
            to.chords_minImpulses1 = from.chords_minImpulses1;
            to.chords_maxImpulses1 = from.chords_maxImpulses1;

            to.chordMode = from.chordMode;
            CopyGSynthConfig(from.chords_synthconfig, to.chords_synthconfig);

            //SNHMEL
            to.snhmel_minImpulses0 = from.snhmel_minImpulses0;
            to.snhmel_maxImpulses0 = from.snhmel_maxImpulses0;
            to.snhmel_minImpulses1 = from.snhmel_minImpulses1;
            to.snhmel_maxImpulses1 = from.snhmel_maxImpulses1;

            to.snhmel_minOccurence0 = from.snhmel_minOccurence0;
            to.snhmel_maxOccurence0 = from.snhmel_maxOccurence0;
            to.snhmel_minOccurence1 = from.snhmel_minOccurence1;
            to.snhmel_maxOccurence1 = from.snhmel_maxOccurence1;

            to.snhmel_melodycurve = from.snhmel_melodycurve;
            to.snhmel_melodymode = from.snhmel_melodymode;

            CopyGSynthConfig(from.snhmel_synthconfig, to.snhmel_synthconfig);
        }

        /// <summary> Copies contents of one GSynthConfig to another. </summary>
        /// <param name="from"> Origin GSynthConfig. </param>
        /// <param name="to">Target GSynthConfig. </param>
        public static void CopyGSynthConfig(GSynthConfig from, GSynthConfig to)
        {
            to.config = from.config;
        }

        public static void CopyScale(MuScale from, MuScale to)
        {
            to.Scale = from.Scale;
            to.ScaleNotes = from.ScaleNotes;
            to.tonic = from.tonic;
        }
    }
}