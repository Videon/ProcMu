using System.Collections.Generic;
using ProcMu.ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProcMu.UnityScripts.Utilities
{
    public static class Chords
    {
        /// <summary> Returns a chord, consisting of multiple notes from a given scale.</summary>
        /// <param name="scale">Scale to use for chord generation.</param>
        /// <param name="octMin">Lowest possible chord note as midi-like index number.</param>
        /// <param name="octMax">Highest possible chord note as midi-like index number.</param>
        /// <returns>A chord to be played by the CHORDS module.</returns>
        public static double[] MakeChord(MuScale scale, int octMin, int octMax, ChordMode chordMode)
        {
            switch (chordMode)
            {
                case ChordMode.Random357:
                    return MakeChordRandom357(scale, octMin, octMax, false);
                case ChordMode.Random357Oct:
                    return MakeChordRandom357(scale, octMin, octMax, true);
                case ChordMode.RandomRoot:
                    return MakeChordRandomRoot(scale, octMin, octMax, false);
                case ChordMode.RandomRootOct:
                    return MakeChordRandomRoot(scale, octMin, octMax, true);
                case ChordMode.Triads:
                    return MakeChordsTriads(scale, octMin, octMax, false);
                case ChordMode.TriadsOct:
                    return MakeChordsTriads(scale, octMin, octMax, true);
                default:
                    return new double[] {-1};
            }
        }

        private static double[] MakeChordRandom357(MuScale scale, int octMin, int octMax, bool oct)
        {
            List<double> chord = new List<double>();
            List<int> notes = new List<int>();

            //Gather all possible notes, multiplying by 12 and adding 11 to convert octave into midi indexes
            for (int i = octMin * 12; i < octMax * 12 + 11; i++)
            {
                if (scale.Scale[i])
                    notes.Add(i);
            }

            //Build chord
            chord.Add(notes[Random.Range(0, notes.Count)]); //Find base note

            int scaleIndex = (int) chord[0] + 3;

            if (scale.Scale[scaleIndex])
                chord.Add(scaleIndex);

            scaleIndex += 2;

            if (scale.Scale[scaleIndex])
                chord.Add(scaleIndex);

            scaleIndex += 2;

            //Add either root +7, or +9, whichever is possible, if at all
            if (scale.Scale[scaleIndex])
                chord.Add(scaleIndex);
            else
            {
                scaleIndex += 2;
                if (scale.Scale[scaleIndex])
                    chord.Add(scaleIndex);
            }

            if (oct) chord.Add(chord[0] - 12); //Add another note which is one octave below original root note.

            return chord.ToArray();
        }

        private static double[] MakeChordRandomRoot(MuScale scale, int octMin, int octMax, bool oct)
        {
            List<double> chord = new List<double>();
            List<int> notes = new List<int>();

            //Gather all possible notes TODO change to use active notes
            for (int i = octMin * 12; i < octMax * 12 + 11; i++)
            {
                if (scale.Scale[i])
                    notes.Add(i);
            }

            //Set base note from tonic
            chord.Add(notes[0]);

            int noteIndex = Random.Range(2, 4);

            if (noteIndex < notes.Count)
                chord.Add(notes[noteIndex]);

            noteIndex += Random.Range(1, 3);

            if (noteIndex < notes.Count)
                chord.Add(notes[noteIndex]);

            if (oct) chord.Add(chord[0] - 12); //Add another note which is one octave below original root note.

            return chord.ToArray();
        }

        private static double[] MakeChordsTriads(MuScale scale, int octMin, int octMax, bool oct)
        {
            List<double> chord = new List<double>();
            int[] triad = GetTriad(Random.Range(1, 8)); //Choose random triad, todo implement coherent succession
            int octave = Random.Range(octMin, octMax + 1);

            int root = octave * 12 + (int) scale.tonic;

            int noteIndex = 0;
            for (int i = 0; i < scale.ScaleNotes.Length; i++)
            {
                if (scale.ScaleNotes[i] == root) noteIndex = i;
            }

            for (int i = 0; i < triad.Length; i++)
            {
                chord.Add(scale.ScaleNotes[noteIndex + triad[i]]);
            }

            return chord.ToArray();
        }

        /// <summary> Returns a triad. </summary>
        /// <param name="numeral">Accepts numerals from 1 to 7. </param>
        /// <returns>Returns scale indices to be used with active notes as midi-like numbers.</returns>
        private static int[] GetTriad(int numeral)
        {
            int[] output;
            switch (numeral)
            {
                case 1:
                    output = new[] {0, 2, 4};
                    break;
                case 2:
                    output = new[] {1, 3, 5};
                    break;
                case 3:
                    output = new[] {2, 4, 6};
                    break;
                case 4:
                    output = new[] {0, 3, 5};
                    break;
                case 5:
                    output = new[] {1, 4, 6};
                    break;
                case 6:
                    output = new[] {0, 2, 5};
                    break;
                case 7:
                    output = new[] {1, 3, 6};
                    break;
                default:
                    output = new[] {-1};
                    break;
            }


            return output;
        }
    }
}