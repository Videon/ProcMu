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

//GLOBAL VARIABLES
giSteps init 16     ;Number of steps per bar (global)
giBars init 4       ;Number of bars per loop (global)

giEuclayers init 4  ;Number of euclidean rhythm percussion layers. Only 4 are supported as of writing. May be changed in the future.

//TABLES  - RANGE 900-999 is reserved for perc samples!
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

;SnhBas config tables - #840-849
giSnhBasConfig ftgen 840, 0, 8, -2, 0               ;Params: 0 = pulses, 1 = occurence, 2 = minOct, 3 = maxOct, 4 = melody (lfo) curve, 5 = melody mode
giSnhBasGsyn ftgen 841, 0, 32, -2, 0                ;GSYNTH Instrument config table
giSnhBasGrid  ftgen 842, 0, giSteps, -2, -1         ;SNHBAS steps grid

;Waveforms
gisine   ftgen 700, 0, 16384, 10, 1	                                                  ; Sine wave
gisquare ftgen 701, 0, 16384, 10, 1, 0, 0.3, 0, 0.2, 0, 0.14, 0, .111                 ; Square
gisaw    ftgen 702, 0, 16384, 10, 1, 0.5, 0.3, 0.25, 0.2, 0.167, 0.14, 0.125, .111    ; Sawtooth
gipulse  ftgen 703, 0, 16384, 10, 1, 1, 1, 1, 0.7, 0.5, 0.3, 0.1                      ; Pulse

//SYSTEM INSTRUMENTS

//Generates a global clock signal (gktrig) as long as it's running.
instr CLOCK
    gkbpm init 110 ;beats per minute
    ;gkbpm portk chnget("gBpm"), 0.5
    gkbpm chnget "gBpm" ;TODO implement smoothing for handling external value changes

    gkcurrentbar init 0 ;setting to -1 as it will be set to 1 on first gkstep
    gkstep init 0

    kpulses = 4 ;pulses per beat

    gktrig metro gkbpm*kpulses/60

    if gktrig == 1 then
      gkstep = (gkstep + 1) % giSteps  ;perform modulo operation to clamp step index
      tabw(gkstep,2,803)  ;set comm table index 2 to current step index

      if gkstep == 0 then
        gkcurrentbar = (gkcurrentbar+1) % giBars
        tabw(1,0,803) ;sets table index=0 to 1 to signify update to Unity
        tabw(gkcurrentbar,1,803)  ;set comm table index 1 to current bar index
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

    arevL, arevR freeverb atab, atab, 0.5, 0.6

    outs atab*0.1+arevL*0.1, atab*0.1+arevR*0.1

endin

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//EUCLIDEAN RHYTHMS
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Fills the grid. Triggered at the start of a new bar.
instr EUC_FILL
    ipulses = p4
    irotation = p5  ;TODO rotation currently not implemented
    itabn = p6  ;p6 is grid table#
    ioffset = p7 ;offsets write index by grid length * ioffset. needed for tables with multiple grid layers (e.g. EUCRTH)

    kbucket init 0
    kstep init 0

    //Fill sequence
    while kstep < giSteps do

        kbucket = kbucket + ipulses

        if kbucket >= giSteps then
            kbucket = kbucket - giSteps
            tabw(1,kstep+ioffset*giSteps,itabn)   ;1 = impulse for triggering sound
        else
            tabw(-1,kstep+ioffset*giSteps,itabn)  ;-1 = no impulse on grid position
        endif

        //gkgrid[ilayer][kstep+ giSteps] = gkgrid[ilayer][istep] ;copying current value to offset position (for rotation support)

        kstep += 1
    od
endin

//Steps through the grid and triggers sounds accordingly
instr EUC_STEP
    if gktrig == 1 then
        //Fill grid on start of a new bar
        if gkstep == 0 then
          //EUC_FILL for all instruments

          //EUCRTH
          klayer = 0
          while klayer < giEuclayers do
            event "i", "EUC_FILL", 0, 1, tab:k(klayer*2+1, 810), 0, 811, klayer
            klayer += 1
          od

          event "i", "EUC_FILL", 0, 1, tab:k(0,830), 0, 833, 0  //CHORDS

          event "i", "EUC_FILL", 0, 1, tab:k(0,820), 0, 822, 0  //SNHMEL

          event "i", "EUC_FILL", 0, 1, tab:k(0,840), 0, 842, 0  //SNHBAS

        endif

        //PLAY ALL INSTRUMENTS ACCORDING TO GRID AND ACTIVE BAR INFORMATION
        //EUCRTH

          klayer = 0
          while klayer < giEuclayers do ;iterate through all percussion layers
            if tab:k(0 * giBars + gkcurrentbar, 802) > 0 then ;only play if current bar is active
              if tab:k(klayer * 16 + gkstep, 811) > -1 then
                  event "i", "SMPLR_UNITY", 0, 2, tab:k(klayer*2, 810)
              endif
            endif
            klayer += 1
          od


        //CHORDS
        if tab:k(1 * giBars + gkcurrentbar, 802) > 0 then ;only play if current bar is active, x * giBars to skip bars indices for other instruments
          if tab:k(gkstep, 833) > -1 then
            event "i", "CHORDS", 0, 1
          endif
        endif

    endif
endin

//EUCLIDEAN RHYTHMS END
//-----------------------------------------------------------------------------------------------------------------------------

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//SAMPLE AND HOLD MELODY
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//TODO Maybe SNHMEL could use a morphable wavetable for sound generation??
instr SNHMEL
  ;kmaxnote = tab:k(127, 801)
  kminnote init 0
  kmaxnote init 0

  koccurence = tab:k(1, 820)
  kminoct = tab:k(2,820)*12 //multiply octave values by 12 to get octaves as midi-style notes
  kmaxoct = tab:k(3,820)*(12+11)  //add 11 to include last note of octave

  //Find min note index whenever oct settings have changed
  if changed(kminoct) == 1 then
    kcnt = 0
    while kcnt < 127 do
      if tab:k(kcnt, 801) >= kminoct then
        kminnote = kcnt
        kcnt=127
      endif
      kcnt +=1
    od
    ;printks "MINNOTE: %d", 0, kminnote
  endif

  if changed(kmaxoct) == 1 then
    kcnt = 0
    while kcnt < 127 do
      kmaxnote = kcnt
      if tab:k(kcnt, 801) >= kmaxoct then
        kcnt=127
      endif
      kcnt +=1
    od
    ;printks "MAXNOTE: %d", 0, kmaxnote
  endif

  krd randomi 0.01, 8, 0.1

  klfo lfo 1, krd, 1
  kscaled = kminnote + (kmaxnote-kminnote) * (klfo - -1) / (1 - -1) //scale lfo from -1...1 to min,max note indices

  ;printks "KRES: %d", 0.5, kscaled

  gksnhmel_note = limit(kscaled,kminnote,kmaxnote)
  gksnhmel_note = tab:k(kscaled,801)


  kmode = tab:k(5,820)
  ;printks "KNOTE: %d", 0.5, knote
  ;TODO check if set to discrete mode and play gsynth only in that case.
  if gktrig == 1 then
    //if continuous
    if kmode == 0 kgoto continuous
      kgoto discrete  ;else

    continuous:
    if changed(kmode) == 1 then
      event "i", "GSYNTH_CONTIN", 0, -1, 0, 821
    endif

    kgoto continue

    discrete:
    if changed(kmode) == 1 then
      event "i", "GSYNTH_CONTIN", 0, 1, 0, 821
    endif

    if tab:k(2 * giBars + gkcurrentbar, 802) > 0 then
      if tab:k(gkstep, 822) > -1 then
        event "i", "GSYNTH", 0, 1, gksnhmel_note, 821
      endif
    endif

    continue:
  endif

endin

//TODO Currently SNHBAS is identical with SNHMEL. Using two different instruments for allowing individual features later on
instr SNHBAS
  ;kmaxnote = tab:k(127, 801)
  kminnote init 0
  kmaxnote init 0

  koccurence = tab:k(1, 840)
  kminoct = tab:k(2,840)*12 //multiply octave values by 12 to get octaves as midi-style notes
  kmaxoct = tab:k(3,840)*(12+11)  //add 11 to include last note of octave

  //Find min note index whenever oct settings have changed
  if changed(kminoct) == 1 then
    kcnt = 0
    while kcnt < 127 do
      if tab:k(kcnt, 801) >= kminoct then
        kminnote = kcnt
        kcnt=127
      endif
      kcnt +=1
    od
    ;printks "MINNOTE: %d", 0, kminnote
  endif

  if changed(kmaxoct) == 1 then
    kcnt = 0
    while kcnt < 127 do
      kmaxnote = kcnt
      if tab:k(kcnt, 801) >= kmaxoct then
        kcnt=127
      endif
      kcnt +=1
    od
    ;printks "MAXNOTE: %d", 0, kmaxnote
  endif


  klfo lfo 1, 1.2, 5
  kscaled = kminnote + (kmaxnote-kminnote) * (klfo - -1) / (1 - -1) //scale lfo from -1...1 to min,max note indices

  ;printks "KRES: %d", 0.5, kscaled

  knote = limit(kscaled,kminnote,kmaxnote)
  knote = tab:k(kscaled,801)

  ;printks "KNOTE: %d", 0.5, knote

  if gktrig == 1 then
    if tab:k(3 * giBars + gkcurrentbar, 802) > 0 then
      if tab:k(gkstep, 842) > -1 then
        event "i", "GSYNTH", 0, 1, knote, 841
      endif
    endif
  endif

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
    kval = tab:k(kCnt,831)  ;kval is also note as midi number

    if kval > -1 then
      event "i", "GSYNTH", 0, 1, kval, 832
    endif

    kCnt += 1
  od
endin

//CHORDS END
//-----------------------------------------------------------------------------------------------------------------------------

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//GAME SYNTH
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Wavetable numbers: sine = 710, square = 711, saw = 712, pulse = 713

gisyntrig init 0

//i GSYNTH [p3 = length] [p4 = note] [p5 = param table#] [p6 = panning, 0=L, 1=R]
// TABLE VALUES: [p5 = velocity] [p6 = osc waveform 0:sin 1:sqr 2:saw] [p7 = noise amp]
//  [p8 = filter frequency] [p9 = filter resonance] [p10 = filter env amount][p11,12,13,14 = filter A,D,S,R] [p15,16,17,18 = amp A,D,S,R]
//  [p19 = stereo width] [p20,21,22 = reverb amount, room size, damp]
instr GSYNTH

;pX = lfo waveform

//Input/midi variables
ifreq = pow(2,(p4-69)/12)*440 ;note as midi# value, is converted to frequency (Hz)

ifn = p5

ivel = tab_i(5,ifn) ;note velocity value

iwf = 700+tab_i(6,ifn)  ;waveform

inoise = tab_i(7,ifn) ;noise amount

//Filter parameters
iffreq = tab_i(8,ifn) ;lowpass filter frequency
ifres = tab_i(9,ifn)  ;lowpass filter resonance
ifenv_amt = tab_i(10,ifn) ;lowpass filter env amount

ifenv_att = tab_i(11,ifn) ;filter attack
ifenv_dec = tab_i(12,ifn) ;filter decay
ifenv_sus = tab_i(13,ifn) ;filter sustain
ifenv_rel = tab_i(14,ifn) ;filter release

//Amp parameters
iaenv_att = tab_i(15,ifn) ;amp attack
iaenv_dec = tab_i(16,ifn) ;amp decay
iaenv_sus = tab_i(17,ifn) ;amp sustain
iaenv_rel = tab_i(18,ifn) ;amp release

//Additional parameters
iwidth = tab_i(19,ifn)  ;stereo width

//FX
irev_amt = tab_i(20,ifn)
irev_roomsize = tab_i(21,ifn)
irev_damp = tab_i(22,ifn)

//Perform alternating execution when a note is played
if gisyntrig == 1 then
  kpan = 0.5 + iwidth / 2 ;set note panning r
  gisyntrig = 0
  else
    kpan = 0.5 - iwidth / 2
    gisyntrig = 1
endif

//LFOs
klfo1 lfo 1, 1


//OSCs
aosc1 poscil ivel, ifreq, iwf

anoise rand limit(inoise,0,1)

abus = aosc1 + anoise

//Filters
kfenv madsr ifenv_att, ifenv_dec, ifenv_sus, ifenv_rel  ;filter envelope


alp zdf_2pole abus, iffreq+kfenv*ifenv_amt, ifres ;filter signal


//Amp
aaenv madsr iaenv_att, iaenv_dec, iaenv_sus, iaenv_rel  ;amplitude envelope

abus = alp*aaenv

abusL, abusR pan2 abus, kpan  ;pan signal

arevL, arevR freeverb abusL, abusR, irev_roomsize, irev_damp

outs abusL+arevL*irev_amt, abusR+arevR*irev_amt

endin

//GAME SYNTH END
//-----------------------------------------------------------------------------------------------------------------------------

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//GAME SYNTH SNHMEL EDITION
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Wavetable numbers: sine = 710, square = 711, saw = 712, pulse = 713

//i GSYNTH [p3 = length] [p4 = note] [p5 = param table#] [p6 = panning, 0=L, 1=R]
// TABLE VALUES: [p5 = velocity] [p6 = osc waveform 0:sin 1:sqr 2:saw] [p7 = noise amp]
//  [p8 = filter frequency] [p9 = filter resonance] [p10 = filter env amount][p11,12,13,14 = filter A,D,S,R] [p15,16,17,18 = amp A,D,S,R]
//  [p19 = stereo width] [p20,21,22 = reverb amount, room size, damp]
instr GSYNTH_CONTIN

;pX = lfo waveform

//Input/midi variables
kfreq = pow(2,(gksnhmel_note-69)/12)*440 ;note as midi# value, is converted to frequency (Hz)

ifn = p5

kvel = tab_i(5,ifn) ;note velocity value

iwf = 700+tab_i(6,ifn)  ;waveform

knoise = tab_i(7,ifn) ;noise amount

//Filter parameters
kffreq = tab_i(8,ifn) ;lowpass filter frequency
kfres = tab_i(9,ifn)  ;lowpass filter resonance
kfenv_amt = tab_i(10,ifn) ;lowpass filter env amount

ifenv_att = tab_i(11,ifn) ;filter attack
ifenv_dec = tab_i(12,ifn) ;filter decay
ifenv_sus = tab_i(13,ifn) ;filter sustain
ifenv_rel = tab_i(14,ifn) ;filter release

//Amp parameters
iaenv_att = tab_i(15,ifn) ;amp attack
iaenv_dec = tab_i(16,ifn) ;amp decay
iaenv_sus = tab_i(17,ifn) ;amp sustain
iaenv_rel = tab_i(18,ifn) ;amp release

//Additional parameters
iwidth = tab_i(19,ifn)  ;stereo width

//FX
krev_amt = tab_i(20,ifn)
krev_roomsize = tab_i(21,ifn)
krev_damp = tab_i(22,ifn)

//TODO Panning may be modulated by k value
kpan = 0.5

//LFOs
klfo1 lfo 1, iwf
krd randomi 0.01, 0.1, 0.1  ;min,max,cps

//OSCs
aosc1 poscil krd, kfreq, 702

anoise rand limit(knoise,0,1)

abus = aosc1 + anoise

//Filters
kfenv madsr ifenv_att, ifenv_dec, ifenv_sus, ifenv_rel  ;filter envelope


alp zdf_2pole abus, kffreq+kfenv*kfenv_amt, kfres ;filter signal


//Amp
aaenv madsr iaenv_att, iaenv_dec, iaenv_sus, iaenv_rel  ;amplitude envelope

;abus = alp*aaenv

abusL, abusR pan2 abus, kpan  ;pan signal

arevL, arevR freeverb abusL, abusR, krev_roomsize, krev_damp

outs aosc1, aosc1
;outs abusL+arevL*krev_amt, abusR+arevR*krev_amt

endin

//GAME SYNTH CONTINUOUS END
//-----------------------------------------------------------------------------------------------------------------------------
</CsInstruments>
<CsScore>
f 0 z
i "CLOCK" 2 -1
i "EUC_STEP" 2 -1
i "SNHMEL" 2 -1
i "SNHBAS" 2 -1
</CsScore>
</CsoundSynthesizer>
