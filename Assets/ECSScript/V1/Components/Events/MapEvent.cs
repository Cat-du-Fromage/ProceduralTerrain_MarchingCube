using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.Data.Event
{
    public struct CreationChunksEntityEvent : IComponentData { }
    public struct HeightMapBigMapCalculEvent : IComponentData { }
    public struct ColorMapCalculEvent : IComponentData { }
    public struct MeshComponentsCalculEvent : IComponentData { }
    public struct ChunksEntityCreated : IComponentData {}
}
