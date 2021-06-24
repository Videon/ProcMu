using System.Collections;
using ProcMu.UnityScripts.Utilities;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class ProcMuMaster : MonoBehaviour
    {
        [SerializeField] private CsoundUnity csoundUnity;
        private bool _isInitialized;

        [SerializeField] private AudioClip[] audioClips;

        //Test variables
        [SerializeField] private double bpm = 120;
        [SerializeField] private double intensity = 1;

        [SerializeField] private MuConfig mc;

        private void Awake()
        {
            //Assign CsoundUnity component
            if (!csoundUnity) Debug.LogError("Can't find CsoundUnity component!");
        }


        // Start is called before the first frame update
        IEnumerator Start()
        {
            _isInitialized = false;
            yield return StartCoroutine(Initialize());
            _isInitialized = true;
        }

        IEnumerator Initialize()
        {
            while (!csoundUnity.IsInitialized)
            {
                yield return null;
            }

            yield return StartCoroutine(InitSamples());
        }

        private IEnumerator InitSamples()
        {
            //Load samples into Csound
            int count = 0;

            foreach (var clip in audioClips)
            {
                string clipName = "samples/" + clip.name;

                double[] samples = CsoundUnity.GetSamples(clipName, CsoundUnity.SamplesOrigin.Resources);

                if (samples.Length > 0)
                {
                    int tableNum = 900 + count;

                    int nChan = clip.channels;
                    int res = csoundUnity.CreateTable(tableNum, samples);

                    csoundUnity.SetChannel($"sampletable{tableNum}", tableNum);

                    count++;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_isInitialized)
                return;
            SetChannels();
        }

        /// <summary> Sends information to Csound. </summary>
        private void SetChannels()
        {
            //Global variables
            csoundUnity.SetChannel("gBpm", bpm);
            csoundUnity.SetChannel("gIntensity", intensity);


            //Set scale table / TODO Only execute when scale has changed
            csoundUnity.CopyTableIn(800, ProcMuUtils.ConvertScale(mc.Scale.Scale));
            //csoundUnity.SetChannel("gScale", 800);

            //Eucrth variables
        }
    }
}