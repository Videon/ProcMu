using System.Collections.Generic;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class SampleDb : ScriptableObject
    {
        private const int tabStartIdx = 900;

        private AudioClip[] audioClips;
        private Dictionary<string, int> sampleDb = new Dictionary<string, int>();

        public Dictionary<string, int> Db => sampleDb;

        /// <summary> Rebuilds sample database. </summary>
        private void UpdateDb()
        {
            int count = 0;

            foreach (AudioClip clip in audioClips)
            {
                sampleDb.Add(clip.name, tabStartIdx + count);

                count++;
            }
        }
    }
}