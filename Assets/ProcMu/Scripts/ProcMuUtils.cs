namespace ProcMu.CSUnity
{
    public class ProcMuUtils
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
    }
}