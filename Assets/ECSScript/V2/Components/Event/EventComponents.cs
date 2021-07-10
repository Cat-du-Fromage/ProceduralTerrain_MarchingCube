using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.V2.Data.Event
{
    public struct Event_ProcessPointsPosition : IComponentData { }
    public struct Event_MarchingCube : IComponentData { }
    public struct Event_DebuggingPoints : IComponentData{}
}
