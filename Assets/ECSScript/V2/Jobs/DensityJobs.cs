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
        [ReadOnly] public float WeightMultiplierJob;
        [ReadOnly] public float NoiseWeightJob;
        [ReadOnly] public float NoiseMinValueJob;

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
            float pointValue = NoiseMap(pointPosition);
            pointValue = pointPosition.y + math.mul(pointValue, NoiseWeightJob) /*+ math.fmod(pointPosition.y, 1)*0*/;
            pointsJob[index] = new float4(pointPosition, pointValue);
        }

        float NoiseMap(float3 position)
        {
            float noiseHeight = 0;
            float frequency = ScaleJob / 100;
            float amplitude = 1;
            float weight = 1;
            for (int i = 0; i < OctavesJob; i++)
            {
                float3 PosFrequency = math.mad(position, (float3)frequency, OctOffsetArrayJob[i]);
                //1+math.abs(noise) => mul by 1.X
                //1-math.abs(noise) => mul by 0.X
                //float noise = 1 + math.abs(/*snoise(PosFrequency)*/math.mad(snoise(PosFrequency), 2,-1) );
                float noise = 1 - snoise(PosFrequency);
                //noise = math.mul(noise, noise);
                noise = math.mul(noise, weight);
                weight = math.max(math.min(math.mul(noise, WeightMultiplierJob), 1), 0);
                noiseHeight = math.mad(noise, amplitude, noiseHeight);
                amplitude = math.mul(amplitude, PersistanceJob);
                frequency = math.mul(frequency, LacunarityJob);
            }

            noiseHeight = math.max(0, noiseHeight - NoiseMinValueJob);
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
