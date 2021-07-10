using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

namespace KaizerWaldCode
{
    public class InitValueData : MonoBehaviour
    {
        private V2.EntityStore _mapSettingsStored;
        private EntityManager _em;
        
        public Entity _mapSetting;
        public Entity _eventHolder;

        public Text IsoSurfaceData;
        public Text ChunkBoundData;
        private float isoValue;
        // Start is called before the first frame update
        void Awake()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        void Start()
        {
            _mapSetting = GetComponent<V2.EntityStore>().entity;
            _eventHolder = GetComponent<V2.EntityStore>().EventHolder;
            //isoValue = _em.GetComponentData<V2.Data.Settings.Map.IsoSurface>(_mapSetting).Value;
            //Data = GetComponent<UnityEngine.UI.Text>();
            IsoSurfaceData.text = _em.GetComponentData<V2.Data.Settings.Map.IsoSurface>(_mapSetting).Value.ToString();
            ChunkBoundData.text = _em.GetComponentData<V2.Data.Settings.Chunk.ChunkBoundXZ>(_mapSetting).Value.ToString();
        }

    }
}
