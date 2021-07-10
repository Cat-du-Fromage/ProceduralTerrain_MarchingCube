using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
/*
namespace KaizerWaldCode.V2.Jobs
{
    public struct Triangle
    {
        public float3 A;
        public float3 B;
        public float3 C;
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct MarchingCubeJobs : IJobParallelFor
    {
        [ReadOnly] public int MapBoundXZJob;
        [ReadOnly] public int MapBoundYJob;

        [ReadOnly] public int MapNumPointPerAxisXZJob;
        [ReadOnly] public int MapNumPointPerAxisYJob;
        [ReadOnly] public float SpacingJob;
        [ReadOnly] public float IsoLevelJob;

        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<int> cornerIndexAFromEdgeJob;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<int> cornerIndexBFromEdgeJob;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<int> triangulationJob;

        [ReadOnly] public NativeArray<float4> pointsJob;
        [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<int> TrianglesJob;
        [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<float3> VerticesJob;
        public void Execute(int index)
        {
            int x = (int)math.fmod(index, MapNumPointPerAxisXZJob);
            int y = (int)math.floor(math.fmod(index, math.mul(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob)) / MapNumPointPerAxisXZJob); // need to test without floor
            int z = (int)math.floor(index / math.mul(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob));
            float3 pointPosition = math.mad(new float3(x, y, z), new float3(SpacingJob, SpacingJob, SpacingJob), -(new float3(MapBoundXZJob / 2f, MapBoundYJob / 2f, MapBoundXZJob / 2f)));

            if (x == MapNumPointPerAxisXZJob || y == MapNumPointPerAxisYJob || z == MapNumPointPerAxisXZJob)
                return;
            NativeArray<float4> cubeCorners = new NativeArray<float4>(8, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            // (x,y,z) / (x+1,y,z)          : index -> index+1 
            // (x,y,z+1) / (x+1,y,z+1)      : index + (numPointXZ*numPointY) -> index + ( (numPointXZ*numPointY)+1 )
            // (x,y+1,z) / (x+1,y+1,z)      : index + (numPointXZ) -> index + (numPointXZ+1)
            // (x+1,y+1,z+1) / (x,y+1,z+1)  : index + ( (numPointXZ*numPointY)+numPointXZ + 1 ) -> index + ( (numPointXZ*numPointY)+numPointXZ)
            
            cubeCorners[0] = pointsJob[index]; //(x,y,z)
            cubeCorners[1] = pointsJob[index + 1]; //(x+1,y,z) 
            cubeCorners[2] = pointsJob[math.mad(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob, index + 1)]; //(x+1,y,z+1)
            cubeCorners[3] = pointsJob[math.mad(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob, index)]; //(x,y,z+1)
            cubeCorners[4] = pointsJob[index + MapNumPointPerAxisXZJob];//(x,y+1,z)
            cubeCorners[5] = pointsJob[(index + 1) + MapNumPointPerAxisXZJob]; //(x+1,y+1,z)
            cubeCorners[6] = pointsJob[(index + 1) + math.mad(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob, MapNumPointPerAxisXZJob)];//(x+1,y+1,z+1)
            cubeCorners[7] = pointsJob[index + math.mad(MapNumPointPerAxisXZJob, MapNumPointPerAxisYJob, MapNumPointPerAxisXZJob)];//(x,y+1,z+1)
            
            int cubeConfiguration = 0;
            
            for (int i = 0; i < 8; i++)
            {
                //00000000 add ( |=(bitwise OR) ) 1 at i (1 << i) when corner is below surface
                if (cubeCorners[i].w < IsoLevelJob)
                {
                    cubeConfiguration |= (1 << i);
                }
            }

            int triangleIndex = math.mul(math.mul(5, 3), index);
            for (int i = 0; triangulationJob[math.mad(16, cubeConfiguration, i)] != -1; i += 3)
            {
                // Get indices of corner points A and B for each of the three edges
                // of the cube that need to be joined to form the triangle.
                int a0 = cornerIndexAFromEdgeJob[triangulationJob[math.mad(16, cubeConfiguration, i)]];
                int b0 = cornerIndexBFromEdgeJob[triangulationJob[math.mad(16, cubeConfiguration, i)]];

                int a1 = cornerIndexAFromEdgeJob[triangulationJob[math.mad(16, cubeConfiguration, i + 1)]];
                int b1 = cornerIndexBFromEdgeJob[triangulationJob[math.mad(16, cubeConfiguration, i + 1)]];

                int a2 = cornerIndexAFromEdgeJob[triangulationJob[math.mad(16, cubeConfiguration, i + 2)]];
                int b2 = cornerIndexBFromEdgeJob[triangulationJob[math.mad(16, cubeConfiguration, i + 2)]];

                //int4 tranglesVertex = new int4(index, index + MapNumPointPerAxisXZJob + 1, index + MapNumPointPerAxisYJob, index + 1);

                //float3 Vert1 = interpolateVerts(cubeCorners[a0], cubeCorners[b0]);
                //float3 Vert2 = interpolateVerts(cubeCorners[a1], cubeCorners[b1]);
                //float3 Vert3 = interpolateVerts(cubeCorners[a2], cubeCorners[b2]);

                float3 Vert1 = (cubeCorners[a0].xyz + cubeCorners[b0].xyz) / 2;
                float3 Vert2 = (cubeCorners[a1].xyz + cubeCorners[b1].xyz) / 2;
                float3 Vert3 = (cubeCorners[a2].xyz + cubeCorners[b2].xyz) / 2;

                VerticesJob[triangleIndex + i] = Vert1;
                VerticesJob[triangleIndex + i + 1] = Vert2;
                VerticesJob[triangleIndex + i + 2] = Vert3;

                TrianglesJob[triangleIndex + i] = triangleIndex + i;
                TrianglesJob[triangleIndex + i + 1] = triangleIndex + i + 1;
                TrianglesJob[triangleIndex + i + 2] = triangleIndex + i + 2;
                //triangleIndex += 3;
                //trianglesListJob.AddNoResize(tri);
            }

            cubeCorners.Dispose();
        }

        float3 interpolateVerts(float4 v1, float4 v2)
        {
            float t = (IsoLevelJob - v1.w) / (v2.w - v1.w);
            return v1.xyz + t * (v2.xyz - v1.xyz);
        }

        int indexFromCoord(int x, int y, int z)
        {
            return z * MapNumPointPerAxisXZJob * MapNumPointPerAxisYJob + y * MapNumPointPerAxisXZJob + x;
        }
    }
}
    */