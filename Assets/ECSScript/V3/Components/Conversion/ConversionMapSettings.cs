using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MapSet = KaizerWaldCode.V3.Data.Settings;

namespace KaizerWaldCode.V3.Data.Conversion
{
    [DisallowMultipleComponent]
    public class ConversionMapSettings : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float IsoSurface = 0.5f;
        public int ChunkBoundSizeXZ = 10;
        public int MapHeight = 20;
        public int NumberOfChunk = 2;
        [Range(1, 10)] public int PointPerMeter = 10;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            #region Values Validation
            ChunkBoundSizeXZ = math.max(1, ChunkBoundSizeXZ);
            MapHeight = math.max(1, MapHeight);
            NumberOfChunk = math.max(1, NumberOfChunk);
            #endregion Values Validation

            #region Tagging
            //Add Tag and remove unecessary default component
            dstManager.AddComponent<Tag.MapSetting>(entity);
            dstManager.RemoveComponent<LocalToWorld>(entity);
            dstManager.RemoveComponent<Translation>(entity);
            dstManager.RemoveComponent<Rotation>(entity);
            #endregion Tagging

            dstManager.AddComponentData(entity, new MapSet.VoxelData()
            {
                PointPerMeter = PointPerMeter,
                PointSpacing = 1.0f / PointPerMeter,
                IsoSurface = IsoSurface,
            });

            dstManager.AddComponentData(entity, new MapSet.HeightData()
            {
                Height = MapHeight,
                PointsPerAxisY = PointPerMeter * PointPerMeter,
                VoxelPerAxisY = (MapHeight * PointPerMeter) - 1,
            });

            dstManager.AddComponentData(entity, new MapSet.ChunkData()
            {
                BoundsXZ = ChunkBoundSizeXZ,
                NumChunk = NumberOfChunk,
                PointsPerAxisXZ = ChunkBoundSizeXZ * PointPerMeter,
                VoxelPerAxisXZ = (ChunkBoundSizeXZ * PointPerMeter) - 1,
            });

            dstManager.AddComponentData(entity, new MapSet.MapData()
            {
                BoundsXZ = ChunkBoundSizeXZ * NumberOfChunk,
                PointsPerAxisXZ = NumberOfChunk * (ChunkBoundSizeXZ * PointPerMeter),
                VoxelPerAxisXZ = (NumberOfChunk * (ChunkBoundSizeXZ * PointPerMeter)) - 1,
            });

            #region Create Chunk Holder
            ComponentTypes chunkHolderComponents = new ComponentTypes
            (
                typeof(LinkedEntityGroup)
                /*
                typeof(ChunksData.DynamicBuffer.PointsBuffer),
                typeof(ChunksData.DynamicBuffer.Vertices),
                typeof(ChunksData.DynamicBuffer.Triangles)
                */
            );

            Entity ChunksHolder = dstManager.CreateEntity(typeof(Tag.ChunksHolder));
            dstManager.AddComponents(ChunksHolder, chunkHolderComponents);
            dstManager.SetName(ChunksHolder, "ChunksHolder");
            #endregion Create Chunk Holder

            
            #region Create Event Holder
            Entity MapEventHolder = dstManager.CreateEntity(typeof(Tag.MapEventHolder));
            dstManager.SetName(MapEventHolder, "MapEventHolder");
            dstManager.AddComponent<Event.Event_ProcessPointsPosition>(MapEventHolder);
            #endregion Create Event Holder
            

        }
    }
}
