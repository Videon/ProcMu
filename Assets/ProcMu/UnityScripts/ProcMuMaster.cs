using System.Collections;
using ProcMu.ScriptableObjects;
using ProcMu.UnityScripts.Utilities;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    /*
     * OVERVIEW OF TABLE NUMBERS FOLLOWS
        ;Global config tables - #800-809
        giScale ftgen 800, 0, 128, -2, -1                   ;Global scale table
        giNotes ftgen 801, 0, 128, -2, -1                   ;Global table containing midi note numbers of all active notes in scale
        giActivebars ftgen 802, 0, 64, -2, -1               ;Global table containing active bar information, read: 4 * # of layers, including individual eucrth layers
        giComm ftgen 803, 0, 4, -2, 0                       ;Communication table for providing Unity with Csound state information, Params: 0 = update

        ;EucRth config tables - #810-819
        giEucRthConfig ftgen 810, 0, 8, -2, 0               ;Params: 0 = sample table#, 1 = pulses
        giEucGrid ftgen 811, 0, giEuclayers*giSteps, -2, -1 ;EucRth grid as table. Length is steps * layers.


        ;SnhMel config tables - #820-829
        giSnhMelConfig ftgen 820, 0, 4, -2, 0               ;Params: 0 = minOct, 1 = maxOct, 2 = occurence


        ;Chords config tables - #830-839
        giChordsConfig ftgen 830, 0, 8, -2, 0               ;Params: 0 = pulses
        giChordsNotes ftgen 831, 0, 16, -2, 0               ;Params: 0 = note0, 1 = note1...16 = note16
        giChordsInstr ftgen 832, 0, 32, -2, 0               ;Instrument config table
        giChordsGrid ftgen 833, 0, giSteps, -2, -1          ;Chords steps grid

        ;Waveforms
        gisine   ftgen 710, 0, 16384, 10, 1	                                                  ; Sine wave
        gisquare ftgen 711, 0, 16384, 10, 1, 0, 0.3, 0, 0.2, 0, 0.14, 0, .111                 ; Square
        gisaw    ftgen 712, 0, 16384, 10, 1, 0.5, 0.3, 0.25, 0.2, 0.167, 0.14, 0.125, .111    ; Sawtooth
        gipulse  ftgen 713, 0, 16384, 10, 1, 1, 1, 1, 0.7, 0.5, 0.3, 0.1                      ; Pulse
     */
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

            if (csoundUnity.GetTableSample(803, 0) > 0)
                UpdateCsound();

            SetCsData();
        }

        /// <summary> Csound update is triggered once every bar (16 steps). </summary>
        void UpdateCsound()
        {
            csoundUnity.SetTable(803, 0, 0);
            //csoundUnity.CopyTableIn(831, ChordsGenerateNotes());
        }

        /// <summary> Sends information to Csound. </summary>
        private void SetCsData()
        {
            //Global variables
            csoundUnity.SetChannel("gBpm", bpm);
            csoundUnity.SetChannel("gIntensity", intensity);


            //Set scale tables / TODO Only execute when scale has changed
            csoundUnity.CopyTableIn(800, ProcMuUtils.ConvertScale(mc.Scale.Scale)); //Total scale
            csoundUnity.CopyTableIn(801, ProcMuUtils.Int2Double(mc.Scale.ScaleNotes)); //Scale as active notes
            csoundUnity.CopyTableIn(802, GlobalGenerateBars()); //Active bars

            csoundUnity.CopyTableIn(810, EucRthGenerateConfig()); //EucRth dynamics config

            csoundUnity.CopyTableIn(820, SnhMelGenerateConfig()); //SnhMel dynamics config
            csoundUnity.CopyTableIn(821, GSynthGenerateConfig(mc.snhmel_synthconfig)); //GSynth config (SnhMel)

            csoundUnity.CopyTableIn(830, ChordsGenerateConfig()); //Chords dynamics config
            csoundUnity.CopyTableIn(831, ChordsGenerateNotes()); //Chords notes
            csoundUnity.CopyTableIn(832, GSynthGenerateConfig(mc.chords_synthconfig)); //GSynth config (Chords)
        }

        private double[] GlobalGenerateBars()
        {
            double[] output = new double[64];

            for (int i = 0; i < output.Length; i++)
            {
                if (intensity <= 0.5f) output[i] = mc.activeBars0[i] ? 1 : -1;
                else output[i] = mc.activeBars1[i] ? 1 : -1;
            }

            return output;
        }

        private double[] EucRthGenerateConfig()
        {
            //Filling EucRth config
            double[] eucrthConfig = new double[8];
            for (int i = 0; i < 4; i++)
            {
                eucrthConfig[2 * i] = mc.sampleSelection[i];
                eucrthConfig[2 * i + 1] = Mathf.RoundToInt(Random.Range(
                    (float) Mathf.RoundToInt(
                        Mathf.Lerp(mc.eucrth_minImpulses0[i], mc.eucrth_minImpulses1[i], intensity)),
                    (float) Mathf.RoundToInt(
                        Mathf.Lerp(mc.eucrth_maxImpulses0[i], mc.eucrth_maxImpulses1[i], intensity))
                ));
            }

            return eucrthConfig;
        }


        private double[] ChordsGenerateConfig()
        {
            double[] output = new double[8];

            //pulses
            output[0] = Mathf.RoundToInt(Random.Range(
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.chords_minImpulses0, mc.chords_minImpulses1, intensity)),
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.chords_maxImpulses0, mc.chords_maxImpulses1, intensity))
            ));
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = 0;
            output[6] = 0;
            output[7] = 0;

            return output;
        }

        private double[] ChordsGenerateNotes()
        {
            double[] output = new double[16];
            double[] notes = Chords.MakeChord(mc.Scale,
                Mathf.RoundToInt(Mathf.Lerp(mc.chords_minOct0, mc.chords_minOct1, intensity)),
                Mathf.RoundToInt(Mathf.Lerp(mc.chords_maxOct0, mc.chords_maxOct1, intensity)),
                mc.chordMode);
            notes.CopyTo(output, 0);

            for (int i = notes.Length; i < output.Length; i++)
                output[i] = -1;

            return output;
        }

        private double[] SnhMelGenerateConfig()
        {
            double[] output = new double[8];

            //pulses
            output[0] = Mathf.RoundToInt(Random.Range(
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_minImpulses0, mc.snhmel_minImpulses1, intensity)),
                (float) Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_maxImpulses0, mc.snhmel_maxImpulses1, intensity))
            ));

            //occurence
            output[1] = Random.Range(
                Mathf.Lerp(mc.snhmel_minOccurence0, mc.snhmel_minOccurence1, intensity),
                Mathf.Lerp(mc.snhmel_maxOccurence0, mc.snhmel_maxOccurence1, intensity)
            );

            output[2] = Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_minOct0, mc.snhmel_minOct1, intensity)); //min oct
            output[3] = Mathf.RoundToInt(Mathf.Lerp(mc.snhmel_maxOct0, mc.snhmel_maxOct1, intensity)); //max oct

            output[4] = (int) mc.snhmel_melodycurve; //melody (lfo) curve
            output[5] = (int) mc.snhmel_melodymode; //melody mode

            return output;
        }

        private double[] GSynthGenerateConfig(GSynthConfig config)
        {
            double[] output = new double[32];
            output[0] = -1;
            output[1] = -1;
            output[2] = -1;
            output[3] = -1;
            output[4] = -1;
            output[5] = config.velocity;
            output[6] = (int) config.wavetable;
            output[7] = config.noise; //noise amount
            output[8] = config.ffreq; //filter frequency
            output[9] = config.fres; //filter resonance

            output[10] = config.fenv_amt; //filter envelope amount

            output[11] = config.fenv_att; //filter attack
            output[12] = config.fenv_dec; //filter decay
            output[13] = config.fenv_sus; //filter sustain
            output[14] = config.fenv_rel; //filter release

            //amp variables
            output[15] = config.aenv_att; //amp attack
            output[16] = config.aenv_dec; //amp decay
            output[17] = config.aenv_sus; //amp sustain
            output[18] = config.aenv_rel; //amp release
            output[19] = config.width;
            return output;
        }
    }
}