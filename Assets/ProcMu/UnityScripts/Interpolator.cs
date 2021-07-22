using System;
using ProcMu.ScriptableObjects;
using ProcMu.UnityScripts.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProcMu.UnityScripts
{
    /// <summary> Interpolates between multiple ProcMu configurations </summary>
    public class Interpolator : MonoBehaviour
    {
        [SerializeField] private ProcMuMaster procMuMaster;

        [SerializeField] private LayerMask layerMask;

        [SerializeField, Tooltip("Player object transform for position tracking.")]
        private Transform playerTransform;

        [SerializeField, Tooltip("The maximum distance of a music zone center away from the player to be considered.")]
        private float maxDistance;

        [SerializeField, Tooltip("The maximum number of music zones to consider for interpolation.")]
        private int maxZones;

        [SerializeField, Tooltip("Checks per second: How often music configuration is evaluated.")]
        private float cps = 2;

        #region private variables

        private readonly Collider[] _colliders = new Collider[16]; //Pre-allocated memory for found music zone colliders

        private Vector3 _playerPos;

        private MuConfig _muConfig;

        [SerializeField]
        private readonly McDist[] _mcs = new McDist[16]; //List of music configurations with attached distance value.

        private float elapsedTime = 0f;

        /// <summary> Cumulated distance factors for all music zones. </summary>
        private float distanceSum = 0f;

        #endregion

        #region test variables

        public string[] distances = new string[16];

        #endregion

        private void Awake()
        {
            _muConfig = procMuMaster.mc;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < 1f / cps) return; //Only proceed if interval has passed.

            elapsedTime = 0f;

            FindAndSortMusicZones();

            //Generate distances array from sorted McDist objects in list
            float[] distances = new float[maxZones];
            for (int i = 0; i < maxZones; i++)
            {
                if (_mcs[i].Mc == null) break;
                
                distances[i] = _mcs[i].Dist;
            }

            procMuMaster.mc.chords_synthconfig.config = Interpolate(distances, CumulateConfigs());
        }

        private void FindAndSortMusicZones()
        {
            Array.Clear(distances, 0, distances.Length);
            Array.Clear(_mcs, 0, _mcs.Length); //Clear list before filling it again with data.

            int found = Physics.OverlapSphereNonAlloc(playerTransform.position, maxDistance, _colliders, layerMask);

            //Use number of colliders as top if there are fewer colliders than space in the array.
            int top = found < _colliders.Length ? found : _colliders.Length;

            for (int i = 0; i < top; i++)
            {
                float dist = 1f / Vector3.Distance(playerTransform.position, _colliders[i].transform.position);

                _mcs[i] = new McDist(_colliders[i].GetComponent<MusicZone>().Config, dist);
            }

            Array.Sort(_mcs, ProcMuUtils.CompareMcDists); //Sort objects according to distance.

            distanceSum = 0f; //Reset cumulated distances to 0


#if UNITY_EDITOR //EDITOR FUNCTION: LIST MUSIC ZONE DISTANCES
            for (int i = 0; i < _mcs.Length; i++)
            {
                if (_mcs[i].Mc == null) break;

                distanceSum += _mcs[i].Dist;

                distances[i] = _mcs[i].Dist.ToString();
            }
#endif
        }

        //TODO: NEEDS MORE FEATURES. CREATE A NEW MUSIC CONFIG FROM WEIGHTED INTERPOLATIONS/RANDOMIZATIONS BETWEEN MULTIPLE MUSIC ZONES!
        /// <summary> Chooses one of multiple MusicZone configurations, with probabilities dependent on their distance to the player. </summary>
        /// <returns> Returns a configuration if found, null if no Music Zone could be determined. </returns>
        private MuConfig GetMusicZoneConfigWeighted()
        {
            float step = 0;
            float rand = Random.Range(0, distanceSum);
            for (int i = 0; i < _mcs.Length; i++)
            {
                if (rand <= step + _mcs[i].Dist) return _mcs[i].Mc;
                step += _mcs[i].Dist;
            }

            return null;
        }

        private double[][] CumulateConfigs()
        {
            double[][] output = new double[maxZones][];
            for (int i = 0; i < output.Length; i++)
            {
                if (_mcs[i].Mc == null) break;
                
                output[i] = _mcs[i].Mc.chords_synthconfig.config;
            }

            return output;
        }

        /// <summary> Performs weighted interpolation of values between multiple arrays. </summary>
        /// <param name="distances"> Weight per array. Values must be in order of input arrays. </param>
        /// <param name="inputs"> Input arrays. Must all be the same length. </param>
        /// <returns> Array of interpolated values. </returns>
        private double[] Interpolate(float[] distances, double[][] inputs)
        {
            double[] output = new double[inputs.Length];

            for (int i = 0; i < inputs.Length; i++)
            {
                double[] values = new double[inputs[0].Length];
                for (int j = 0; j < values.Length; j++)
                {
                    values[j] = inputs[i][j];
                }

                output[i] = Interpolate(distances, values);
            }

            return output;
        }

        /// <summary> Performs weighted interpolation between multiple values. </summary>
        /// <param name="distances"> Weight per value. Weights must be in order of inputs. </param>
        /// <param name="values"> Input values. </param>
        /// <returns> Interpolated value. </returns>
        private double Interpolate(float[] distances, double[] values)
        {
            double sumW = 0;
            double sumWz = 0;

            for (int i = 0; i < values.Length; i++)
            {
                double w = 1 / Mathf.Pow(distances[i], 2f);
                sumW += w;

                sumWz += values[i] * w;
            }

            return sumWz / sumW;
        }
    }
}