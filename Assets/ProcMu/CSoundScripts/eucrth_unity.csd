<Cabbage>
</Cabbage>

<CsoundSynthesizer>
<CsOptions>
-n -d
</CsOptions>
<CsInstruments>
; Initialize the global variables.
ksmps = 32
nchnls = 2
0dbfs = 1

seed 0  //Sets a new seed for randomization on every init

//TABLES
;Global config tables - #800-809
giScale ftgen 800, 0, 128, -2, -1 ;Global scale table
giNotes ftgen 801, 0, 128, -2, -1 ;Global table containing midi note numbers of all active notes in scale

;EucRth config tables - #810-819
giEucRthConfig ftgen 810, 0, 8, -2, 0
giSoundFiles[] init 4   //EucRth samples tables, allocating space to load samples from Unity. Uses tables 900+n. Currently n = 4


;SnhMel config tables - #820-829
giSnhMelConfig ftgen 820, 0, -3, -2, 0  ;params: 0 = minOct, 1 = maxOct, 2 = occurence


;Chords config tables - #830-839
giChordsConfig ftgen 830, 0, -7, -2, 0  ;params: 0 = minOct, 1 = maxOct
giChordsNotes ftgen 831, 0, 16, -2, 0 ;params: 0 = note0, 1 = note1...16 = note16
giChordsInstr ftgen 832, 0, 32, -2, 0 ;instrument config table

;Waveforms
giImp  ftgen  700, 0, 4096, 10, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
giSaw  ftgen  701, 0, 4096, 10, 1,-1/2,1/3,-1/4,1/5,-1/6,1/7,-1/8,1/9,-1/10
giSqu  ftgen  702, 0, 4096, 10, 1, 0, 1/3, 0, 1/5, 0, 1/7, 0, 1/9, 0
giTri  ftgen  703, 0, 4096, 10, 1, 0, -1/9, 0, 1/25, 0, -1/49, 0, 1/81, 0

//GLOBAL VARIABLES
gitotalsteps init 16

//CHANNELS
chn_k "update", 1

//SYSTEM INSTRUMENTS

//Generates a global clock signal (gktrig) as long as it's running.
instr CLOCK
    gkbpm init 110 ;beats per minute
    ;gkbpm portk chnget("gBpm"), 0.5
    gkbpm chnget "gBpm" ;TODO implement smoothing for handling external value changes

    kstep init 0

    kpulses = 4 ;pulses per beat

    gktrig metro gkbpm*kpulses/60

    if gktrig == 1 then
      kstep = (kstep + 1) % gitotalsteps  ;perform modulo operation to clamp step index

      if kstep == 0 then
        chnset 1, "update"

        event "i", "CHORDS", 0, 1
      endif
    endif

endin

//INSTRUMENTS

//Sampler instrument for use with audioclips from Unity
//i, start, dur, pitch, file_index
instr SMPLR_UNITY ; play audio from function table using flooper2 opcode

    ifn   =  chnget(sprintf("sampletable%d", 900+p4))
    ;prints "giTable p4 = %d, ifn = %d\n", p4, ifn
    ilen  =  nsamp(ifn)
    ;prints "actual numbers of samples = %d\n", ilen
    itrns =  1	; no transposition
    ilps  =  0	; loop starts at index 0
    ilpe  =  ilen	; ends at value returned by nsamp above
    imode =  1	; loops forward
    istrt =  0	; commence playback at index 0 samples
    ; lphasor provides index into f1
    alphs lphasor itrns, ilps, ilpe, imode, istrt
    atab  tablei  alphs, ifn

    outs atab *.1, atab*.1


    ;chnmix atab, "eucrth_l"  TODO use chnmix to sum sound of all sampler instances
    ;chnmix atab, "eucrth_r"

endin

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//EUCLIDEAN RHYTHMS
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

gilayers init 4 ;number of layers may be changed, however, only up to 4 files are supported as of writing
giEucsteps init 16

gkgrid[][] init gilayers,giEucsteps

gkpulses[] init gilayers

//Fills the grid. Triggered at the start of a new bar.
instr EUC_FILL
    //
    kbucket[] init gilayers ;using as many "buckets" as percussion layers
    kstep init 0
    klayer = 0
    //
    //Set pulse and rotation values from sliders for each layer, TODO rotation needs to be implemented!
    while klayer < lenarray(gkgrid,1) do
        gkpulses[klayer] tab (klayer * 2) + 1, 810 ;fetch number of pulses from eucrth config table

        klayer+=1
    od

    klayer = 0
    //Fill sequence
    while klayer < lenarray(gkgrid,1) do
        kstep = 0
        kbucket[klayer] = 0
        while kstep < lenarray(gkgrid,2) do ;dividing by 2 as array is twice the step size (for rotation support)

            kbucket[klayer] = kbucket[klayer] + gkpulses[klayer]

            if kbucket[klayer] >= lenarray(gkgrid,2) then
                kbucket[klayer] = kbucket[klayer] - lenarray(gkgrid,2)
                gkgrid[klayer][kstep] = tab(klayer * 2, 810)  ;if impulse, set grid value to sample index
            else
                gkgrid[klayer][kstep] = -1  ;-1 = no impulse on grid position
            endif

            //gkgrid[ilayer][istep+ lenarray(gkgrid,2)] = gkgrid[ilayer][istep] ;copying current value to offset position (for rotation support)

            kstep += 1
        od
        klayer += 1
    od
endin

//Steps through the grid and triggers sounds accordingly
instr EUC_STEP
    //
    krot init 0 ;rotation, i.e., grid offset
    kstep init 0 ;current sequencer step
    //

    if gktrig == 1 then
        //Fill grid on start of a new bar
        if kstep == 0 then
            event "i", "EUC_FILL", 0, 1
        endif

        //Check grid for all layers at current step and trigger samples accordingly
        klayer = 0

        while klayer < lenarray(gkgrid,1) do
            if gkgrid[klayer][kstep] > -1 then
                event "i", "SMPLR_UNITY", 0, 2, gkgrid[klayer][kstep]
            endif
            klayer += 1
        od

        kstep = (kstep + 1) % giEucsteps  ;perform modulo operation to clamp step index
    endif
endin

//EUCLIDEAN RHYTHMS END
//-----------------------------------------------------------------------------------------------------------------------------

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//SAMPLE AND HOLD MELODY
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//TODO Maybe SNHMEL could use a morphable wavetable for sound generation??
instr SNHMEL
  krangeMin = tablei(0,820)
  krangeMax = tablei(1,820)
  kfreqMin = tablei(2,820)
  kfreqMax = kfreqMin

  kAmp = 1

  kCnt init 0
  kTablen tableng 800

  //TODO: Make sure this is only performed if the table contents have changed!
  while tablei(kCnt,800) > -1 do
    kCnt += 1
  od

//SNH FOR VOLUME
  ;kAmp rspline -1, 0.5, 0.1, 1  ;kFreqMin, kFreqMax should be modified through config in Unity and connected with intensity
  ;kAmp limit kAmp, 0, 1

//SNH FOR MELODY
  ksp rspline krangeMin*12, krangeMax*12, kfreqMin, kfreqMax

  ktab tab int(ksp), 800

  if ktab > -1 then
    kPitch = pow(2,(ktab-69)/12)*440
  endif


  ;ksnh samphold ksp, gktrig
  ;ksnh limit ksp, 0, kCnt
  ;ksnh = int(ksnh)


  ;ktab tab ksnh, 800  ;test code, remove/edit

  ;if gktrig == 1 then
  ;  kPitch = pow(2,(ktab-69)/12)*440  //Manually calculating frequency from midi note number
    ;kPitch = mtof:k(ktab)  //...because mtof is not working in this version of CsoundUnity
  ;endif

  kPitchPrt portk kPitch, .01

  //Sound generation

  aOsc poscil kAmp*.1, kPitchPrt

  chnset aOsc, "snhmel_l"
  chnset aOsc, "snhmel_r"
endin

//SAMPLE AND HOLD MELODY END
//-----------------------------------------------------------------------------------------------------------------------------


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//CHORDS
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//Fetches notes from note ftable (#831)
instr CHORDS
  kCnt init 0

  while kCnt < 16 do
    kval = table(kCnt,831)

    if kval > -1 then
      event "i", "GSYNTH", 0, 1, kval, tab_i(5,832), tab_i(6,832), tab_i(7,832), tab_i(8,832), tab_i(9,832), tab_i(10,832), tab_i(11,832), tab_i(12,832), tab_i(13,832), tab_i(14,832), tab_i(15,832), tab_i(16,832), tab_i(17,832), tab_i(18,832)
    endif

    kCnt += 1
  od
endin

//CHORDS END
//-----------------------------------------------------------------------------------------------------------------------------

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//GAME SYNTH
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

gisine   ftgen 710, 0, 16384, 10, 1	;sine wave
gisquare ftgen 711, 0, 16384, 10, 1, 0 , .33, 0, .2 , 0, .14, 0 , .11, 0, .09 ;odd harmonics
gisaw    ftgen 712, 0, 16384, 10, 0, .2, 0, .4, 0, .6, 0, .8, 0, 1, 0, .8, 0, .6, 0, .4, 0,.2 ;even harmonics

//i GSYNTH [p3 = length] [p4 = note] [p5 = velocity] [p6 = osc waveform 0:sin 1:sqr 2:saw] [p7 = noise amp]
//  [p8 = filter frequency] [p9 = filter resonance] [p10 = filter env amount][p11,12,13,14 = filter A,D,S,R] [p15,16,17,18 = amp A,D,S,R]
instr GSYNTH

;pX = lfo waveform

//Input/midi variables
ifreq = pow(2,(p4-69)/12)*440 ;note as midi# value, is converted to frequency (Hz)
ivel = p5 ;note velocity value

ifn = 710+p6

inoise = p7 ;noise amount

//Filter variables
iffreq = p8 ;lowpass filter frequency
ifres = p9  ;lowpass filter resonance
ifenv_amt = p10 ;lowpass filter env amount

ifenv_att = p11 ;filter attack
ifenv_dec = p12 ;filter decay
ifenv_sus = p13 ;filter sustain
ifenv_rel = p14 ;filter release

//Amp variables
iaenv_att = p15 ;amp attack
iaenv_dec = p16 ;amp decay
iaenv_sus = p17 ;amp sustain
iaenv_rel = p18 ;amp release


//LFOs
klfo1 lfo 1, 1


//OSCs
aosc1 poscil ivel, ifreq, ifn

anoise rand limit(inoise,0,1)

abus = aosc1 + anoise

//Filters
kfenv madsr ifenv_att, ifenv_dec, ifenv_sus, ifenv_rel  ;filter envelope


alp zdf_2pole abus, iffreq+kfenv*ifenv_amt, ifres ;filter signal


//Amp
aaenv madsr iaenv_att, iaenv_dec, iaenv_sus, iaenv_rel  ;amplitude envelope

abus = alp*aaenv

outs abus, abus

endin
//GAME SYNTH END
//-----------------------------------------------------------------------------------------------------------------------------

</CsInstruments>
<CsScore>
f 0 z
i "CLOCK" 2 -1
i "EUC_STEP" 2 -1
i "SNHMEL" 2 -1
</CsScore>
</CsoundSynthesizer>
