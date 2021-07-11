using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using mapSet = KaizerWaldCode.V2.Data.Settings.Map;
namespace KaizerWaldCode.V2.System
{
    public class PointsGenerationSystem : SystemBase
    {
        EntityQueryDesc _eventDescription;
        private EntityManager _em;

        protected override void OnCreate()
        {
            _eventDescription = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    typeof(Data.Event.Event_ProcessPointsPosition),
                },
            };
            RequireForUpdate(GetEntityQuery(_eventDescription));
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        protected override void OnStartRunning()
        {
            Entity mapSetting = GetSingletonEntity<Data.Tag.MapSetting>();

            /*========================
             * Random octaves Offset Job
             * return : OctOffsetArrayJob
             ========================*/
            //~10 Iteration MAX NO NEED FOR COMPUTE SHADER
            NativeArray<float3> octaveOffsetNativeArray = new NativeArray<float3>(GetComponent<Data.Settings.Noise.Octaves>(mapSetting).Value, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            //-------------------------------------------------------------------
            Jobs.NoiseRandomJob noiseRandomJob = new Jobs.NoiseRandomJob()
            {
                RandomJob = new Unity.Mathematics.Random((uint)GetComponent<Data.Settings.Noise.Seed>(mapSetting).Value),
                OctOffsetArrayJob = octaveOffsetNativeArray,
            };
            JobHandle noiseRandomJobHandle = noiseRandomJob.Schedule(octaveOffsetNativeArray.Length, JobsUtility.JobWorkerCount - 1);

            int numPointMapAxisXZ = _em.GetComponentData<mapSet.MapNumPointPerAxisXZ>(mapSetting).Value;
            int numPointMapAxisY = _em.GetComponentData<mapSet.MapNumPointPerAxisY>(mapSetting).Value;
            int mapBoundXZ = _em.GetComponentData<mapSet.MapBoundXZ>(mapSetting).Value;
            int mapBoundY = _em.GetComponentData<mapSet.MapBoundY>(mapSetting).Value;

            NativeArray<float4> points = new NativeArray<float4>(math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            Jobs.PointsJob pointsJob = new Jobs.PointsJob()
            {
                MapBoundXZJob = mapBoundXZ,
                MapBoundYJob = mapBoundY,
                MapNumPointPerAxisXZJob = numPointMapAxisXZ,
                MapNumPointPerAxisYJob = numPointMapAxisY,
                SpacingJob = _em.GetComponentData<Data.Settings.Chunk.PointSpacing>(mapSetting).Value,
                pointsJob = points,
                OctOffsetArrayJob = octaveOffsetNativeArray,
                OctavesJob = _em.GetComponentData<Data.Settings.Noise.Octaves>(mapSetting).Value,
                LacunarityJob = _em.GetComponentData<Data.Settings.Noise.Lacunarity>(mapSetting).Value,
                PersistanceJob = _em.GetComponentData<Data.Settings.Noise.Persistance>(mapSetting).Value,
                ScaleJob = _em.GetComponentData<Data.Settings.Noise.Scale>(mapSetting).Value,
                WeightMultiplierJob = _em.GetComponentData<Data.Settings.Noise.WeightMultiplier>(mapSetting).Value,
                NoiseWeightJob = _em.GetComponentData<Data.Settings.Noise.NoiseWeight>(mapSetting).Value,
            };
            JobHandle pointsJobHandle = pointsJob.Schedule(math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ, JobsUtility.JobWorkerCount - 1, noiseRandomJobHandle);
            pointsJobHandle.Complete();

            GetBuffer<Data.ChunksData.DynamicBuffer.PointsBuffer>(GetSingletonEntity<Data.Tag.ChunksHolder>()).Reinterpret<float4>().CopyFrom(points);
            points.Dispose();

            _em.RemoveComponent<Data.Event.Event_ProcessPointsPosition>(GetSingletonEntity<Data.Tag.MapEventHolder>());
            _em.AddComponent<Data.Event.Event_MarchingCube>(GetSingletonEntity<Data.Tag.MapEventHolder>());
        }

        protected override void OnUpdate()
        {

        }
    }
}
