using UnityEngine;

namespace ProcMu.CSUnity
{
    [CreateAssetMenu(fileName = "muc_", menuName = "ProcMu/Music configuration", order = 1)]
    public class MuConfig : ScriptableObject
    {
        #region Global parameters

        [SerializeField] private double bpm = 120;
        [SerializeField] private MuScale scale;

        #endregion

        #region EUCRTH parameters

        #endregion


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}