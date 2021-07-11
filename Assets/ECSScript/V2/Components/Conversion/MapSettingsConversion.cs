using Unity.Entities;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using ChunkSet = KaizerWaldCode.V2.Data.Settings.Chunk;
using MapSet = KaizerWaldCode.V2.Data.Settings.Map;
using VoxelSet = KaizerWaldCode.V2.Data.Settings.Voxel;
using NoiseSet = KaizerWaldCode.V2.Data.Settings.Noise;

namespace KaizerWaldCode.V2.Data.Conversion
{
    [DisallowMultipleComponent]
    public class MapSettingsConversion : MonoBehaviour, IConvertGameObjectToEntity
    {
        
        //Link To MonoBehaviour
        [SerializeField] private List<EntityStore> store;
        
        public enum ActionType
        {
            Normal,
            DebugPoints,
        };

        public ActionType Action;
        

        public float IsoSurface = 0.5f;
        public int ChunkBound = 10;
        public int NumChunk = 1;
        [Range(2, 100)] public int ChunkNumPointPerAxis = 10;

        [Header("Noise Settings")]
        public int Seed = 1;
        public int Octaves = 4;
        public float Lacunarity = 2;
        [Range(0, 1)] public float Persistance = 0.5f;
        public float ScaleNoise = 0.001f;
        public float NoiseWeight = 1.0f;
        public float WeightMultiplier = 0.0f;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            #region Check Values
            //Validate Field before assigning them
            ChunkBound = math.max(1, ChunkBound);
            NumChunk = math.max(1, NumChunk);

            //Noise Checker
            Seed = math.max(1, Seed);
            Octaves = math.max(1, Octaves);
            Lacunarity = math.max(1, Lacunarity);
            ScaleNoise = math.max(0.001f, ScaleNoise);
            #endregion Check Values

            #region Tagging
            //Add Tag and remove unecessary default component
            dstManager.AddComponent<V2.Data.Tag.MapSetting>(entity);
            dstManager.RemoveComponent<LocalToWorld>(entity);
            dstManager.RemoveComponent<Translation>(entity);
            dstManager.RemoveComponent<Rotation>(entity);
            #endregion Tagging

            #region Chunk Components
            dstManager.AddComponentData(entity, new ChunkSet.ChunkBoundXZ { Value = ChunkBound });
            dstManager.AddComponentData(entity, new ChunkSet.ChunkBoundY {Value = ChunkBound});
            dstManager.AddComponentData(entity, new ChunkSet.NumChunk { Value = NumChunk });
            dstManager.AddComponentData(entity, new ChunkSet.ChunkNumPointPerAxisXZ { Value = ChunkNumPointPerAxis });
            dstManager.AddComponentData(entity, new ChunkSet.ChunkNumPointPerAxisY { Value = ChunkNumPointPerAxis });

            dstManager.AddComponentData(entity, new ChunkSet.PointSpacing { Value = (float)ChunkBound / (float)(ChunkNumPointPerAxis-1) });
            #endregion Chunk Components

            #region Map Components
            dstManager.AddComponentData(entity, new MapSet.IsoSurface { Value = IsoSurface });
            dstManager.AddComponentData(entity, new MapSet.MapBoundXZ { Value = math.mul(ChunkBound, NumChunk) });
            dstManager.AddComponentData(entity, new MapSet.MapBoundY { Value = ChunkBound });
            dstManager.AddComponentData(entity, new MapSet.MapNumPointPerAxisXZ { Value = math.mul(ChunkNumPointPerAxis, NumChunk) });
            dstManager.AddComponentData(entity, new MapSet.MapNumPointPerAxisY { Value = ChunkNumPointPerAxis });
            #endregion Map Components

            #region Voxel Components
            //Chunk
            dstManager.AddComponentData(entity, new VoxelSet.ChunkNumVoxelPerAxisXZ { Value = ChunkNumPointPerAxis - 1 });
            dstManager.AddComponentData(entity, new VoxelSet.ChunkNumVoxelPerAxisY { Value = ChunkNumPointPerAxis - 1 });
            //Map
            dstManager.AddComponentData(entity, new VoxelSet.MapNumVoxelPerAxisXZ { Value = math.mad(ChunkNumPointPerAxis, NumChunk, -1) });
            dstManager.AddComponentData(entity, new VoxelSet.MapNumVoxelPerAxisY { Value = ChunkNumPointPerAxis - 1 });
            #endregion Voxel Components

            #region Noise Components
            dstManager.AddComponentData(entity, new NoiseSet.Octaves { Value = Octaves });
            dstManager.AddComponentData(entity, new NoiseSet.Lacunarity { Value = Lacunarity });
            dstManager.AddComponentData(entity, new NoiseSet.Persistance { Value = Persistance });
            dstManager.AddComponentData(entity, new NoiseSet.Scale { Value = ScaleNoise });
            dstManager.AddComponentData(entity, new NoiseSet.Seed { Value = Seed });
            dstManager.AddComponentData(entity, new NoiseSet.WeightMultiplier { Value = WeightMultiplier });
            dstManager.AddComponentData(entity, new NoiseSet.NoiseWeight { Value = NoiseWeight });
            #endregion Noise Components

            #region Create Event Holder
            Entity MapEventHolder = dstManager.CreateEntity(typeof(Tag.MapEventHolder));
            dstManager.SetName(MapEventHolder, "MapEventHolder");
            dstManager.AddComponent<Event.Event_ProcessPointsPosition>(MapEventHolder);
            switch (Action)
            {
                case ActionType.Normal:
                    break;
                case ActionType.DebugPoints:
                    dstManager.AddComponent<Event.Event_DebuggingPoints>(MapEventHolder);
                    break;
                default:
                    break;
            }
            #endregion Create Event Holder

            #region Create Chunk Holder
            ComponentTypes chunkHolderComponents = new ComponentTypes
            (
                typeof(LinkedEntityGroup),
                typeof(ChunksData.DynamicBuffer.PointsBuffer),
                typeof(ChunksData.DynamicBuffer.Vertices),
                typeof(ChunksData.DynamicBuffer.Triangles)
            );

            Entity ChunksHolder = dstManager.CreateEntity(typeof(Tag.ChunksHolder));
            dstManager.AddComponents(ChunksHolder, chunkHolderComponents);
            dstManager.SetName(ChunksHolder, "ChunksHolder");

            #endregion Create Chunk Holder
            //Link To MonoBehaviour
            /*
            EntityStore entityMapSett = new EntityStore();
            entityMapSett.MapSetting = entity;
            //store.Add(entityMapSett);
            //Link To MonoBehaviour
            EntityStore entityEvent = new EntityStore();
            entityEvent.EventHolder = ChunksHolder;
            */
            //store.Add(entityEvent);
            
            var numStores = store.Count;
            for (var i = 0; i < numStores; i++)
            {
                store[i].entity = entity;
                store[i].EventHolder = MapEventHolder;
            }
            
        }
    }
}
