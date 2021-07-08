using System.Collections;
using ProcMu.ScriptableObjects;
using ProcMu.UnityScripts.Utilities;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class ProcMuMaster : MonoBehaviour
    {
        [SerializeField] private CsoundUnity csoundUnity;
        private bool _isInitialized;

        private AudioClip[] audioClips;

        //Test variables
        [SerializeField] private double bpm = 120;
        [SerializeField] private float intensity = 1;

        public MuConfig mc;

        private void Awake()
        {
            audioClips = MuSampleDb.instance.audioClips; //Fetch audio clips from sample db
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

        /// <summary> Loads all samples in database into ram using Csound ftables. </summary>
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


            csoundUnity.CopyTableIn(801, EucRthGenerateConfig());

            //Eucrth variables
        }

        private double[] EucRthGenerateConfig()
        {
            //Filling EucRth config
            double[] eucrthConfig = new double[8];
            for (int i = 0; i < 4; i++)
            {
                eucrthConfig[2 * i] = mc.sampleSelection[i];
                eucrthConfig[2 * i + 1] = Mathf.RoundToInt(Random.Range(
                    Mathf.RoundToInt(Mathf.Lerp(mc.minImpulses0[i], mc.minImpulses1[i], intensity)),
                    Mathf.RoundToInt(Mathf.Lerp(mc.maxImpulses0[i], mc.maxImpulses1[i], intensity))
                ));
            }
            
            return eucrthConfig;
        }
    }
}