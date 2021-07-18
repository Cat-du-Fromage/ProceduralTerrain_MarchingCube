using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.V3.Data.Tag
{
    public struct MapSetting : IComponentData { }
    public struct ChunksHolder : IComponentData { }
    public struct MapEventHolder : IComponentData { }
}

namespace KaizerWaldCode.V3.Data.Event
{
    public struct Event_ProcessPointsPosition : IComponentData { }
}
