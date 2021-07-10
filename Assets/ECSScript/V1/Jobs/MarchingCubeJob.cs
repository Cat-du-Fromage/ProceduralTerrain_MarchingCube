using KaizerWaldCode.System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace KaizerWaldCode.Jobs
{
    [BurstCompile(CompileSynchronously = true)]
    public struct MarchingCubeJob : IJobParallelFor
    {
        [ReadOnly] public int MapSizeJob;
        [ReadOnly] public int numPointsPerAxisJob;
        [ReadOnly] public float isoLevelJob;

        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<int> cornerIndexAFromEdgeJob;
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<int> cornerIndexBFromEdgeJob;
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<int> triangulationJob;
        [NativeDisableParallelForRestriction]
        public NativeList<Triangle> trianglesListJob;



        [NativeDisableParallelForRestriction]
        public NativeArray<float4> pointsJob;
        //[NativeDisableParallelForRestriction]
        //public NativeArray<int> trianglesJob;
        //[NativeDisableParallelForRestriction]
        //private NativeArray<float4> _cubeCorners;
        public void Execute(int index)
        {
            int y = (int)math.floor(index / MapSizeJob);
            int x = index - math.mul(y, MapSizeJob);
            int z = (int)math.floor(index / math.mul(MapSizeJob, MapSizeJob));

            //8 corner of the current Cube
            NativeArray<float4> _cubeCorners = new NativeArray<float4>(8, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            _cubeCorners[0] = pointsJob[indexFromCoord(x, y, z, numPointsPerAxisJob)];
            _cubeCorners[1] = pointsJob[indexFromCoord(x+1, y, z, numPointsPerAxisJob)];
            _cubeCorners[2] = pointsJob[indexFromCoord(x+1, y, z+1, numPointsPerAxisJob)];
            _cubeCorners[3] = pointsJob[indexFromCoord(x, y, z+1, numPointsPerAxisJob)];
            _cubeCorners[4] = pointsJob[indexFromCoord(x, y+1, z, numPointsPerAxisJob)];
            _cubeCorners[5] = pointsJob[indexFromCoord(x+1, y+1, z, numPointsPerAxisJob)];
            _cubeCorners[6] = pointsJob[indexFromCoord(x+1, y+1, z+1, numPointsPerAxisJob)];
            _cubeCorners[7] = pointsJob[indexFromCoord(x, y+1, z+1, numPointsPerAxisJob)];

            // Calculate unique index for each cube configuration.
            // There are 256 possible values
            // A value of 0 means cube is entirely inside surface; 255 entirely outside.
            // The value is used to look up the edge table, which indicates which edges of the cube are cut by the isosurface.
            int cubeIndex = 0;
            if (_cubeCorners[0].w < isoLevelJob) cubeIndex |= 1;
            if (_cubeCorners[1].w < isoLevelJob) cubeIndex |= 2;
            if (_cubeCorners[2].w < isoLevelJob) cubeIndex |= 4;
            if (_cubeCorners[3].w < isoLevelJob) cubeIndex |= 8;
            if (_cubeCorners[4].w < isoLevelJob) cubeIndex |= 16;
            if (_cubeCorners[5].w < isoLevelJob) cubeIndex |= 32;
            if (_cubeCorners[6].w < isoLevelJob) cubeIndex |= 64;
            if (_cubeCorners[7].w < isoLevelJob) cubeIndex |= 128;

            // Create triangles for current cube configuration
            for (int i = 0; triangulationJob[math.mad(16, cubeIndex, i)] != -1; i += 3)
            {
                // Get indices of corner points A and B for each of the three edges
                // of the cube that need to be joined to form the triangle.
                int a0 = cornerIndexAFromEdgeJob[triangulationJob[math.mad(16, cubeIndex, i)]];
                int b0 = cornerIndexBFromEdgeJob[triangulationJob[math.mad(16, cubeIndex, i)]];

                int a1 = cornerIndexAFromEdgeJob[triangulationJob[math.mad(16, cubeIndex, i + 1)]];
                int b1 = cornerIndexBFromEdgeJob[triangulationJob[math.mad(16, cubeIndex, i + 1)]];

                int a2 = cornerIndexAFromEdgeJob[triangulationJob[math.mad(16, cubeIndex, i + 2)]];
                int b2 = cornerIndexBFromEdgeJob[triangulationJob[math.mad(16, cubeIndex, i + 2)]];

                Triangle tri;
                tri.vertexA = interpolateVerts(_cubeCorners[a0], _cubeCorners[b0], isoLevelJob);
                tri.vertexB = interpolateVerts(_cubeCorners[a1], _cubeCorners[b1], isoLevelJob);
                tri.vertexC = interpolateVerts(_cubeCorners[a2], _cubeCorners[b2], isoLevelJob);
                trianglesListJob.Add(tri);
                //trianglesJob[math.mul(index+i,6)] =  
            }

            _cubeCorners.Dispose();
        }

        float3 interpolateVerts(float4 v1, float4 v2, float isoLevel)
        {
            float t = (isoLevel - v1.w) / (v2.w - v1.w);
            return v1.xyz + t * (v2.xyz - v1.xyz);
        }

        int indexFromCoord(int x, int y, int z, int numPointsPerAxis)
        {
            return z * numPointsPerAxis * numPointsPerAxis + y * numPointsPerAxis + x;
        }
    }
}
