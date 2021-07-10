using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using KaizerWaldCode.V2;
using KaizerWaldCode.V2.Jobs;
using Unity.Animation;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using Debug = UnityEngine.Debug;
/*
namespace KaizerWaldCode.Mono
{
    public class TestPoints : MonoBehaviour
    {
        private EntityStore _mapSettingsStored;
        private EntityManager _em;
        public Entity _mapSetting;
        private NativeArray<float4> points;
        public float displayRadius = 0.5f;

        private float4[] testArr;

        private JobHandle pointsJobHandle;
        // Start is called before the first frame update
        void Awake()
        {
            //Array.Clear(0,testArr.Length, testArr);
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        void Start()
        {
            _mapSetting = GetComponent<EntityStore>().entity;
            int numPointMapAxisXZ = _em.GetComponentData<V2.Data.Settings.Map.MapNumPointPerAxisXZ>(_mapSetting).Value;
            int numPointMapAxisY = _em.GetComponentData<V2.Data.Settings.Map.MapNumPointPerAxisY>(_mapSetting).Value;
            int mapBoundXZ = _em.GetComponentData<V2.Data.Settings.Map.MapBoundXZ>(_mapSetting).Value;
            int mapBoundY = _em.GetComponentData<V2.Data.Settings.Map.MapBoundY>(_mapSetting).Value;
            points = new NativeArray<float4>(math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            V2.Jobs.PointsJob pointsJob = new PointsJob()
            {
                MapBoundXZJob = mapBoundXZ,
                MapBoundYJob = mapBoundY,
                MapNumPointPerAxisXZJob = numPointMapAxisXZ,
                MapNumPointPerAxisYJob = numPointMapAxisY,
                SpacingJob = _em.GetComponentData<V2.Data.Settings.Chunk.PointSpacing>(_mapSetting).Value,
                pointsJob = points,
            };
            pointsJobHandle = pointsJob.Schedule(math.mul(numPointMapAxisXZ, numPointMapAxisY) * numPointMapAxisXZ, JobsUtility.JobWorkerCount - 1);
            pointsJobHandle.Complete();
            testArr = new float4[points.Length];
            points.CopyTo(testArr);
            points.Dispose();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        void OnDrawGizmos()
        {
            if (testArr != null)
            {
                foreach (float4 point in testArr)
                {
                    if (point.w == 0)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(point.xyz, displayRadius);
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(point.xyz, displayRadius);
                    }
                }
            }
        }
        
        
    }
}
*/
