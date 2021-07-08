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
giScale ftgen 800, 0, 128, 2, -1

;EucRth config table
giEucRthConfig ftgen 801, 0, 8, -2, 0

;EucRth samples tables
giSoundFiles[] init 4   //Allocating space to load samples from Unity

//SYSTEM INSTRUMENTS
instr UPD

  gktest table 0, 800 ;check if obsolete
endin

//Generates a global clock signal (gktrig) as long as it's running.
instr CLOCK
    gkbpm init 110 ;beats per minute, TODO fetch from Unity with chnget
    gkbpm chnget "gBpm"

    kpulses = 4 ;pulses per beat

    gktrig metro gkbpm*kpulses/60

    ;gkbpm = chnget("gkbpm")
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
    giSoundFiles[p4] chnget sprintf("sampletable%d", 900+p4)

    ifn   = giSoundFiles[p4]
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

    ;outs atab *.1, atab*.1
    chnset atab, "eucrth_l"
    chnset atab, "eucrth_r"

endin

//EUCLIDEAN RHYTHMS

gilayers init 4 ;number of layers may be changed, however, only up to 4 files are supported as of writing
gitotalsteps init 16

gkgrid[][] init gilayers,gitotalsteps

gkpulses[] init gilayers

//Fills the grid. Triggered at the start of a new bar.
instr EUC_FILL
    //
    kbucket[] init gilayers
    kstep init 0
    klayer init 0
    //
    //Set pulse and rotation values from sliders for each layer, TODO rotation not implemented!
    while klayer < lenarray(gkgrid,1) do
        gkpulses[klayer] tab (klayer * 2) + 1, 801

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
                gkgrid[klayer][kstep] = tab(klayer * 2, 801)  ;set grid value to sample index
            else
                gkgrid[klayer][kstep] = 0
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
            if gkgrid[klayer][kstep] != 0 then
                event "i", "SMPLR_UNITY", 0, 2, gkgrid[klayer][kstep]
            endif
            klayer += 1
        od

        kstep = (kstep + 1) % gitotalsteps  ;perform modulo operation to clamp step index
    endif
endin

//Maybe SNHMEL could use a morphable wavetable for sound generation??
instr SNHMEL
  krangeMin init 0
  krangeMax init 0
  kfreqMin = 60/gkbpm
  kfreqMax = gkbpm/60

  kAmp init 1



  kCnt init 0
  kTablen tableng 800 //Note: 800 is index of global scale table. Preferable to use a variable instead...

  //Todo: Make sure that this is only performed if the table contents have changed!
  while tablei(kCnt,800) != -1 do
    kCnt += 1
  od

//SNH FOR VOLUME
  kAmp rspline -1, 0.5, 0.1, 1  ;kFreqMin, kFreqMax should be modified through config in Unity and connected with intensity
  kAmp limit kAmp, 0, 1

//SNH FOR MELODY
  ksp rspline 0, kCnt, kfreqMin, kfreqMax


  ksnh samphold ksp, gktrig
  ksnh limit ksp, 0, kCnt
  ksnh = int(ksnh)


  ktab tablei ksnh, 800  ;test code, remove/edit

  if gktrig == 1 then
    kPitch = pow(2,(ktab-69)/12)*440  //Manually calculating frequency from midi note number
    ;kPitch = mtof:k(ktab)  //...because mtof is not working in this version of CsoundUnity
  endif

  kPitchPrt portk kPitch, .01

  //Sound generation

  aOsc poscil kAmp, kPitchPrt

  chnset aOsc, "snhmel_l"
  chnset aOsc, "snhmel_r"
endin

</CsInstruments>
<CsScore>
f 0 z
i "UPD" 2 -1
;i "SMPLR_LOAD" 2 1
i "CLOCK" 2 -1
i "EUC_STEP" 2 -1
i "SNHMEL" 2 -1
</CsScore>
</CsoundSynthesizer>
