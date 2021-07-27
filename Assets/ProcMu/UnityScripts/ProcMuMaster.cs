using System.Collections;
using ProcMu.ScriptableObjects;
using ProcMu.UnityScripts.Utilities;
using UnityEngine;

namespace ProcMu.UnityScripts
{
    /*
     * OVERVIEW OF TABLE NUMBERS
        ;Global config tables - #800-809
        giScale ftgen 800, 0, 128, -2, -1                   ;Global scale table
        giNotes ftgen 801, 0, 128, -2, -1                   ;Global table containing midi note numbers of all active notes in scale, last index(127) contains number of active notes, i.e. highest index containing a valid note
        giActivebars ftgen 802, 0, 64, -2, -1               ;Global table containing active bar information, read: 4 * # of layers, including individual eucrth layers
        giComm ftgen 803, 0, 4, -2, 0                       ;Communication table for providing Unity with Csound state information, Params: 0 = update, 1 = currentbar, 2 = currentstep

        ;EucRth config tables - #810-819
        giEucRthConfig ftgen 810, 0, 8, -2, 0               ;Params: 0 = sample table#, 1 = pulses, for layers 0-3
        giEucGrid ftgen 811, 0, giEuclayers*giSteps, -2, -1 ;EucRth grid as table. Length is steps * layers.


        ;SnhMel config tables - #820-829
        giSnhMelConfig ftgen 820, 0, 8, -2, 0               ;Params: 0 = pulses, 1 = occurence, 2 = minOct, 3 = maxOct, 4 = melody (lfo) curve, 5 = melody mode
        giSnhMelGsyn ftgen 821, 0, 32, -2, 0                ;GSYNTH Instrument config table
        giSnhMelGrid  ftgen 822, 0, giSteps, -2, -1         ;SNHMEL steps grid

        ;Chords config tables - #830-839
        giChordsConfig ftgen 830, 0, 8, -2, 0               ;Params: 0 = pulses
        giChordsNotes ftgen 831, 0, 16, -2, 0               ;Params: 0 = note0, 1 = note1...16 = note16
        giChordsGsyn ftgen 832, 0, 32, -2, 0                ;GSYNTH Instrument config table
        giChordsGrid ftgen 833, 0, giSteps, -2, -1          ;CHORDS steps grid

        ;Waveforms
        gisine   ftgen 700, 0, 16384, 10, 1	                                                  ; Sine wave
        gisquare ftgen 701, 0, 16384, 10, 1, 0, 0.3, 0, 0.2, 0, 0.14, 0, .111                 ; Square
        gisaw    ftgen 702, 0, 16384, 10, 1, 0.5, 0.3, 0.25, 0.2, 0.167, 0.14, 0.125, .111    ; Sawtooth
        gipulse  ftgen 703, 0, 16384, 10, 1, 1, 1, 1, 0.7, 0.5, 0.3, 0.1                      ; Pulse
     */
    public class ProcMuMaster : MonoBehaviour
    {
        [SerializeField] private CsoundUnity csoundUnity;
        private bool _isInitialized;

        private double[] comm = new double[4]; //Allocate space for communication information with Csound

        private AudioClip[] audioClips;
        private MuSampleDb sampleDb;

        //Test variables
        [SerializeField, Range(0, 1000)] private double bpm = 120;
        [SerializeField, Range(0f, 1f)] private float intensity = 1;

        public MuConfig mc;

        private void Awake()
        {
            sampleDb = Resources.Load<MuSampleDb>("procmu_sampledb");
            audioClips = sampleDb.audioClips; //Fetch audio clips from sample db
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

            //Update csound state in Unity
            csoundUnity.CopyTableOut(803, out comm);
            if (comm[0] > 0) UpdateCsound();

            SetCsData();

#if UNITY_EDITOR
            mc.comm = comm;
#endif
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
            csoundUnity.CopyTableIn(802, FtableConverter.GlobalGenerateBars(mc, intensity)); //Active bars

            csoundUnity.CopyTableIn(810, FtableConverter.EucRthGenerateConfig(mc, intensity)); //EucRth dynamics config

            csoundUnity.CopyTableIn(820, FtableConverter.SnhMelGenerateConfig(mc, intensity)); //SnhMel dynamics config
            csoundUnity.CopyTableIn(821, mc.snhmel_synthconfig.config); //GSynth config (SnhMel)

            csoundUnity.CopyTableIn(830, FtableConverter.ChordsGenerateConfig(mc, intensity)); //Chords dynamics config
            csoundUnity.CopyTableIn(831, FtableConverter.ChordsGenerateNotes(mc, intensity)); //Chords notes
            csoundUnity.CopyTableIn(832, mc.chords_synthconfig.config); //GSynth config (Chords)

            csoundUnity.CopyTableIn(840, FtableConverter.SnhBasGenerateConfig(mc, intensity)); //SnhBas dynamics config
            csoundUnity.CopyTableIn(841, mc.snhbas_synthconfig.config); //GSynth config (SnhBas)
        }
    }
}