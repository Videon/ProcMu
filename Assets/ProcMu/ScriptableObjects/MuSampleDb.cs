using UnityEditor;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    public class MuSampleDb : ScriptableObject
    {
        public AudioClip[] audioClips;
        public string[] audioClipNames;
        public int[] sampleIds;

#if UNITY_EDITOR
        public void FillDb(AudioClip[] audioClips)
        {
            this.audioClips = audioClips;

            audioClipNames = new string[this.audioClips.Length];
            sampleIds = new int[this.audioClips.Length];

            for (int i = 0; i < audioClips.Length; i++)
            {
                audioClipNames[i] = audioClips[i].name;
                sampleIds[i] = i;
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}