using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.noise;


namespace KaizerWaldCode.Jobs
{

        /// <summary>
        /// Process RandomJob
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        public struct NoiseRandomJob : IJobParallelFor
        {
            [ReadOnly] public Unity.Mathematics.Random RandomJob;
            [ReadOnly] public float3 OffsetJob;
            [WriteOnly] public NativeArray<float3> OctOffsetArrayJob;

            public void Execute(int index)
            {
                
                float _offsetX = RandomJob.NextFloat(-100000.0f, 100000.0f) + OffsetJob.x;
                float _offsetY = RandomJob.NextFloat(-100000.0f, 100000.0f) - OffsetJob.y;
                float _offsetZ = RandomJob.NextFloat(-100000.0f, 100000.0f) + OffsetJob.z;
                OctOffsetArrayJob[index] = new float3(_offsetX, _offsetY, _offsetZ);
                
                //OctOffsetArrayJob[index] = new float3((float)RandomJob.NextDouble() * 2 - 1, (float)RandomJob.NextDouble() * 2 - 1, (float)RandomJob.NextDouble() * 2 - 1);
            }
        }

        /// <summary>
        /// Noise Height
        /// </summary>
        [BurstCompile(CompileSynchronously = true)]
        public struct NoiseDensityJob : IJobParallelFor
        {
            //Noise
            [ReadOnly] public int MapSizeJob;
            [ReadOnly] public float3 FullMapSizeJob; //calcul outside
            [ReadOnly] public float3 MapCenterJob; //calcul outside
            [ReadOnly] public int OctavesJob;
            [ReadOnly] public float LacunarityJob;
            [ReadOnly] public float PersistanceJob;
            [ReadOnly] public float ScaleJob;
            [DeallocateOnJobCompletion][ReadOnly] public NativeArray<float3> OctOffsetArray;

            //Voxel
            [ReadOnly] public float NoiseWeightJob;
            [ReadOnly] public int NumPointPerAxisJob;
            [ReadOnly] public float SpacingJob; //calculated outside
            [ReadOnly] public float FloorOffsetJob;
            [ReadOnly] public float HardFloorJob;
            [ReadOnly] public float HardFloorWeightJob;
            [ReadOnly] public float WeightMultiplierJob;
            [ReadOnly] public float4 ShaderParametersJob; //What is this?

            [NativeDisableParallelForRestriction][WriteOnly] public NativeArray<float4> NoiseMap;

            public void Execute(int index)
            {
            /*
            int y = (int)math.floor(index / MapSizeJob);
            int x = index - math.mul(y, MapSizeJob);
            int z = (int)math.floor(index / math.mul(MapSizeJob,MapSizeJob));
            */
            int x = (int)math.fmod(index, MapSizeJob);
            int y = (int)math.fmod(index,math.mul(MapSizeJob, MapSizeJob))/ MapSizeJob;
            int z = (int)math.floor(index/math.mul(MapSizeJob, MapSizeJob));
            float3 positionByIndex = new float3(x, y, z);

                float3 vetrexPosition = math.mad(positionByIndex, SpacingJob, MapCenterJob) - (MapSizeJob / 2f); // may have to check the result
                float noiseHeight = 0;

                float frequency = ScaleJob / 100; //Why /100?

                //float frequency = 1;
                float amplitude = 1;
                float weight = 1;

                for (int i = 0; i < OctavesJob; i++)
                {
                /*
                float sampleX = math.mul((x - (MapSizeJob / 2f) + OctOffsetArray[i].x)/ ScaleJob , frequency);
                float sampleY = math.mul((y - (MapSizeJob / 2f) + OctOffsetArray[i].y)/ ScaleJob , frequency);
                float sampleZ = math.mul((z - (MapSizeJob / 2f) + OctOffsetArray[i].z)/ ScaleJob , frequency);
                float3 sample = new float3(sampleX, sampleY, sampleZ);

                float n = snoise(sample);
                */
                //float n = snoise((vetrexPosition/*+ (float)offsetNoise*/) * frequency + OctOffsetArray[i]);
                    float n = snoise(math.mad(vetrexPosition, frequency, OctOffsetArray[i]));
                    float v = 1 - math.abs(n);
                    v = v * v;
                    v *= weight;
                    weight = math.max(math.min(math.mul(v, WeightMultiplierJob), 1), 0);

                    noiseHeight = math.mad(v, amplitude, noiseHeight);
                    amplitude = math.mul(amplitude, PersistanceJob);
                    frequency = math.mul(frequency, LacunarityJob);
                }

            //float finalValue = -(vetrexPosition.y + FloorOffsetJob) + noiseHeight * NoiseWeightJob + (vetrexPosition.y % ShaderParametersJob.x) * ShaderParametersJob.y;
            float finalValue = -(vetrexPosition.y + FloorOffsetJob) + math.mul(noiseHeight, NoiseWeightJob) + math.mul(math.fmod(vetrexPosition.y, ShaderParametersJob.x), ShaderParametersJob.y);

                if (vetrexPosition.y < HardFloorJob)
                {
                    finalValue += HardFloorWeightJob;
                }

                NoiseMap[index] = new float4(vetrexPosition, finalValue);
            }
        }
}
