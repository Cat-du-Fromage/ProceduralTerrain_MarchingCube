using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.V2.Data.Settings
{
    namespace Noise
    {
        public struct Octaves : IComponentData
        {
            public int Value;
        }

        public struct Lacunarity : IComponentData
        {
            public float Value;
        }

        public struct Persistance : IComponentData
        {
            public float Value;
        }

        public struct Scale : IComponentData
        {
            public float Value;
        }

        public struct Seed : IComponentData
        {
            public int Value;
        }
    }
}
