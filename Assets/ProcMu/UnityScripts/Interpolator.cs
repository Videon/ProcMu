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

            int inner = CheckInner(top);

            //If check position is within inner radius of a zone, use its config, otherwise interpolate.
            if (inner > -1) ProcMuUtils.CopyMuConfig(_mcs[inner].Mc, procMuMaster.mc);
            else InterpolateAll(distances, top);
        }

        /// <summary> Checks whether player is in inner zone of any music zone in reach and returns its index. </summary>
        /// <returns> Index of zone whose inner zone the player occupies. -1 if not in any inner zone. </returns>
        private int CheckInner(int top)
        {
            for (int i = 0; i < top; i++)
            {
                if (_mcs[i].Dist <= _mcs[i].RangeInner) return i;
            }

            return -1;
        }

        /// <summary> Finds all music zones within range and puts them in McDist array. </summary>
        /// <returns> Number of found music zones. </returns>
        private int FindAndSortMusicZones()
        {
            Array.Clear(_mcs, 0, _mcs.Length); //Clear list before filling it again with data.

            int found = Physics.OverlapSphereNonAlloc(playerTransform.position, maxDistance, _colliders, layerMask);

            //Use number of colliders as top if there are fewer colliders than space in the array.
            int top = found < _colliders.Length ? found : _colliders.Length;

            for (int i = 0; i < top; i++)
            {
                float dist;


                dist = Vector3.Distance(playerTransform.position, _colliders[i].transform.position);

                MusicZone mz = _colliders[i].GetComponent<MusicZone>();

                _mcs[i] = new McDist(mz.Config, dist, mz.RadiusInner);
            }

            Array.Sort(_mcs, ProcMuUtils.CompareMcDists); //Sort objects according to distance.

            distanceSum = 0f; //Reset cumulated distances to 0

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
            int[][] cumulatedJaggedInt = new int[top][]; //Allocating array for cumulating values across configs.
            double[][] cumulatedJaggedDouble = new double[top][];

            #region Global parameters

            //TODO EVALUATE WHETHER BPM SHOULD ALSO BE INTERPOLATED
            //procMuMaster.mc.bpm = _mcs[0].Mc.bpm

            //Getting scale from closest music zone, which is at index = 0 as _mcs array is sorted by distance.
            ProcMuUtils.CopyScale(_mcs[0].Mc.Scale, procMuMaster.mc.Scale);

            procMuMaster.mc.activeBars0 = _mcs[0].Mc.activeBars0;
            procMuMaster.mc.activeBars1 = _mcs[0].Mc.activeBars1;

            #endregion

            #region EUCRTH parameters

            //Sets each eucrth sound randomly, chance of selected sound depends on distance of player to zone.
            for (int i = 0; i < procMuMaster.mc.eucrth_sampleSelection.Length; i++)
                procMuMaster.mc.eucrth_sampleSelection[i] =
                    _mcs[GetIndexWeighted(distances, top)].Mc.eucrth_sampleSelection[i];

            for (int i = 0; i < cumulatedJaggedInt.Length; i++)
                cumulatedJaggedInt[i] = _mcs[i].Mc.eucrth_minImpulses0;

            procMuMaster.mc.eucrth_minImpulses0 = Interpolate(distances, cumulatedJaggedInt);

            for (int i = 0; i < cumulatedJaggedInt.Length; i++)
                cumulatedJaggedInt[i] = _mcs[i].Mc.eucrth_maxImpulses0;

            procMuMaster.mc.eucrth_maxImpulses0 = Interpolate(distances, cumulatedJaggedInt);

            for (int i = 0; i < cumulatedJaggedInt.Length; i++)
                cumulatedJaggedInt[i] = _mcs[i].Mc.eucrth_minImpulses1;

            procMuMaster.mc.eucrth_minImpulses1 = Interpolate(distances, cumulatedJaggedInt);

            for (int i = 0; i < cumulatedJaggedInt.Length; i++)
                cumulatedJaggedInt[i] = _mcs[i].Mc.eucrth_maxImpulses1;

            procMuMaster.mc.eucrth_maxImpulses1 = Interpolate(distances, cumulatedJaggedInt);

            #endregion

            #region CHORDS parameters

            for (int i = 0; i < cumulatedJaggedDouble.Length; i++)
            {
                cumulatedJaggedDouble[i] = _mcs[i].Mc.chords_config;
            }

            procMuMaster.mc.chords_config = Interpolate(distances, cumulatedJaggedDouble);

            procMuMaster.mc.chordMode = _mcs[0].Mc.chordMode;

            procMuMaster.mc.chords_synthconfig.config =
                Interpolate(distances, CumulateConfigs(top, Instrument.Chords));

            #endregion

            #region SNHMEL parameters

            for (int i = 0; i < cumulatedJaggedDouble.Length; i++)
            {
                cumulatedJaggedDouble[i] = _mcs[i].Mc.snhmel_config;
            }

            procMuMaster.mc.snhmel_config = Interpolate(distances, cumulatedJaggedDouble);

            //Misc

            procMuMaster.mc.snhmel_melodycurve = _mcs[0].Mc.snhmel_melodycurve;
            procMuMaster.mc.snhmel_melodymode = _mcs[0].Mc.snhmel_melodymode;

            procMuMaster.mc.snhmel_synthconfig.config =
                Interpolate(distances, CumulateConfigs(top, Instrument.SnhMel));

            #endregion

            #region SNHBAS parameters

            for (int i = 0; i < cumulatedJaggedDouble.Length; i++)
            {
                cumulatedJaggedDouble[i] = _mcs[i].Mc.snhbas_config;
            }

            procMuMaster.mc.snhbas_config = Interpolate(distances, cumulatedJaggedDouble);

            //Misc

            procMuMaster.mc.snhbas_melodycurve = _mcs[0].Mc.snhbas_melodycurve;
            procMuMaster.mc.snhbas_melodymode = _mcs[0].Mc.snhbas_melodymode;

            procMuMaster.mc.snhbas_synthconfig.config =
                Interpolate(distances, CumulateConfigs(top, Instrument.SnhMel));

            #endregion
        }

        /// <summary> Performs weighted interpolation of multiple arrays of double config values. </summary>
        /// <param name="distances"> Weight per array. Values must be in order of input arrays. </param>
        /// <param name="inputs"> Input arrays. Must all be the same length. </param>
        /// <returns> Array of interpolated values. </returns>
        private double[] Interpolate(float[] distances, double[][] inputs)
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

        /// <summary> Performs weighted interpolation of multiple arrays of int config values. </summary>
        /// <param name="distances"> Weight per array. Values must be in order of input arrays. </param>
        /// <param name="inputs"> Input arrays. Must all be the same length. </param>
        /// <returns> Array of interpolated values. </returns>
        private int[] Interpolate(float[] distances, int[][] inputs)
        {
            int[] output = new int[inputs[0].Length];

            for (int i = 0; i < inputs[0].Length; i++)
            {
                int[] values = new int[inputs.Length];
                for (int j = 0; j < inputs.Length; j++)
                {
                    values[j] = inputs[j][i];
                }

                output[i] = Interpolate(distances, values);
            }

            return output;
        }

        /// <summary> Performs weighted interpolation between multiple int values. </summary>
        /// <param name="distances"> Weight per value. Weights must be in order of inputs. </param>
        /// <param name="values"> Input values. </param>
        /// <returns> Interpolated value. </returns>
        private int Interpolate(float[] distances, int[] values)
        {
            float sumW = 0;
            float sumWz = 0;

            for (int i = 0; i < values.Length; i++)
            {
                float w;

                //Tiny chance for division by zero if distance to player is very small, therefore using MinValue as catch value.
                try
                {
                    w = 1 / Mathf.Pow(distances[i], 2f);
                }
                catch (DivideByZeroException)
                {
                    w = 1 / float.MinValue;
                }

                sumW += w;

                sumWz += values[i] * w;
            }

            return Mathf.RoundToInt(sumWz / sumW);
        }

        /// <summary> Performs weighted interpolation between multiple float values. </summary>
        /// <param name="distances"> Weight per value. Weights must be in order of inputs. </param>
        /// <param name="values"> Input values. </param>
        /// <returns> Interpolated value. </returns>
        private double Interpolate(float[] distances, double[] values)
        {
            double sumW = 0;
            double sumWz = 0;

            for (int i = 0; i < values.Length; i++)
            {
                double w;
                //Tiny chance for division by zero if distance to player is very small, therefore using MinValue as catch value.
                try
                {
                    w = 1 / Mathf.Pow(distances[i], 2f);
                }
                catch (DivideByZeroException)
                {
                    w = 1 / Double.MinValue;
                }

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

        private int GetIndexWeighted(float[] distances, int amount)
        {
            int[] indices = new int[amount];
            for (int i = 0; i < indices.Length; i++) indices[i] = i; //Fill array with 0 to amount, increasing order.


            float distanceSum = 0f;
            for (int i = 0; i < distances.Length; i++) distanceSum += distances[i];

            float random = Random.Range(0, distanceSum);

            for (int i = 0; i < distances.Length; i++)
            {
                if (random <= distances[i]) return i;
            }

            return 0;
        }
    }
}