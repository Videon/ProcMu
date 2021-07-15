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
        [SerializeField, Range(0, 1000)] private double bpm = 120;
        [SerializeField, Range(0f, 1f)] private float intensity = 1;

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

            if (csoundUnity.GetChannel("update") > 0)
                UpdateCsound();

            SetChannels();
        }

        /// <summary> Csound update is triggered once every bar (16 steps). </summary>
        void UpdateCsound()
        {
            return; //TODO FIX CHANNEL UPDATING ONLY ONCE
            csoundUnity.SetChannel("update", 0);

            csoundUnity.CopyTableIn(831, ChordsGenerateNotes());
        }

        /// <summary> Sends information to Csound. </summary>
        private void SetChannels()
        {
            //Global variables
            csoundUnity.SetChannel("gBpm", bpm);
            csoundUnity.SetChannel("gIntensity", intensity);


            //Set scale table / TODO Only execute when scale has changed
            csoundUnity.CopyTableIn(800, ProcMuUtils.ConvertScale(mc.Scale.Scale));


            csoundUnity.CopyTableIn(810, EucRthGenerateConfig());
            csoundUnity.CopyTableIn(820, SnhMelGenerateConfig());

            csoundUnity.CopyTableIn(831, ChordsGenerateNotes());
        }

        private double[] EucRthGenerateConfig()
        {
            //Filling EucRth config
            double[] eucrthConfig = new double[8];
            for (int i = 0; i < 4; i++)
            {
                eucrthConfig[2 * i] = mc.sampleSelection[i];
                eucrthConfig[2 * i + 1] = Mathf.RoundToInt(Random.Range(
                    (float) Mathf.RoundToInt(Mathf.Lerp(mc.minImpulses0[i], mc.minImpulses1[i], intensity)),
                    (float) Mathf.RoundToInt(Mathf.Lerp(mc.maxImpulses0[i], mc.maxImpulses1[i], intensity))
                ));
            }

            return eucrthConfig;
        }

        private double[] SnhMelGenerateConfig()
        {
            double[] snhmelConfig = new double[3];
            snhmelConfig[0] = mc.snhmel_minOct;
            snhmelConfig[1] = mc.snhmel_maxOct;
            snhmelConfig[2] = mc.occurence;

            return snhmelConfig;
        }

        private double[] ChordsGenerateNotes()
        {
            double[] output = new double[16];
            double[] notes =
                Chords.MakeChord(mc.Scale, mc.snhmel_minOct, mc.snhmel_maxOct, mc.chordMode);
            notes.CopyTo(output, 0);

            for (int i = notes.Length; i < output.Length; i++)
                output[i] = -1;

            return output;
        }
    }
}