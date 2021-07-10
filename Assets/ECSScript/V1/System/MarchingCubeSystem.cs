using KaizerWaldCode.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using MapSett = KaizerWaldCode.Data.MapSettings;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

namespace KaizerWaldCode.System
{
    public struct Triangle
    {
        public float3 vertexC;
        public float3 vertexB;
        public float3 vertexA;
        public float3 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return vertexA;
                    case 1:
                        return vertexB;
                    default:
                        return vertexC;
                }
            }
        }
    };
    public class MarchingCubeSystem : SystemBase
    {
        EntityManager _em;

        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(Data.Event.MeshComponentsCalculEvent)));
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnStartRunning()
        {
            Entity mapSettings = GetSingletonEntity<Data.Tag.MapSettings>();
            int pointperAxis = GetComponent<Data.Voxel.NumPointPerAxis>(mapSettings).Value;
            NativeArray<int> cornerIndexAFromEdgeNatArr = new NativeArray<int>(12, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> cornerIndexBFromEdgeNatArr = new NativeArray<int>(12, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> triangulationNatArr = new NativeArray<int>(4096, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            cornerIndexAFromEdgeNatArr.CopyFrom(TriangulationTable.cornerIndexAFromEdge);
            cornerIndexBFromEdgeNatArr.CopyFrom(TriangulationTable.cornerIndexBFromEdge);
            triangulationNatArr.CopyFrom(TriangulationTable.ECStriangulation);

            NativeList<Triangle> trianglesList = new NativeList<Triangle>(4096,Allocator.Persistent);

            MarchingCubeJob marchingCubeJob = new MarchingCubeJob()
            {
                MapSizeJob = GetComponent<MapSett.MapSize>(mapSettings).Value,
                numPointsPerAxisJob = GetComponent<Data.Voxel.NumPointPerAxis>(mapSettings).Value,
                isoLevelJob = GetComponent<Data.Voxel.IsoLevel>(mapSettings).Value,
                cornerIndexAFromEdgeJob = cornerIndexAFromEdgeNatArr,
                cornerIndexBFromEdgeJob = cornerIndexBFromEdgeNatArr,
                triangulationJob = triangulationNatArr,
                trianglesListJob = trianglesList,
                pointsJob = GetBuffer<Data.DynamicBuffer.HeightMap>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<float4>().AsNativeArray(),
            };
            JobHandle marchingCubeJpbHandle = marchingCubeJob.Schedule(math.mul(pointperAxis, pointperAxis) * pointperAxis,
                    JobsUtility.JobWorkerCount - 1);
            marchingCubeJpbHandle.Complete();
            //temporary
            NativeArray<int> TempTri = new NativeArray<int>(trianglesList.Length * 3, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<float3> TempVertices = new NativeArray<float3>(trianglesList.Length * 3, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < trianglesList.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    TempTri[i * 3 + j] = i * 3 + j;
                    if(j ==0)
                        TempVertices[i * 3 + j] = trianglesList[i * 3 + j].vertexA;
                    else if(j==1)
                        TempVertices[i * 3 + j] = trianglesList[i * 3 + j].vertexB;
                    else
                        TempVertices[i * 3 + j] = trianglesList[i * 3 + j].vertexC;
                    //TempVertices[i * 3 + j] = trianglesList[i][j];
                }
            }

            GetBuffer<Data.Chunks.MeshBuffer.Triangles>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<int>().CopyFrom(TempTri);
            GetBuffer<Data.Chunks.MeshBuffer.Vertices>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<float3>().CopyFrom(TempVertices);
            //temporary
            TempTri.Dispose();
            TempVertices.Dispose();
            trianglesList.Dispose();
        }

        protected override void OnUpdate()
        {
            _em.RemoveComponent<Data.Event.MeshComponentsCalculEvent>(GetSingletonEntity<Data.Tag.MapEventHolder>());
        }
    }
}
