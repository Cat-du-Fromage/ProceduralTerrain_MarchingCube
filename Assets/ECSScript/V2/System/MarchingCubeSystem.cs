using System.Linq;
using KaizerWaldCode.V2.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using mapSet = KaizerWaldCode.V2.Data.Settings.Map;
/*
namespace KaizerWaldCode.V2.System
{
    public class MarchingCubeSystem : SystemBase
    {
        EntityManager _em;

        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(V2.Data.Event.Event_MarchingCube)));
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnStartRunning()
        {
            Entity mapSetting = GetSingletonEntity<Data.Tag.MapSetting>();

            int numPointMapAxisXZ = _em.GetComponentData<mapSet.MapNumPointPerAxisXZ>(mapSetting).Value;
            int numPointMapAxisY = _em.GetComponentData<mapSet.MapNumPointPerAxisY>(mapSetting).Value;
            int mapBoundXZ = _em.GetComponentData<mapSet.MapBoundXZ>(mapSetting).Value;
            int mapBoundY = _em.GetComponentData<mapSet.MapBoundY>(mapSetting).Value;

            NativeArray<int> cornerIndexAFromEdgeNatArr = new NativeArray<int>(12, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> cornerIndexBFromEdgeNatArr = new NativeArray<int>(12, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> triangulationNatArr = new NativeArray<int>(4096, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            cornerIndexAFromEdgeNatArr.CopyFrom(TriangulationTable.cornerIndexAFromEdge);
            cornerIndexBFromEdgeNatArr.CopyFrom(TriangulationTable.cornerIndexBFromEdge);
            triangulationNatArr.CopyFrom(TriangulationTable.ECStriangulation);

            int numVoxXZ = _em.GetComponentData<Data.Settings.Voxel.MapNumVoxelPerAxisXZ>(mapSetting).Value;
            int numVoxY = _em.GetComponentData<Data.Settings.Voxel.MapNumVoxelPerAxisY>(mapSetting).Value;

            NativeArray<float3> verticesNatArr = new NativeArray<float3>((math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ) * math.mul(3, 5), Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> trianglesNatArr = new NativeArray<int>((math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ)*math.mul(3,5), Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            Jobs.MarchingCubeJobs marchingCubeJob = new Jobs.MarchingCubeJobs()
            {
                MapBoundXZJob = mapBoundXZ,
                MapBoundYJob = mapBoundY,
                MapNumPointPerAxisXZJob = numPointMapAxisXZ,
                MapNumPointPerAxisYJob = numPointMapAxisY,
                SpacingJob = _em.GetComponentData<Data.Settings.Chunk.PointSpacing>(mapSetting).Value,
                IsoLevelJob = GetComponent<Data.Settings.Chunk.PointSpacing>(mapSetting).Value,
                cornerIndexAFromEdgeJob = cornerIndexAFromEdgeNatArr,
                cornerIndexBFromEdgeJob = cornerIndexBFromEdgeNatArr,
                triangulationJob = triangulationNatArr,
                VerticesJob = verticesNatArr,
                TrianglesJob = trianglesNatArr,
                //trianglesJob = trianglesList.AsParallelWriter(),
                pointsJob = GetBuffer<V2.Data.ChunksData.DynamicBuffer.PointsBuffer>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<float4>().AsNativeArray(),
            };
            JobHandle marchingCubeJpbHandle = marchingCubeJob.Schedule(math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ,  JobsUtility.JobWorkerCount - 1);
            marchingCubeJpbHandle.Complete();
            Debug.Log($"triangles : {trianglesNatArr.Length}");
            Debug.Log($"vertices : {verticesNatArr.Length}");
            for (int i = 0; i < 50; i++)
            {
                Debug.Log($"triangles{i} : {trianglesNatArr[i]}");
            }
            //Try to remove


            DynamicBuffer<Data.ChunksData.DynamicBuffer.Vertices> verticesBuffer = GetBuffer<Data.ChunksData.DynamicBuffer.Vertices>(GetSingletonEntity<Data.Tag.ChunksHolder>());
            DynamicBuffer<Data.ChunksData.DynamicBuffer.Triangles> trianglesBuffer = GetBuffer<Data.ChunksData.DynamicBuffer.Triangles>(GetSingletonEntity<Data.Tag.ChunksHolder>());

            verticesBuffer.Reinterpret<float3>().CopyFrom(verticesNatArr);
            trianglesBuffer.Reinterpret<int>().CopyFrom(trianglesNatArr);

            Debug.Log($"triangles before trim : {verticesBuffer.Length}");
            Debug.Log($"vertices before trim : {trianglesBuffer.Length}");
            verticesNatArr.Dispose();
            trianglesNatArr.Dispose();

            verticesBuffer.TrimExcess();
            trianglesBuffer.TrimExcess();
            Debug.Log($"triangles after trim : {verticesBuffer.Length}");
            Debug.Log($"vertices after trim : {trianglesBuffer.Length}");

            //TestMesh
            //float3[] vertArr = verticesBuffer.Reinterpret<float3>().ToArray();
            //int[] triArr = trianglesBuffer.Reinterpret<int>().ToArray();

            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = verticesBuffer.AsNativeArray().Reinterpret<Vector3>().ToArray();
            mesh.triangles = trianglesBuffer.AsNativeArray().Reinterpret<int>().ToArray();
            mesh.RecalculateNormals();
            _em.AddSharedComponentData(GetSingletonEntity<V2.Data.Tag.MapEventHolder>(), new RenderMesh() {mesh = mesh});

            _em.RemoveComponent<V2.Data.Event.Event_MarchingCube>(GetSingletonEntity<V2.Data.Tag.MapEventHolder>());
        }

        protected override void OnUpdate()
        {

        }
    }
}
*/