using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Animation;
using UnityEngine;
using AnimationCurve = Unity.Animation.AnimationCurve;

namespace KaizerWaldCode.Data
{
    public struct MapGenerationData : IComponentData { }

    namespace MapSettings
    {
        public struct MapSize : IComponentData
        {
            public int Value;
        }
        public struct ChunkSize : IComponentData
        {
            public int Value;
        }
        public struct NumChunk : IComponentData
        {
            public int Value;
        }
        public struct Seed : IComponentData
        {
            public int Value;
        }
        public struct Octaves : IComponentData
        {
            public int Value;
        }
        public struct Scale : IComponentData
        {
            public float Value;
        }
        public struct Lacunarity : IComponentData
        {
            public float Value;
        }
        public struct Persistance : IComponentData
        {
            public float Value;
        }
        public struct Offset : IComponentData
        {
            public float3 Value;
        }

        public struct HeightMultiplier : IComponentData
        {
            public float Value;
        }
        public struct LevelOfDetail : IComponentData
        {
            public int Value;
        }

        public struct HeightCurve : IComponentData
        {
            public AnimationCurve Value;
        }
    }

    namespace Voxel
    {
        public struct NumPointPerAxis : IComponentData
        {
            public int Value;
        }

        public struct IsoLevel : IComponentData
        {
            public float Value;
        }

        public struct PointSpacing : IComponentData
        {
            public float Value;
        }
        public struct NoiseWeight : IComponentData
        {
            public float Value;
        }
        public struct FloorOffset : IComponentData
        {
            public float Value;
        }
        public struct HardFloor : IComponentData
        {
            public float Value;
        }
        public struct HardFloorWeight : IComponentData
        {
            public float Value;
        }
        public struct WeightMultiplier : IComponentData
        {
            public float Value;
        }
        public struct ShaderParameters : IComponentData
        {
            public float4 Value;
        }
    }

    namespace DynamicBuffer
    {
        public struct HeightMap : IBufferElementData
        {
            public float4 Value;
        }

        public struct ColorMap : IBufferElementData
        {
            public MaterialColor Value;
        }

        public struct Regions : IBufferElementData
        {
            public float Height;
            public MaterialColor Color;
        }
    }
}
