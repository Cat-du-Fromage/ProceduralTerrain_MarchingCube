using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.V2.Data.Settings
{
    namespace Chunk
    {
        public struct ChunkBoundXZ : IComponentData
        {
            public int Value;
        }

        public struct ChunkBoundY : IComponentData
        {
            public int Value;
        }

        public struct NumChunk : IComponentData
        {
            public int Value;
        }
        
        public struct ChunkNumPointPerAxisXZ : IComponentData
        {
            public int Value;
        }
        // == ChunkNumPointPerAxisXZ
        public struct ChunkNumPointPerAxisY : IComponentData
        {
            public int Value;
        }
        //Same for XZ and Y all the time
        //ChunkBoundXZ / (ChunkNumPointPerAxisXZ-1)
        public struct PointSpacing : IComponentData
        {
            public float Value;
        }
    }

    namespace Map
    {
        //NumChunk * ChunkBoundXZ
        public struct MapBoundXZ : IComponentData
        {
            public int Value;
        }
        //ChunkBoundY (for now)
        public struct MapBoundY : IComponentData
        {
            public int Value;
        }
        //NumChunk * ChunkNumPointPerAxisXZ
        public struct MapNumPointPerAxisXZ : IComponentData
        {
            public int Value;
        }
        //ChunkNumPointPerAxisY
        public struct MapNumPointPerAxisY : IComponentData
        {
            public int Value;
        }

        public struct IsoSurface : IComponentData
        {
            public float Value;
        }
    }

    namespace Voxel
    {
        //ChunkBoundXZ - 1
        public struct ChunkNumVoxelPerAxisXZ : IComponentData
        {
            public int Value;
        }

        //ChunkBoundY - 1
        public struct ChunkNumVoxelPerAxisY : IComponentData
        {
            public int Value;
        }
        //ChunkBoundXZ*NumChunk - 1
        public struct MapNumVoxelPerAxisXZ : IComponentData
        {
            public int Value;
        }
        //== ChunkNumVoxelPerAxisY
        public struct MapNumVoxelPerAxisY : IComponentData
        {
            public int Value;
        }
    }
}
