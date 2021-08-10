using ProcMu.UnityScripts;
using UnityEditor;

namespace ProcMu.ScriptableObjects.Editor
{
    [CustomEditor(typeof(GSynthConfig))]
    public class GSynthConfigEditor : UnityEditor.Editor
    {
        private GSynthConfig _gSynthConfig;

        private void OnEnable()
        {
            _gSynthConfig = target as GSynthConfig;
        }

        public override void OnInspectorGUI()
        {
            _gSynthConfig.Velocity = EditorGUILayout.Slider("Velocity", (float) _gSynthConfig.Velocity, 0f, 1f);
            _gSynthConfig.Wavetable = (Wavetable) EditorGUILayout.EnumPopup(_gSynthConfig.Wavetable);
            _gSynthConfig.Noise = EditorGUILayout.Slider("Noise", (float) _gSynthConfig.Noise, 0f, 1f);

            _gSynthConfig.ffreq =
                EditorGUILayout.Slider("LP filter frequency", (float) _gSynthConfig.ffreq, 0f, 20000f);
            _gSynthConfig.fres = EditorGUILayout.Slider("LP filter resonance", (float) _gSynthConfig.fres, 0.5f, 25f);

            _gSynthConfig.fenv_amt =
                EditorGUILayout.Slider("LP filter envelope amount", (float) _gSynthConfig.fenv_amt, 0f, 20000f);
            _gSynthConfig.fenv_att =
                EditorGUILayout.Slider("LP filter envelope attack time", (float) _gSynthConfig.fenv_att, 0f, 10f);
            _gSynthConfig.fenv_dec =
                EditorGUILayout.Slider("LP filter envelope decay time", (float) _gSynthConfig.fenv_dec, 0f, 10f);
            _gSynthConfig.fenv_sus = EditorGUILayout.Slider("LP filter envelope sustain level",
                (float) _gSynthConfig.fenv_sus, 0f, 1f);
            _gSynthConfig.fenv_rel =
                EditorGUILayout.Slider("LP filter envelope release time", (float) _gSynthConfig.fenv_rel, 0f, 10f);

            _gSynthConfig.aenv_att =
                EditorGUILayout.Slider("Amplitude envelope attack time", (float) _gSynthConfig.aenv_att, 0f, 10f);
            _gSynthConfig.aenv_dec =
                EditorGUILayout.Slider("Amplitude envelope decay time", (float) _gSynthConfig.aenv_dec, 0f, 10f);
            _gSynthConfig.aenv_sus = EditorGUILayout.Slider("Amplitude envelope sustain level",
                (float) _gSynthConfig.aenv_sus, 0f, 1f);
            _gSynthConfig.aenv_rel =
                EditorGUILayout.Slider("Amplitude envelope release time", (float) _gSynthConfig.aenv_rel, 0f, 10f);

            _gSynthConfig.width = EditorGUILayout.Slider("Auto pan stereo width", (float) _gSynthConfig.width, 0f, 1f);
            
            _gSynthConfig.rev_amt = EditorGUILayout.Slider("Reverb fx amount", (float) _gSynthConfig.rev_amt, 0f, 1f);
            _gSynthConfig.rev_roomsize = EditorGUILayout.Slider("Reverb fx room size", (float) _gSynthConfig.rev_roomsize, 0f, 1f);
            _gSynthConfig.rev_damp = EditorGUILayout.Slider("Reverb fx frequency damping", (float) _gSynthConfig.rev_damp, 0f, 1f);
            
            EditorUtility.SetDirty(_gSynthConfig); //TODO SET ONLY DIRTY WHEN A CHANGE WAS MADE
        }
    }
}