using UnityEngine;

namespace ProcMu.UnityScripts
{
    public class ParamController : MonoBehaviour
    {
        [SerializeField] private CharacterController cc;
        // Start is called before the first frame update

        [SerializeField] private CsoundUnity _csoundUnity;
        private bool _isInitialized;

        private void Awake()
        {
            //Assign CsoundUnity component
            _csoundUnity = GetComponent<CsoundUnity>();
            if (!_csoundUnity) Debug.LogError("Can't find CsoundUnity component!");
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!_csoundUnity.IsInitialized) return;

            _csoundUnity.SetChannel("gIntensity", cc.velocity.magnitude);
        }
    }
}