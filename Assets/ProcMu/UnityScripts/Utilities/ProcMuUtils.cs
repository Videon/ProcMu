using System;
using Unity.Mathematics;
using UnityEngine;

namespace ProcMu.UnityScripts.Utilities
{
    public static class ProcMuUtils
    {
        /// <summary> Converts scale in boolean form to midi-style note number for use in Csound. </summary>
        public static double[] ConvertScale(bool[] input)
        {
            double[] output = new double[input.Length];
            int index = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i])
                {
                    output[index] = i;
                    index++;
                }
            }

            for (int i = index; i < output.Length; i++)
            {
                output[i] = -1;
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
    }
}