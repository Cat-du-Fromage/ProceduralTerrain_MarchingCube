using System;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace KaizerWaldCode.V3.Jobs
{
    public struct ChunkSliceJob : IJobParallelFor
    {
        [ReadOnly] public int NumChunkJob;
        [ReadOnly] public int MapBoundXZJob;
        [ReadOnly] public int MapBoundYJob;
        [ReadOnly] public int MapPointPerAxisXZJob;
        [ReadOnly] public int MapPointPerAxisYJob;
        [ReadOnly] public float SpacingJob;

        [ReadOnly] public int VoxelIndex;

        [ReadOnly] public float ScaleJob;
        public NativeArray<float4> pointsJob;
        [WriteOnly]public NativeArray<float4> ChunkPointsJob;
        //Index represente the number of slice (y*z) or numRowY * numRowZ or PointPerAxisXZJob*PointPerAxisYJob
        public void Execute(int index)
        {
            int realStart = index * VoxelIndex;

            int x = (int)math.fmod(index, MapPointPerAxisXZJob);
            int y = ((int)math.fmod(index, math.mul(MapPointPerAxisXZJob, MapPointPerAxisYJob)) / MapPointPerAxisXZJob); // need to test without floor
            int z = (int)math.floor(index / math.mul(MapPointPerAxisXZJob, MapPointPerAxisYJob));

            float3 trueOrigin = new float3(math.mul(SpacingJob, NumChunkJob) / 2, math.mul(SpacingJob, NumChunkJob) / 2, math.mul(SpacingJob, NumChunkJob) / 2);

            float3 gridCenter = new float3(MapBoundXZJob / 2f, MapBoundYJob / 2f, MapBoundXZJob / 2f);
            float3 mapCenter = new float3(MapPointPerAxisXZJob / 2f, MapPointPerAxisXZJob / 2f, MapPointPerAxisXZJob / 2f);
            float3 pointPosition = pointsJob[index].xyz;

            NativeArray<float4> slicedArray = new NativeArray<float4>(12, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            slicedArray = pointsJob.GetSubArray(0, 1); // contains the value we want at the given index
            //ChunkPointsJob.Append(new float4(0,0,0,0)); //apend possible?

            // source - beginIndexSource - destination - BeginDestination - LengthOfCopy
            NativeArray<float4>.Copy(slicedArray, 0, ChunkPointsJob, 10, slicedArray.Length);
            slicedArray.Dispose();

            //we search for points not distances like heightMap
            //float zPos = 
        }
    }
}
