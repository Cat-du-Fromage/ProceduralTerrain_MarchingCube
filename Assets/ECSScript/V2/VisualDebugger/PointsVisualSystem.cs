using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using mapSet = KaizerWaldCode.V2.Data.Settings.Map;
namespace KaizerWaldCode.Debugging.Points
{
    public class PointsVisualSystem : SystemBase
    {
        private EntityQueryDesc _eventDescription;
        private EntityManager _em;
        private BeginSimulationEntityCommandBufferSystem _ecbBS;
        protected override void OnCreate()
        {
            _eventDescription = new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    typeof(V2.Data.Event.Event_DebuggingPoints)
                },
                None = new ComponentType[]
                {
                    typeof(V2.Data.Event.Event_ProcessPointsPosition)
                }
            };
            RequireForUpdate(GetEntityQuery(_eventDescription));
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
            _ecbBS = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            #region Chunks Creation

            Entity mapSetting = GetSingletonEntity<V2.Data.Tag.MapSetting>();

            int numPointMapAxisXZ = _em.GetComponentData<mapSet.MapNumPointPerAxisXZ>(mapSetting).Value;
            int numPointMapAxisY = _em.GetComponentData<mapSet.MapNumPointPerAxisY>(mapSetting).Value;
            int numChunk = math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ;

            Entity prefabEntity = GetComponent<Data.Authoring.PrefabHolderComponent>(GetSingletonEntity<Data.Tag.PointDebuggerTag>()).prefabEntity;

            NativeArray<Entity> spheres = _em.Instantiate(prefabEntity, numChunk, Allocator.Persistent);
            spheres.Dispose();
            #endregion Chunks Creation


        }

        protected override void OnUpdate()
        {
            DynamicBuffer<V2.Data.ChunksData.DynamicBuffer.PointsBuffer> pointsBuffer = GetBuffer<V2.Data.ChunksData.DynamicBuffer.PointsBuffer>(GetSingletonEntity<V2.Data.Tag.ChunksHolder>());
            EntityCommandBuffer.ParallelWriter ecb = _ecbBS.CreateCommandBuffer().AsParallelWriter();
            Entities
                .WithReadOnly(pointsBuffer)
                .WithBurst()
                .WithAll<Data.Authoring.SphereTagAuthoring>()
                .ForEach((Entity ent, int entityInQueryIndex) =>
                {
                    ecb.SetComponent(entityInQueryIndex, ent, new Translation(){Value = pointsBuffer[entityInQueryIndex].Value.xyz });
                }).ScheduleParallel();

            float isoSurface = GetComponent<mapSet.IsoSurface>(GetSingletonEntity<V2.Data.Tag.MapSetting>()).Value;
            EntityCommandBuffer ecb2 = _ecbBS.CreateCommandBuffer();
            Entities
                .WithoutBurst()
                .WithReadOnly(isoSurface)
                .WithReadOnly(pointsBuffer)
                //.WithStructuralChanges()
                .WithAll<Data.Authoring.SphereTagAuthoring>()
                .ForEach((Entity ent, int entityInQueryIndex, in MaterialChanger material, in RenderMesh render) =>
                {
                    if (pointsBuffer[entityInQueryIndex].Value.w <= isoSurface)
                    {
                        ecb2.SetSharedComponent( ent, new RenderMesh(){mesh = render.mesh, material = material.Red});
                    }
                    else if(pointsBuffer[entityInQueryIndex].Value.w <= isoSurface + 2)
                    {
                        ecb2.SetSharedComponent( ent, new RenderMesh() { mesh = render.mesh, material = material.Blue});
                        //ecb2.DestroyEntity(ent);
                    }
                    else
                    {
                        ecb2.DestroyEntity(ent);
                    }
                }).Run();
            _ecbBS.AddJobHandleForProducer(this.Dependency);
            _em.RemoveComponent<V2.Data.Event.Event_DebuggingPoints>(GetSingletonEntity<V2.Data.Tag.MapEventHolder>());


        }
    }
}
