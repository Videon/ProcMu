using ProcMu.ScriptableObjects;
using ProcMu.UnityScripts;
using ProcMu.UnityScripts.Utilities;
using UnityEditor;
using UnityEngine;

namespace ProcMu.CSUnity.Editor
{
    /// <summary> Editor class for music scale object. </summary>   TODO Add automatic scale builder
    [CustomEditor(typeof(MuScale))]
    public class MuScaleEditor : UnityEditor.Editor
    {
        private Tonic _tonic;
        private Scale _scale;

        public override void OnInspectorGUI()
        {
            //Assign object variables
            MuScale scale = target as MuScale;
            bool[] activeNotes = scale.Scale;

            int arrlen = activeNotes.Length;

            DrawScale(scale, activeNotes, arrlen);
            DrawGenerateDialogue(scale, ref activeNotes, arrlen);

            EditorUtility.SetDirty(target); //TODO Set dirty only when something was changed!
        }

        #region Drawing elements

        private void DrawScale(MuScale scale, bool[] activeNotes, int arrlen)
        {
            EditorGUILayout.BeginVertical();
            while (arrlen > 0)
            {
                int top = 0;
                if (arrlen % 12 != 0)
                    top = arrlen % 12;
                else top = 12;

                arrlen -= top;

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < top; i++)
                {
                    int index = arrlen + i;
                    activeNotes[index] = EditorGUILayout.ToggleLeft(NoteToName(index), activeNotes[index],
                        StyleFromNoteIndex(index),
                        GUILayout.Width(45f), GUILayout.Height(IsBlackKey(index) ? 10f : 40f));
                }

                scale.Scale = activeNotes; //Update origin array with changes from ui.

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawGenerateDialogue(MuScale scale, ref bool[] activeNotes, int arrlen)
        {
            EditorGUILayout.BeginHorizontal();

            _tonic = (Tonic) EditorGUILayout.EnumPopup(_tonic);
            _scale = (Scale) EditorGUILayout.EnumPopup(_scale);

            if (GUILayout.Button("Generate scale"))
            {
                ProcMuUtils.GenerateScale(_tonic, _scale, ref activeNotes);
                DrawScale(scale, activeNotes, arrlen);
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Utility classes

        private string NoteToName(int noteIndex)
        {
            string name = (noteIndex % 12) switch
            {
                0 => "C",
                1 => "C#",
                2 => "D",
                3 => "D#",
                4 => "E",
                5 => "F",
                6 => "F#",
                7 => "G",
                8 => "G#",
                9 => "A",
                10 => "A#",
                11 => "B",
                _ => "ERR"
            };

            return name += (int) (noteIndex / 12);
        }

        private GUIStyle StyleFromNoteIndex(int noteIndex)
        {
            GUIStyle gs = new GUIStyle();
            gs.normal.textColor = IsBlackKey(noteIndex) ? Color.black : Color.white;

            return gs;
        }

        private bool IsBlackKey(int noteIndex)
        {
            return (noteIndex % 12) switch
            {
                0 => false,
                1 => true,
                2 => false,
                3 => true,
                4 => false,
                5 => false,
                6 => true,
                7 => false,
                8 => true,
                9 => false,
                10 => true,
                11 => false,
                _ => false
            };
        }

        #endregion
    }
}