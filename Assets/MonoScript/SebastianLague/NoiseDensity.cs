using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.noise;
using uRandom = Unity.Mathematics.Random;
/*
namespace KaizerWaldCode
{
    
    public static class NoiseDensity
    {
        
        // Noise settings
        //[SerializeField] float3 offsets;
        //[SerializeField] int octaves;
        //[SerializeField] float noiseScale;

        //[SerializeField] float noiseWeight;
        //[SerializeField] float floorOffset;
        //[SerializeField] float weightMultiplier;
        //[SerializeField] bool closeEdges;
        //[SerializeField] float hardFloor;
        //[SerializeField] float hardFloorWeight;
        //[SerializeField] float4 parameters;

        //float lacunarity = 2;
        //float persistence = 0.5f;
        
        public static float4[] NoiseMapGen(int size, NoiseSettings noiseSettings)
        {
            float4[] points = new float4[size * size * size];

            #region Random

            uRandom pRNG = new uRandom((uint)noiseSettings.seed);
            float3[] octaveOffsets = new float3[noiseSettings.octaves];
            for (int i = 0; i < noiseSettings.octaves; i++)
            {
                float offsetX = pRNG.NextFloat(-100000, 100000) + noiseSettings.offset.x;
                float offsetY = pRNG.NextFloat(-100000, 100000) + noiseSettings.offset.y;
                float offsetZ = pRNG.NextFloat(-100000, 100000) + noiseSettings.offset.z;
                octaveOffsets[i] = new float3(offsetX, offsetY, offsetZ);
            }

            #endregion Random

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {

                        float noise = 0;

                        float frequency = noiseSettings.scale / 100;
                        float amplitude = 1;
                        float weight = 1;
                        for (int j = 0; j < noiseSettings.octaves; j++)
                        {
                            float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseSettings.scale * frequency;
                            float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseSettings.scale * frequency;
                            float perlinValue = snoise(sampleX, sampleY) * 2 - 1;
                            //float n = snoise((pos + offsetNoise) * frequency + offsets[j] + offset);
                            float v = 1 - abs(n);
                            v = v * v;
                            v *= weight;
                            weight = max(min(v * weightMultiplier, 1), 0);
                            noise += v * amplitude;
                            amplitude *= noiseSettings.persistance;
                            frequency *= noiseSettings.lacunarity;
                        }
                        int index = indexFromCoord(x, y, z);
                        points[index] = new float4(pos, finalVal);
                    }
                }
            }

            return points;
        }

        //Get Index from a 3D vector
        static int indexFromCoord(int x, int y, int z, int size)
        {
            return ((z * size * size) + (y * size + x));
        }

        
    }

    //Settings
    [System.Serializable]
    public class NoiseSettings
    {

        public float scale = 50;

        public int octaves = 6;
        [Range(0, 1)] public float persistance = 0.5f;
        public float lacunarity = 2;

        public int seed;
        public float3 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}
    */
