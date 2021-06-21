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




//Generates a global clock signal (gktrig) as long as it's running.
instr CLOCK
    kbpm init 120
    ;kbpm = chnget("gBpm")  ;beats per minute
    kpulses = 4 ;pulses per beat
    
    gktrig metro kbpm*kpulses/60
endin

//INSTRUMENTS

//TEST SIMPLE OSCIL SYNTH
//i, start, dur, pitch
instr SYNTH
    kAmp adsr .01,0.1,0,0
    kPitch = p4
    aOsc poscil kAmp, kPitch
    out aOsc,aOsc
endin

//SAMPLER
instr SMPLR_LOAD
    ; file operations/setup
    giSoundFiles[] init 4   //Allocating space to load samples from Unity

    ipcnt = 0 ;counter for iterating through the following arrays

    //Set file locations / THIS SHOULD BECOME DEPRECATED, AS SAMPLES SHALL BE LOADED THROUGH CSOUND-UNITY
    gSSoundFileLocs[] init 4
    gSSoundFileLocs[0] = "samples/HIHAT1.wav"
    gSSoundFileLocs[1] = "samples/TOM1.wav"
    gSSoundFileLocs[2] = "samples/CLAV1.wav"
    gSSoundFileLocs[3] = "samples/CONGA1.wav"


    //Generate ftables from files
    ;giSoundFiles[] init 4 ;we only allocate space here. Initialization with values
/*
    ipcnt = 0
    while ipcnt < lenarray(gSSoundFileLocs,1) do
    giSoundFiles[ipcnt] ftgen 0, 0, 0, 1, gSSoundFileLocs[ipcnt], 0, 0, 0
    ipcnt += 1
    od
*/
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

//Sampler instrument
//i, start, dur, pitch, file_index
instr SMPLR ; play audio from function table using flooper2 opcode
    giSoundFiles[p5] chnget sprintf("sampletable%d", 900+p5)

    kAmp adsr 0.01,0.2,0,0     ; volume envelope
    kPitch       =         p4  ; pitch/speed
    kLoopStart   =         0   ; point where looping begins (in seconds)
    kLoopEnd     =         giSoundFileLgts[p5]; loop end (end of file)
    kCrossFade   =         0   ; cross-fade time
    ; read audio from the function table using the flooper2 opcode
    aSigL,aSigR         flooper2  kAmp,kPitch,kLoopStart,kLoopEnd,kCrossFade,giSoundFiles[p5]
                 out       aSigL, aSigR ; send audio to output
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
    outs atab *.1, atab*.1
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
    //Set pulse and rotation values from sliders for each layer
    while klayer < lenarray(gkgrid,1) do
        gkpulses[klayer] = klayer + chnget("gIntensity") ;test value, change to: chnget sprintfk("layer%d_pulses",ilayer)
        
        //krotation[klayer] chnget sprintfk("layer%d_rotation",klayer)
        klayer+=1
    od
    
    klayer = 0
    //Fill sequence
    while klayer < lenarray(gkgrid,1) do
        kstep = 0
        while kstep < lenarray(gkgrid,2) do ;dividing by 2 as array is twice the step size (for rotation support)
    
            kbucket[klayer] = kbucket[klayer] + gkpulses[klayer]
    
            if kbucket[klayer] >= lenarray(gkgrid,2) then
                kbucket[klayer] = kbucket[klayer] - lenarray(gkgrid,2)
                gkgrid[klayer][kstep] = 1
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
            if gkgrid[klayer][kstep] == 1 then
                event "i", "SMPLR_UNITY", 0, 2, klayer
            endif
            klayer += 1
        od
    
        kstep = (kstep + 1) % gitotalsteps  ;perform modulo operation to clamp step index
    endif
endin

</CsInstruments>
<CsScore>
f 0 z
i "SMPLR_LOAD" 0 1
i "CLOCK" 0 -1
i "EUC_STEP" 0 -1
</CsScore>
</CsoundSynthesizer>