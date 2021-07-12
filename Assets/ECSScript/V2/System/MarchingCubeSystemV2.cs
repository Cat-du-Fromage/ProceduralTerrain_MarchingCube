using KaizerWaldCode.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using MapSet = KaizerWaldCode.V2.Data.Settings.Map;

namespace KaizerWaldCode.V2.System
{
    struct Triangle
    {
        public Vector3 c;
        public Vector3 b;
        public Vector3 a;

        public Vector3 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }

    public class MarchingCubeSystemV2 : SystemBase
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

            ComputeShader marchingCubesCSH = AssetDatabase.LoadAssetAtPath<ComputeShader>("Assets/ECSScript/V2/ComputeShader/MarchingCubes.compute");
            int MarchCubeKernel = marchingCubesCSH.FindKernel("MarchingCubes");

            int numVoxXZ = _em.GetComponentData<Data.Settings.Voxel.MapNumVoxelPerAxisXZ>(mapSetting).Value;
            int numVoxY = _em.GetComponentData<Data.Settings.Voxel.MapNumVoxelPerAxisY>(mapSetting).Value;
            int numVoxels = math.mul(numVoxXZ, numVoxY) * numVoxXZ;
            int maxTriangleCount = numVoxels * 5;


            //Array
            float4[] pointsArray = GetBuffer<Data.ChunksData.DynamicBuffer.PointsBuffer>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<float4>().AsNativeArray().ToArray();
            //Set buffers
            ComputeBuffer trianglesBuffer = new ComputeBuffer(maxTriangleCount, sizeof(float) * math.mul(3, 3), ComputeBufferType.Append);
            trianglesBuffer.SetCounterValue(0);
            marchingCubesCSH.SetBuffer(MarchCubeKernel, "triangles", trianglesBuffer);

            ComputeBuffer pointsBuffer = new ComputeBuffer(GetBuffer<Data.ChunksData.DynamicBuffer.PointsBuffer>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Length, sizeof(float) * 4);
            UtComputeShader.CSHSetBuffer(marchingCubesCSH, MarchCubeKernel,"points",pointsBuffer, pointsArray);

            ComputeBuffer triangleCount = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);//what is this?
            //Set variables
            marchingCubesCSH.SetFloat("isoLevel", GetComponent<MapSet.IsoSurface>(mapSetting).Value);
            marchingCubesCSH.SetInt("numPointsPerAxisXZ", GetComponent<MapSet.MapNumPointPerAxisXZ>(mapSetting).Value);
            marchingCubesCSH.SetInt("numPointsPerAxisY", GetComponent<MapSet.MapNumPointPerAxisY>(mapSetting).Value);
            //DISPATCH
            int numThreadsPerAxisXZ = (int)math.ceil((float)numVoxXZ / (float)8);
            int numThreadsPerAxisY = (int)math.ceil((float)numVoxY / (float)8);
            marchingCubesCSH.Dispatch(0, numThreadsPerAxisXZ, numThreadsPerAxisY, numThreadsPerAxisXZ);
            //COPY NUM TRIANGLE INTO trianglCount Buffer
            ComputeBuffer.CopyCount(trianglesBuffer, triangleCount,0);

            int[] triCountArray = { 0 };
            triangleCount.GetData(triCountArray);
            int numTris = triCountArray[0];

            Triangle[] tris = new Triangle[numTris];
            trianglesBuffer.GetData(tris, 0, 0, numTris);

            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.indexFormat = IndexFormat.UInt32;

            var vertices = new Vector3[numTris * 3];
            var meshTriangles = new int[numTris * 3];

            for (int i = 0; i < numTris; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    meshTriangles[i * 3 + j] = i * 3 + j;
                    vertices[i * 3 + j] = tris[i][j];
                }
            }
            mesh.vertices = vertices;
            mesh.triangles = meshTriangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            //WITHOUT this the camera won't render at certain angle!
            _em.SetComponentData(GetSingletonEntity<Data.Tag.AuthoringMap>(), new RenderBounds
            {
                Value = new AABB
                {
                    Center = new float3(mesh.bounds.center.x, mesh.bounds.center.y, mesh.bounds.center.z),
                    Extents = new float3(mesh.bounds.extents.x, mesh.bounds.extents.y, mesh.bounds.extents.z)
                }
            });

            Material mat = _em.GetSharedComponentData<RenderMesh>(GetSingletonEntity<Data.Tag.AuthoringMap>()).material;
            _em.SetSharedComponentData(GetSingletonEntity<Data.Tag.AuthoringMap>(), new RenderMesh(){material = mat , mesh = mesh});
            UtComputeShader.CSHReleaseBuffers(trianglesBuffer, pointsBuffer, triangleCount);

            _em.RemoveComponent<V2.Data.Event.Event_MarchingCube>(GetSingletonEntity<Data.Tag.MapEventHolder>());
        }

        protected override void OnUpdate()
        {
        }

    }
}
