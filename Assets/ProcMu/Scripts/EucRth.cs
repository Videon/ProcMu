using System.Collections;
using UnityEngine;

namespace ProcMu.CSUnity
{
    public class EucRth : MonoBehaviour
    {
        private CsoundUnity _csoundUnity;
        private bool _isInitialized;

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

            foreach (var clip in audioClips)
            {
                string clipName = "samples/" + clip.name;

                double[] samples = CsoundUnity.GetSamples(clipName, CsoundUnity.SamplesOrigin.Resources);

                if (samples.Length > 0)
                {
                    int nChan = clip.channels;
                    int tableNum = 900 + count;
                    int res = _csoundUnity.CreateTable(tableNum, samples);

                    _csoundUnity.SetChannel($"sampletable{tableNum}", tableNum);

                    count++;
                }

                yield return new WaitForEndOfFrame();
            }

            _isInitialized = true;
        }
    }
}