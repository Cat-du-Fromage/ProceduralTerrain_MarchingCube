using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.noise;
using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Random = Unity.Mathematics.Random;

namespace KaizerWaldCode.V2.Jobs
{
    [BurstCompile(CompileSynchronously = true)]
    public struct IslandDentityJob : IJobParallelFor
    {
        [ReadOnly] public int FallOffJob;
        [ReadOnly] public float IsoSurfaceJob;
        [ReadOnly] public int NumChunkJob;
        [ReadOnly] public int randJob;
        [ReadOnly] public int MapBoundXZJob;
        [ReadOnly] public int MapBoundYJob;
        [ReadOnly] public int MapNumPointPerAxisXZJob;
        [ReadOnly] public int MapNumPointPerAxisYJob;
        [ReadOnly] public float SpacingJob;

        [ReadOnly] public float ScaleJob;
        public NativeArray<float4> pointsJob;
        public void Execute(int index)
        {
            if (pointsJob[index].w < IsoSurfaceJob) return;

            //Get all Points
            int x = (int)math.fmod(index, MapNumPointPerAxisXZJob);
            int y = /*(int)math.floor*/((int)math.fmod(index, math.mul(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob)) / MapNumPointPerAxisXZJob); // need to test without floor
            int z = (int)math.floor(index / math.mul(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob));
            float3 gridCenter = new float3(MapBoundXZJob / 2f, MapBoundYJob / 2f, MapBoundXZJob / 2f);
            float3 mapCenter = new float3(MapNumPointPerAxisXZJob / 2f, MapNumPointPerAxisXZJob / 2f, MapNumPointPerAxisXZJob / 2f);
            float3 pointPosition = pointsJob[index].xyz;

            float2 trueOrigin = new float2(math.mul(SpacingJob, NumChunkJob) / 2, math.mul(SpacingJob, NumChunkJob) / 2);
            
            float sampleX = (pointPosition.x / MapNumPointPerAxisXZJob * ScaleJob); //real or grid position?
            float sampleZ = (pointPosition.z / MapNumPointPerAxisXZJob * ScaleJob);

            float noiseHeight = NoiseMapIsland(pointPosition) - (math.lengthsq(pointPosition.xz - new float2(0 + (SpacingJob * NumChunkJob) / 2, 0 + (SpacingJob * NumChunkJob) / 2)) / math.mul(FallOffJob/2, FallOffJob/2));
            //noiseHeight = 1 + noiseHeight;
            //float noiseHeight = NoiseMapIsland(pointPosition);
            //noiseHeight = NoiseMapIsland(pointPosition);
            noiseHeight = (1 - math.abs(NoiseMapIsland(pointPosition)));
            noiseHeight *= noiseHeight;
            //noiseHeight = math.abs(noiseHeight);
            //noiseHeight = math.mul(pointsJob[index].w, noiseHeight);

            noiseHeight = pointsJob[index].w - noiseHeight*2;
            //CAREFUL add spacing * num chunk to center map / 2

            if (    math.length(pointPosition.xz - new float2(0 + trueOrigin.x, 0 + trueOrigin.y)) <= MapBoundXZJob/2 && pointsJob[index].w >= IsoSurfaceJob)
            {
                pointsJob[index] = new float4(pointsJob[index].xyz, noiseHeight);
            }
            else
            {
                pointsJob[index] = new float4(pointsJob[index].xyz, pointsJob[index].w);
            }
            //Noise Based on XZ value Only
            //Minus Fall off Distance
            //CAN get index from coord of the point!
            // => 2 seperate array 1 with all and second with only the island(not now)
        }

        float NoiseMapIsland(float3 position)
        {
            float noiseHeight = 0;
            float frequency = 1;
            float amplitude = 1;
            for (int i = 0; i < 6; i++)
            {

                float sampleX = math.mul((position.x - (SpacingJob * NumChunkJob) / 2) / ScaleJob, frequency);

                float sampleY = math.mul((position.y - (SpacingJob * NumChunkJob) / 2) / ScaleJob, frequency);

                float sampleZ = math.mul((position.z - (SpacingJob * NumChunkJob) / 2) / ScaleJob, frequency);
                //float2 sampleXY = new float2(sampleX, sampleZ);
                float3 sampleXYZ = new float3(sampleX, sampleY, sampleZ);
                float pNoiseValue = snoise(sampleXYZ);
                noiseHeight = math.mad(pNoiseValue, amplitude, noiseHeight);
                amplitude = math.mul(amplitude, 0.5f);
                frequency = math.mul(frequency, 2);
            }
            return noiseHeight;
        }
    }
}
