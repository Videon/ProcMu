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
;global scale config table
giScale ftgen 800, 0, 128, -2, -1

;EucRth config table
giEucRthConfig ftgen 801, 0, 8, -2, 0
giSnhMelConfig ftgen 802, 0, -3, -2, 0  ;params: 0 = minOct, 1 = maxOct, 2 = occurence
giChordsConfig ftgen 803, 0, 2, -2, 0 ;params 0 = minOct, 1 = maxOct

;EucRth samples tables
giSoundFiles[] init 4   //Allocating space to load samples from Unity

//SYSTEM INSTRUMENTS
instr UPD

  gktest table 0, 800 ;check if obsolete
endin

//Generates a global clock signal (gktrig) as long as it's running.
instr CLOCK
    gkbpm init 110 ;beats per minute
    ;gkbpm portk chnget("gBpm"), 0.5
    gkbpm chnget "gBpm" ;TODO implement smoothing for handling external value changes

    kpulses = 4 ;pulses per beat

    gktrig metro gkbpm*kpulses/60
endin

//INSTRUMENTS

//TEST SIMPLE OSCIL SYNTH
//i, start, dur, pitch
instr SYNTH
    kAmp adsr .01,0.1,0,0
    kPitch = p4
    aOsc poscil kAmp, kPitch
    out aOsc*0.5,aOsc*0.5
endin

//SAMPLER - TODO DEPRECATED?
instr SMPLR_LOAD
    ; file operations/setup


    ipcnt = 0 ;counter for iterating through the following arrays

    //Set file locations / THIS SHOULD BECOME DEPRECATED, AS SAMPLES SHALL BE LOADED THROUGH CSOUND-UNITY
    gSSoundFileLocs[] init 4


    //Get file sample rates
    giSoundFileSrs[] init 4

    ipcnt = 0
    while ipcnt < lenarray(gSSoundFileLocs,1) do
    giSoundFileSrs[ipcnt] filesr gSSoundFileLocs[ipcnt]
    ipcnt += 1
    od

    //Get file channels (mono/stereo)
    giSoundFileChnls[] init 4

    ipcnt = 0
    while ipcnt < lenarray(gSSoundFileLocs,1) do
    giSoundFileChnls[ipcnt] filenchnls gSSoundFileLocs[ipcnt]
    ipcnt += 1
    od

    //Get file lengths
    giSoundFileLgts[] init 4

    ipcnt = 0
    while ipcnt < lenarray(gSSoundFileLocs,1) do
    giSoundFileLgts[ipcnt] filelen gSSoundFileLocs[ipcnt]
    ipcnt += 1
    od
endin

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

//EUCLIDEAN RHYTHMS

gilayers init 4 ;number of layers may be changed, however, only up to 4 files are supported as of writing
gitotalsteps init 16

gkgrid[][] init gilayers,gitotalsteps

gkpulses[] init gilayers

//Fills the grid. Triggered at the start of a new bar.
instr EUC_FILL
    //
    kbucket[] init gilayers ;using as many "buckets" as percussion layers
    kstep init 0
    klayer = 0
    //
    //Set pulse and rotation values from sliders for each layer, TODO rotation not implemented!
    while klayer < lenarray(gkgrid,1) do
        gkpulses[klayer] tab (klayer * 2) + 1, 801 ;fetch number of pulses from eucrth config table

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
                gkgrid[klayer][kstep] = tab(klayer * 2, 801)  ;if impulse, set grid value to sample index
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

        kstep = (kstep + 1) % gitotalsteps  ;perform modulo operation to clamp step index
    endif
endin

//Maybe SNHMEL could use a morphable wavetable for sound generation??
instr SNHMEL
  krangeMin = tab(0,802)
  krangeMax = tab(1,802)
  kfreqMin = tab(2,802)
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

gkChord[] init 5 ;holds max 5 notes: 1-2 lower notes w bigger intervals, 2-3 higher notes w smaller intervals

//Builds a new chord
instr CHORDS_SET
  ;krangeMin = tab(0,803)
  ;krangeMax = tab(1,803)

  krangeMin = 64  ;TODO REMOVE TEST VALUES AND USE ABOVE FETCH FROM TAB
  krangeMax = 88

  kNotes[] init 128 ; array for saving possible notes to choose from (as midi note number)

  gkChord[] init 5 ;holds max 5 notes: 1-2 lower notes w bigger intervals, 2-3 higher notes w smaller intervals

  kCnt = 0  ;counter variable

//Find all notes which are part of the scale and are between krangeMin/krangeMax
  while krangeMin + kCnt < krangeMax do
    kVal = tab(krangeMin + kCnt, 800)
    if kVal > -1 then
      kNotes[kCnt] = krangeMin + kCnt

      else
    endif

    kCnt+=1
  od

  kNotesMax = kCnt


  kRoot = kNotes[int(random(0, kNotesMax))]   ; 1. get "root" note, can be any note which was found above



//Filling the chord note array
  kCnt = 0

  gkChord[kCnt] = kRoot
  kCnt+=1

  ; 2. if root note + 7 is within scale, add note
  kVal = tab(kRoot + 7, 800)
  if kVal > -1 then
    gkChord[kCnt] = kRoot+7
  endif

  kCnt+=1

  ; 3. add note which is root note + 12
  kVal = tab(kRoot + 12, 800)
  if kVal > -1 then
    gkChord[kCnt] = kRoot + 12
  endif

  kCnt+=1

  ; 4. add root note + 15, if possible
  kVal = tab(kRoot + 15, 800)
  if kVal > -1 then
    gkChord[kCnt] = kRoot + 15
  endif

  kCnt+=1

  ; 5. add root note + 17, if possible
  kVal = tab(kRoot + 17, 800)
  if kVal > -1 then
    gkChord[kCnt] = kRoot + 17
    else
      kVal = tab(kRoot + 19, 800)
      if kVal > -1 then
        gkChord[kCnt] = kRoot + 19
      endif
  endif

  kCnt=0

  while kCnt < lenarray(gkChord) do
    prints "chord note %d: %d", kCnt, gkChord[kCnt]
    kCnt+=1
  od

endin

</CsInstruments>
<CsScore>
f 0 z
i "UPD" 2 -1
;i "SMPLR_LOAD" 2 1
i "CLOCK" 2 -1
i "EUC_STEP" 2 -1
i "SNHMEL" 2 -1
i "CHORDS_SET" 20 1
i "CHORDS_SET" 21 1
i "CHORDS_SET" 22 1
</CsScore>
</CsoundSynthesizer>
