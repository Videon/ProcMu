using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcMu.CSUnity
{
    public class SampleManager : MonoBehaviour
    {
        private CsoundUnity _csoundUnity;
        private bool _isInitialized;

        [SerializeField] private SampleDb sampleDatabase;

        [SerializeField] private AudioClip[] audioClips;

        private void Awake()
        {
            //Assign CsoundUnity component
            _csoundUnity = GetComponent<CsoundUnity>();
            if (!_csoundUnity) Debug.LogError("Can't find CsoundUnity component!");
        }


        // Start is called before the first frame update
        IEnumerator Start()
        {
            _isInitialized = false;

            while (!_csoundUnity.IsInitialized)
            {
                yield return null;
            }

            //Load samples into Csound
            int count = 0;

            foreach (var key in sampleDatabase.Db)
            {
                string clipName = "samples/" + key.Key;

                double[] samples = CsoundUnity.GetSamples(clipName, CsoundUnity.SamplesOrigin.Resources);

                if (samples.Length > 0)
                {
                    //int nChan = clip.channels;
                    int res = _csoundUnity.CreateTable(key.Value, samples);

                    _csoundUnity.SetChannel($"sampletable{key.Value}", key.Value);

                    count++;
                }

                yield return new WaitForEndOfFrame();
            }

            _isInitialized = true;
        }
    }
}