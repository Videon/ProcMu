using ProcMu.ScriptableObjects;

namespace ProcMu.UnityScripts
{
    /// <summary> Data type for assigning distance float value to music configuration. </summary>
    public struct McDist
    {
        public McDist(MuConfig config, float distance)
        {
            Mc = config;
            Dist = distance;
        }

        public MuConfig Mc { get; }
        public float Dist { get; }
    }
}