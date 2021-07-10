using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Animation;
using Unity.Animation.Hybrid;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.UI;
using AnimationCurve = Unity.Animation.AnimationCurve;
using Debug = UnityEngine.Debug;
using MapSett = KaizerWaldCode.Data.MapSettings;

namespace KaizerWaldCode.Data.Conversion
{
    [DisallowMultipleComponent]
    public class MapSettings : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("Perlin Noise")]
        [SerializeField] int _mapSize;
        [SerializeField] int _chunkSize;
        [Tooltip("num chunk is calculated as a table exemple : numchunk = 100 (100 X 100)")]
        [SerializeField] int _numChunk = 4;
        [SerializeField] int _seed = 1;
        [SerializeField] int _octaves = 6;
        [SerializeField] float _scale = 1.0f;
        [SerializeField] float _lacunarity = 2.0f;
        [Range(0,1)]
        [SerializeField] float _persistance = 0.5f;
        [SerializeField] float3 _offset;
        [SerializeField] float _heightMultiplier;
        [Range(0, 6)]
        [SerializeField] int _levelOfDetail;
        [Space]
        [SerializeField] UnityEngine.AnimationCurve _animationCurve;

        [Header("Voxel Settings")] 
        [SerializeField] float _isoLevel = 0.5f;
        [Range(2, 100)]
        [SerializeField] int _numPointsPerAxis = 30;
        [SerializeField] float _noiseWeight = 1;
        [SerializeField] float _floorOffset = 1;
        [SerializeField] float _weightMultiplier = 1;
        [SerializeField] float _hardFloorHeight;
        [SerializeField] float _hardFloorWeight;
        [SerializeField] float4 _shaderParams = new float4(1f,0,0,0);
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            #region Check Values
            _chunkSize = _chunkSize <= 0 ? 241 : _chunkSize;
            _numChunk = math.max(1, _numChunk);
            _mapSize = _chunkSize * _numChunk;
            _seed = math.max(1, _seed);
            _octaves = math.max(1, _octaves);
            _scale = math.max(0.0001f, _scale);
            _lacunarity = math.max(1f, _lacunarity);
            _heightMultiplier = math.max(1f, _heightMultiplier);
            _numPointsPerAxis = _numPointsPerAxis >= _mapSize ? _mapSize - 1 : _numPointsPerAxis;
            float pointSpacing = (float)_chunkSize / ((float)_numPointsPerAxis - 1);
            #endregion Check Values

            dstManager.AddComponent<Tag.MapSettings>(entity);
            dstManager.RemoveComponent<LocalToWorld>(entity);
            dstManager.RemoveComponent<Translation>(entity);
            dstManager.RemoveComponent<Rotation>(entity);
            //Perlin Noise
            dstManager.AddComponentData(entity, new MapSett.MapSize {Value = _mapSize});
            dstManager.AddComponentData(entity, new MapSett.ChunkSize { Value = _chunkSize });
            dstManager.AddComponentData(entity, new MapSett.NumChunk { Value = _numChunk });
            dstManager.AddComponentData(entity, new MapSett.Octaves { Value = _octaves });
            dstManager.AddComponentData(entity, new MapSett.Scale { Value = _scale });
            dstManager.AddComponentData(entity, new MapSett.Seed { Value = _seed });
            dstManager.AddComponentData(entity, new MapSett.Lacunarity { Value = _lacunarity });
            dstManager.AddComponentData(entity, new MapSett.Persistance { Value = _persistance });
            dstManager.AddComponentData(entity, new MapSett.Offset { Value = _offset });
            dstManager.AddComponentData(entity, new MapSett.HeightMultiplier { Value = _heightMultiplier });
            dstManager.AddComponentData(entity, new MapSett.LevelOfDetail { Value = _levelOfDetail });
            dstManager.AddComponentData(entity, new MapSett.HeightCurve { Value = _animationCurve.ToDotsAnimationCurve() });
            //Voxel
            dstManager.AddComponentData(entity, new Voxel.IsoLevel { Value = _isoLevel });
            dstManager.AddComponentData(entity, new Voxel.FloorOffset { Value = _floorOffset });
            dstManager.AddComponentData(entity, new Voxel.PointSpacing { Value = pointSpacing });
            dstManager.AddComponentData(entity, new Voxel.HardFloor { Value = _hardFloorHeight });
            dstManager.AddComponentData(entity, new Voxel.HardFloorWeight { Value = _hardFloorWeight });
            dstManager.AddComponentData(entity, new Voxel.NoiseWeight { Value = _noiseWeight });
            dstManager.AddComponentData(entity, new Voxel.NumPointPerAxis { Value = _numPointsPerAxis });
            dstManager.AddComponentData(entity, new Voxel.WeightMultiplier { Value = _weightMultiplier });
            dstManager.AddComponentData(entity, new Voxel.ShaderParameters { Value = _shaderParams });
            
            ComponentTypes _chunkHolderComponents = new ComponentTypes
            (
                typeof(LinkedEntityGroup),
                typeof(DynamicBuffer.HeightMap),
                typeof(Chunks.MeshBuffer.Vertices),
                //typeof(Chunks.MeshBuffer.Uvs),
                typeof(Chunks.MeshBuffer.Triangles)
            );
            Entity ChunksHolder = dstManager.CreateEntity(typeof(Tag.ChunksHolder));
            dstManager.AddComponents(ChunksHolder, _chunkHolderComponents);
            dstManager.SetName(ChunksHolder, "ChunksHolder");

            //Create Event Holder with a the Event "MapSettingsConverted"
            Entity MapEventHolder = dstManager.CreateEntity(typeof(Tag.MapEventHolder),/*typeof(Event.CreationChunksEntityEvent),*/typeof(Event.HeightMapBigMapCalculEvent));
            dstManager.SetName(MapEventHolder, "MapEventHolder");
            
        }

        private void OnValidate()
        {
            //_chunkSize = _chunkSize > _mapSize || _chunkSize == 0 ? _mapSize : _chunkSize;
            _chunkSize = _chunkSize <= 0 || _chunkSize > 241 ? 241 : _chunkSize;
            _numChunk = math.max(1, _numChunk);

            _mapSize = (_chunkSize) * _numChunk;

            _seed = math.max(1, _seed);
            _octaves = math.max(1, _octaves);
            _scale = math.max(0.0001f, _scale);
            _lacunarity = math.max(1f, _lacunarity);
            _heightMultiplier = math.max(1f, _heightMultiplier);
        }
    }
}
