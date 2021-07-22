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

            output[output.Length - 1] = index;  //Last index indicates number of active notes. Needed for certain features in csound, e.g. lfo range to select notes.

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
        
        
    }
}