using UnityEditor;
using UnityEngine;

namespace ProcMu.ScriptableObjects
{
    [FilePath("Assets/ProcMu/SamplesDB", FilePathAttribute.Location.ProjectFolder)]
    public class MuSampleDb : ScriptableSingleton<MuSampleDb>
    {
        public AudioClip[] audioClips;
        public string[] audioClipNames;
        public int[] sampleIds;

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
        }

        public void SaveAsset()
        {
            Save(true);
        }
    }
}