using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using MapSet = KaizerWaldCode.V2.Data.Settings.Map;
using ChunkSet = KaizerWaldCode.V2.Data.Settings.Chunk;
using VoxelSet = KaizerWaldCode.V2.Data.Settings.Voxel;
using NoiseSet = KaizerWaldCode.V2.Data.Settings.Noise;
namespace KaizerWaldCode.V2.UI
{
    public class GenerateButton : MonoBehaviour
    {
        private EntityManager _em;
        private InitValueData _mapSettingData;

        public TMP_InputField IsoSurfaceIF;
        public TMP_InputField BoundSizeIF;
        public TMP_InputField NumChunkIF;
        public TMP_InputField PointPerAxisIF;

        //Noise Settings

        public TMP_InputField SeedIF;
        public TMP_InputField OctavesIF;
        public TMP_InputField NoiseScaleIF;
        public TMP_InputField NoiseWeightIF;
        public TMP_InputField WeightMultiplierIF;

        private bool valueChanged;
        void Awake()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        void Start()
        {
            _mapSettingData = gameObject.GetComponent<InitValueData>();
        }

        /// <summary>
        /// Check if Noise Values changed
        /// </summary>
        /// <returns></returns>
        private bool NoiseSettingsChanged()
        {
            bool NoisevalueChanged = false;

            if (InputNotNull(SeedIF.text))
            {
                int seed = Int32.Parse(SeedIF.text);
                if (seed != _em.GetComponentData<NoiseSet.Seed>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new NoiseSet.Seed { Value = seed });
                    NoisevalueChanged = true;
                }
            }

            if (InputNotNull(OctavesIF.text))
            {
                int octaves = Int32.Parse(OctavesIF.text);
                if (octaves != _em.GetComponentData<NoiseSet.Octaves>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new NoiseSet.Octaves { Value = octaves });
                    NoisevalueChanged = true;
                }
            }

            if (InputNotNull(NoiseScaleIF.text))
            {
                float scale = Single.Parse(NoiseScaleIF.text);
                if (scale != _em.GetComponentData<NoiseSet.Scale>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new NoiseSet.Scale { Value = scale });
                    NoisevalueChanged = true;
                }
            }
            if (InputNotNull(NoiseWeightIF.text))
            {
                float noiseWeight = Single.Parse(NoiseWeightIF.text);
                if (noiseWeight != _em.GetComponentData<NoiseSet.NoiseWeight>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new NoiseSet.NoiseWeight { Value = noiseWeight });
                    NoisevalueChanged = true;
                }
            }
            if (InputNotNull(WeightMultiplierIF.text))
            {
                float weightMultiplier = Single.Parse(WeightMultiplierIF.text);
                if (weightMultiplier != _em.GetComponentData<NoiseSet.WeightMultiplier>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new NoiseSet.WeightMultiplier { Value = weightMultiplier });
                    NoisevalueChanged = true;
                }
            }

            return NoisevalueChanged;
        }

        public void UpdateMapData()
        {
            valueChanged = false;
            //check if values out of bounds
            if (!CheckInputValues()) return;
            //Check if Noise val Changed
            valueChanged = NoiseSettingsChanged();

            //Check if values are different
            if (InputNotNull(IsoSurfaceIF.text))
            {
                float isoSurface = Single.Parse(IsoSurfaceIF.text);
                if (isoSurface != _em.GetComponentData<MapSet.IsoSurface>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new MapSet.IsoSurface() { Value = isoSurface });
                    valueChanged = true;
                }
            }

            //BoundsSize
            if (InputNotNull(BoundSizeIF.text))
            {
                int bounds = Int32.Parse(BoundSizeIF.text);
                if (bounds > 0 && bounds != _em.GetComponentData<ChunkSet.ChunkBoundXZ>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new ChunkSet.ChunkBoundXZ() { Value = bounds });
                    _em.SetComponentData(_mapSettingData._mapSetting, new ChunkSet.ChunkBoundY() { Value = bounds });
                    _em.SetComponentData(_mapSettingData._mapSetting, new MapSet.MapBoundY() { Value = bounds });
                    valueChanged = true;
                }
            }

            //NumChunk change
            if (InputNotNull(NumChunkIF.text))
            {
                int numChunk = Int32.Parse(NumChunkIF.text);
                if (numChunk > 0 && numChunk != _em.GetComponentData<ChunkSet.NumChunk>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new ChunkSet.NumChunk { Value = numChunk });
                    valueChanged = true;
                }
            }

            //NumChunk change
            if (InputNotNull(PointPerAxisIF.text))
            {
                int pointPerAxis = Int32.Parse(PointPerAxisIF.text);
                if (pointPerAxis != _em.GetComponentData<ChunkSet.ChunkNumPointPerAxisXZ>(_mapSettingData._mapSetting).Value)
                {
                    _em.SetComponentData(_mapSettingData._mapSetting, new ChunkSet.ChunkNumPointPerAxisXZ { Value = pointPerAxis });
                    _em.SetComponentData(_mapSettingData._mapSetting, new ChunkSet.ChunkNumPointPerAxisY { Value = pointPerAxis });
                    valueChanged = true;
                }
            }

            //Refresh labels On UI to match changed values
            if (valueChanged)
            {
                _mapSettingData.RefreshLabels();
                UpdateBounds();
                _em.AddComponent<V2.Data.Event.Event_ProcessPointsPosition>(_mapSettingData._eventHolder);
            }

        }

        /// <summary>
        /// Updates all values on the mapSetting Entity
        /// </summary>
        void UpdateBounds()
        {
            int bounds = _em.GetComponentData<ChunkSet.ChunkBoundXZ>(_mapSettingData._mapSetting).Value;
            int numChunk = _em.GetComponentData<ChunkSet.NumChunk>(_mapSettingData._mapSetting).Value;
            int numPointPerAxis = _em.GetComponentData<ChunkSet.ChunkNumPointPerAxisXZ>(_mapSettingData._mapSetting).Value;
            //Only Map related changed

            _em.SetComponentData(_mapSettingData._mapSetting, new ChunkSet.PointSpacing() { Value = (float)bounds / (float)(numPointPerAxis-1) });
            _em.SetComponentData(_mapSettingData._mapSetting, new MapSet.MapBoundXZ() { Value = math.mul(bounds,numChunk) });
            _em.SetComponentData(_mapSettingData._mapSetting, new MapSet.MapNumPointPerAxisXZ() { Value = math.mul(numPointPerAxis, numChunk) });
            _em.SetComponentData(_mapSettingData._mapSetting, new MapSet.MapNumPointPerAxisY() { Value = numPointPerAxis });
            //Voxel
            _em.SetComponentData(_mapSettingData._mapSetting, new VoxelSet.ChunkNumVoxelPerAxisXZ() { Value = numPointPerAxis - 1 });
            _em.SetComponentData(_mapSettingData._mapSetting, new VoxelSet.ChunkNumVoxelPerAxisY() { Value = numPointPerAxis - 1 });

            _em.SetComponentData(_mapSettingData._mapSetting, new VoxelSet.MapNumVoxelPerAxisXZ() { Value = math.mad(numPointPerAxis,numChunk,-1) });
            _em.SetComponentData(_mapSettingData._mapSetting, new VoxelSet.MapNumVoxelPerAxisY() { Value = numPointPerAxis - 1 });

        }

        // IsoSurface change -> nothing
        // BoundSize change -> (ChunkboundXZ + CunkBoundY) + (MapBoundXZ)


        /// <summary>
        /// Check if Inputs are valid (not out of bounds)
        /// empty inputs is not considered invalid
        /// </summary>
        /// <returns>true : all value are valid / false : one value is invalid</returns>
        public bool CheckInputValues()
        {
            if (InputNotNull(BoundSizeIF.text))
            {
                if (Int32.Parse(BoundSizeIF.text) <= 0) { return false; }
            }

            if (InputNotNull(NumChunkIF.text))
            {
                if (Int32.Parse(NumChunkIF.text) <= 0) { return false; }
            }

            if (InputNotNull(SeedIF.text))
            {
                if (Int32.Parse(SeedIF.text) < 0) { return false; }
            }

            if (InputNotNull(OctavesIF.text))
            {
                if (Int32.Parse(OctavesIF.text) < 1) { return false; }
            }

            if (InputNotNull(NoiseScaleIF.text))
            {
                if (Single.Parse(NoiseScaleIF.text) < 0.001f) { return false; }
            }

            return true;
        }



        /// <summary>
        /// Check if the input entry is not empty
        /// </summary>
        /// <param name="s">input to check</param>
        /// <returns>true:not null / false:is null</returns>
        bool InputNotNull(string s)
        {
            return s != "" ? true : false;
        }
    }
}
