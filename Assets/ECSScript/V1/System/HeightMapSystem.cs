using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using MapJobs = KaizerWaldCode.Jobs;
using MapSett = KaizerWaldCode.Data.MapSettings;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Rendering;

namespace KaizerWaldCode.System
{
    public class HeightMapSystem : SystemBase
    {
        EntityManager _em;
        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(typeof(Data.Event.HeightMapBigMapCalculEvent)));
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnStartRunning()
        {
            Entity mapSettings = GetSingletonEntity<Data.Tag.MapSettings>();
            int mapSize = GetComponent<MapSett.MapSize>(mapSettings).Value;
            /*========================
             * Random octaves Offset Job
             * return : OctOffsetArrayJob
             ========================*/
            //~10 Iteration MAX NO NEED FOR COMPUTE SHADER
            NativeArray<float3> octaveOffsetNativeArray = new NativeArray<float3>(GetComponent<MapSett.Octaves>(mapSettings).Value, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            //-------------------------------------------------------------------
            MapJobs.NoiseRandomJob noiseRandomJob = new MapJobs.NoiseRandomJob()
            {
                RandomJob = new Unity.Mathematics.Random((uint)GetComponent<MapSett.Seed>(mapSettings).Value),
                OffsetJob = GetComponent<MapSett.Offset>(mapSettings).Value,
                OctOffsetArrayJob = octaveOffsetNativeArray,
            };
            JobHandle noiseRandomJobHandle = noiseRandomJob.Schedule(octaveOffsetNativeArray.Length, JobsUtility.JobWorkerCount - 1);
            //needed for compute shader => can't use dependency in this case
            //noiseRandomJobHandle.Complete();
            NativeArray<float4> heightMapNativeArray = new NativeArray<float4>(math.mul(mapSize, mapSize)* mapSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            //-------------------------------------------------------------------
            MapJobs.NoiseDensityJob noiseDensityJob = new MapJobs.NoiseDensityJob()
            {
                MapSizeJob = mapSize,
                FullMapSizeJob = new float3(mapSize, mapSize, mapSize),
                MapCenterJob = new float3(mapSize, mapSize, mapSize) /2f,
                OctavesJob = GetComponent<MapSett.Octaves>(mapSettings).Value,
                LacunarityJob = GetComponent<MapSett.Lacunarity>(mapSettings).Value,
                PersistanceJob = GetComponent<MapSett.Persistance>(mapSettings).Value,
                OctOffsetArray = octaveOffsetNativeArray,
                NoiseWeightJob = GetComponent<Data.Voxel.NoiseWeight>(mapSettings).Value,
                SpacingJob = GetComponent<Data.Voxel.PointSpacing>(mapSettings).Value,
                FloorOffsetJob = GetComponent<Data.Voxel.FloorOffset>(mapSettings).Value,
                HardFloorJob = GetComponent<Data.Voxel.HardFloor>(mapSettings).Value,
                HardFloorWeightJob = GetComponent<Data.Voxel.HardFloorWeight>(mapSettings).Value,
                WeightMultiplierJob = GetComponent<Data.Voxel.WeightMultiplier>(mapSettings).Value,
                ShaderParametersJob = GetComponent<Data.Voxel.ShaderParameters>(mapSettings).Value,
                NoiseMap = heightMapNativeArray,
            };
            JobHandle noiseDensityJobHandle = noiseDensityJob.Schedule(heightMapNativeArray.Length, JobsUtility.JobWorkerCount - 1, noiseRandomJobHandle);
            noiseDensityJobHandle.Complete();

            GetBuffer<Data.DynamicBuffer.HeightMap>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<float4>().CopyFrom(heightMapNativeArray);
            heightMapNativeArray.Dispose();

            _em.RemoveComponent<Data.Event.HeightMapBigMapCalculEvent>(GetSingletonEntity<Data.Tag.MapEventHolder>());
            _em.AddComponent<Data.Event.MeshComponentsCalculEvent>(GetSingletonEntity<Data.Tag.MapEventHolder>());
        }

        protected override void OnUpdate()
        {
        }
    }
}