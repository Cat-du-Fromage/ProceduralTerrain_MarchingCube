using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.V2.Data.ChunksData
{
    namespace DynamicBuffer
    {
        public struct PointsBuffer : IBufferElementData
        {
            public float4 Value;
        }

        public struct Vertices : IBufferElementData
        {
            public float3 Value;
        }

        public struct Triangles : IBufferElementData
        {
            public int Value;
        }
    }
}
