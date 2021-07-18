using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.V3.Data.Settings
{
    public struct ChunkData : IComponentData
    {
        public int BoundsXZ;
        public int NumChunk;
        public int PointsPerAxisXZ; // ChunkData.BoundsXZ * VoxelData.pointPerMeter
        public int VoxelPerAxisXZ; // ChunkData.PointsPerAxisXZ - 1
    }

    public struct MapData : IComponentData
    {
        public int BoundsXZ; // ChunkData.NumChunk * ChunkData.BoundsXZ
        public int PointsPerAxisXZ; // ChunkData.NumChunk * ( ("ChunkData.PointsPerAxisXZ")ChunkData.BoundsXZ * VoxelData.pointPerMeter )
        public int VoxelPerAxisXZ; // (ChunkData.NumChunk * ChunkData.PointsPerAxisXZ) - 1
    }

    public struct HeightData : IComponentData
    {
        public int Height;
        public int PointsPerAxisY; // VoxelData.pointPerMeter * HeightData.Height
        public int VoxelPerAxisY; // HeightData.PointsPerAxisY - 1
    }

    public struct VoxelData : IComponentData
    {
        public int PointPerMeter; //Range 1-10
        public float PointSpacing; // 1/VoxelData.pointPerMeter
        public float IsoSurface;
    }
}