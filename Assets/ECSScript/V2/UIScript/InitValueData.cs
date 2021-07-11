using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

namespace KaizerWaldCode.V2.UI
{
    public class InitValueData : MonoBehaviour
    {
        private V2.EntityStore _mapSettingsStored;
        private EntityManager _em;
        
        public Entity _mapSetting;
        public Entity _eventHolder;

        public TMP_Text IsoSurfaceData;
        public TMP_Text ChunkBoundData;
        public TMP_Text NumChunk;
        public TMP_Text PointPerAxis;

        //Noise Settings
        public TMP_Text Seed;
        public TMP_Text Octaves;
        public TMP_Text NoiseScale;
        public TMP_Text NoiseWeight;
        public TMP_Text NoiseMultiplier;
        public TMP_Text NoiseMinValue;
        // Start is called before the first frame update
        void Awake()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        void Start()
        {
            RefreshLabels();
        }

        public void RefreshLabels()
        {
            _mapSetting = GetComponent<V2.EntityStore>().entity;
            _eventHolder = GetComponent<V2.EntityStore>().EventHolder;
            IsoSurfaceData.text = _em.GetComponentData<V2.Data.Settings.Map.IsoSurface>(_mapSetting).Value.ToString();
            ChunkBoundData.text = _em.GetComponentData<V2.Data.Settings.Chunk.ChunkBoundXZ>(_mapSetting).Value.ToString();
            NumChunk.text = _em.GetComponentData<V2.Data.Settings.Chunk.NumChunk>(_mapSetting).Value.ToString();
            PointPerAxis.text = _em.GetComponentData<V2.Data.Settings.Chunk.ChunkNumPointPerAxisXZ>(_mapSetting).Value.ToString();

            Seed.text = _em.GetComponentData<V2.Data.Settings.Noise.Seed>(_mapSetting).Value.ToString();
            Octaves.text = _em.GetComponentData<V2.Data.Settings.Noise.Octaves>(_mapSetting).Value.ToString();
            NoiseScale.text = _em.GetComponentData<V2.Data.Settings.Noise.Scale>(_mapSetting).Value.ToString();
            NoiseWeight.text = _em.GetComponentData<V2.Data.Settings.Noise.NoiseWeight>(_mapSetting).Value.ToString();
            NoiseMultiplier.text = _em.GetComponentData<V2.Data.Settings.Noise.WeightMultiplier>(_mapSetting).Value.ToString();
            NoiseMinValue.text = _em.GetComponentData<V2.Data.Settings.Noise.NoiseMinValue>(_mapSetting).Value.ToString();
        }

    }
}
