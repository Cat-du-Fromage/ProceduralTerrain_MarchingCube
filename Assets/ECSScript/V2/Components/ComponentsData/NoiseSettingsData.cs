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
        public struct WeightMultiplier : IComponentData
        {
            public float Value;
        }
        public struct NoiseWeight : IComponentData
        {
            public float Value;
        }
        public struct NoiseMinValue : IComponentData
        {
            public float Value;
        }

        public struct IslandNoiseLayer : IComponentData
        {
            public int Octaves;
            public float Lacunarity;
            public float Persistance;
            public float Scale;
            public int Seed;
            public float WeightMultiplier;
            public float NoiseWeight;
            public float NoiseMinValue;
        }
    }
}
