using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.noise;
using Unity.Collections;
using Unity.Entities;
using Unity.Burst;

namespace KaizerWaldCode.V2.Jobs
{
    [BurstCompile(CompileSynchronously = true)]
    public struct PointsJob : IJobParallelFor
    {
        //Map/Voxel Settings
        [ReadOnly] public int MapBoundXZJob;
        [ReadOnly] public int MapBoundYJob;
        
        [ReadOnly] public int MapNumPointPerAxisXZJob;
        [ReadOnly] public int MapNumPointPerAxisYJob;
        [ReadOnly] public float SpacingJob;

        //Noise Job
        [ReadOnly] public int OctavesJob;
        [ReadOnly] public float LacunarityJob;
        [ReadOnly] public float PersistanceJob;
        [ReadOnly] public float ScaleJob;

        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<float3> OctOffsetArrayJob;
        [WriteOnly]public NativeArray<float4> pointsJob;
        public void Execute(int index)
        {
            int x = (int)math.fmod(index, MapNumPointPerAxisXZJob);
            int y = (int)math.floor(math.fmod(index, math.mul(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob)) / MapNumPointPerAxisXZJob); // need to test without floor
            int z = (int)math.floor(index/math.mul(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob));
            float3 pointPosition = math.mad( new float3(x, y, z), new float3(SpacingJob, SpacingJob, SpacingJob), -(new float3(MapBoundXZJob / 2f, MapBoundYJob / 2f, MapBoundXZJob / 2f)) );
            //value depending on height value in the world
            //float pointValue = pointPosition.y < isoSurfaceJob ? 0 : 1;
            float pointValue = NoiseMap(OctavesJob, LacunarityJob, PersistanceJob, ScaleJob, pointPosition, OctOffsetArrayJob);
            pointValue = (pointPosition.y + 0) + pointValue + math.fmod(pointPosition.y, 1)*0;
            pointsJob[index] = new float4(pointPosition, pointValue);
        }

        float NoiseMap(int octaves, float lacunarity, float persistance, float scale, float3 position, NativeArray<float3> OctOffsets)
        {
            float noiseHeight = 0;
            float frequency = scale / 100;
            float amplitude = 1;
            for (int i = 0; i < octaves; i++)
            {
                float3 PosFrequency = math.mad(position, (float3)frequency, OctOffsets[i]);
                float noise = 1 - math.abs(/*snoise(PosFrequency)*/math.mad(snoise(PosFrequency), 2,-1) );
                //float noise = snoise(math.mad(snoise(PosFrequency), (float3)2,-1));

                noiseHeight = math.mad(noise, amplitude, noiseHeight);
                amplitude = math.mul(amplitude, persistance);
                frequency = math.mul(frequency, lacunarity);
            }

            return noiseHeight;
        }
    }

    /// <summary>
    /// Process RandomJob
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    public struct NoiseRandomJob : IJobParallelFor
    {
        [ReadOnly] public Unity.Mathematics.Random RandomJob;
        //[ReadOnly] public float3 OffsetJob;
        [WriteOnly] public NativeArray<float3> OctOffsetArrayJob;

        public void Execute(int index)
        {

            float _offsetX = RandomJob.NextFloat(-100000.0f, 100000.0f);
            float _offsetY = RandomJob.NextFloat(-100000.0f, 100000.0f);
            float _offsetZ = RandomJob.NextFloat(-100000.0f, 100000.0f);
            OctOffsetArrayJob[index] = new float3(_offsetX, _offsetY, _offsetZ);

            //OctOffsetArrayJob[index] = new float3((float)RandomJob.NextDouble() * 2 - 1, (float)RandomJob.NextDouble() * 2 - 1, (float)RandomJob.NextDouble() * 2 - 1);
        }
    }
}
