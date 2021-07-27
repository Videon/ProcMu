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

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime < 1f / cps) return; //Only proceed if interval has passed.

            elapsedTime = 0f;

            int top = FindAndSortMusicZones();

            //Generate distances array from sorted McDist objects in list
            float[] distances = new float[top];
            for (int i = 0; i < top; i++)
            {
                if (_mcs[i].Mc == null) break;

                distances[i] = _mcs[i].Dist;
            }

            InterpolateAll(distances, top);
        }

        /// <summary> Finds all music zones within range and puts them in McDist array. </summary>
        /// <returns> Number of found music zones. </returns>
        private int FindAndSortMusicZones()
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

            return top;
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

        /// <summary> Collects all configuration parameters of one type into one jagged array. </summary>
        /// <param name="amount">Number of configs to cumulate. This should correspond with number of Music Zones in reach.</param>
        /// <returns>The specified amount of configurations in one jagged array.</returns>
        private double[][] CumulateConfigs(int amount, Instrument instrument)
        {
            double[][] output = new double[amount][];
            for (int i = 0; i < output.Length; i++)
            {
                if (_mcs[i].Mc == null) break;

                switch (instrument)
                {
                    case Instrument.EucRth:
                        throw new NotImplementedException();
                        break;
                    case Instrument.Chords:
                        output[i] = _mcs[i].Mc.chords_synthconfig.config;
                        break;
                    case Instrument.SnhMel:
                        output[i] = _mcs[i].Mc.snhmel_synthconfig.config;
                        break;
                    case Instrument.SnhBass:
                        throw new NotImplementedException();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(instrument), instrument, null);
                }
            }

            return output;
        }

        private void InterpolateAll(float[] distances, int top)
        {
            procMuMaster.mc.chords_synthconfig.config =
                InterpolateGsynthConfigs(distances, CumulateConfigs(top, Instrument.Chords));
            procMuMaster.mc.snhmel_synthconfig.config =
                InterpolateGsynthConfigs(distances, CumulateConfigs(top, Instrument.SnhMel));
        }

        /// <summary> Performs weighted interpolation of multiple arrays of config values. </summary>
        /// <param name="distances"> Weight per array. Values must be in order of input arrays. </param>
        /// <param name="inputs"> Input arrays. Must all be the same length. </param>
        /// <returns> Array of interpolated values. </returns>
        private double[] InterpolateGsynthConfigs(float[] distances, double[][] inputs)
        {
            double[] output = new double[inputs[0].Length];

            for (int i = 0; i < inputs[0].Length; i++)
            {
                double[] values = new double[inputs.Length];
                for (int j = 0; j < inputs.Length; j++)
                {
                    values[j] = inputs[j][i];
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

        private double ClosestValue(float[] distances, double[] values)
        {
            int closestIndex = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] < distances[closestIndex])
                    closestIndex = i;
            }

            return values[closestIndex];
        }
    }
}