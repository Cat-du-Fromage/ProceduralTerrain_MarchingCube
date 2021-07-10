using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace KaizerWaldCode.Debugging.Points.Data
{
    namespace Tag
    {
        public struct PointDebuggerTag : IComponentData{}
        public struct DebugPoint : IComponentData { }
    }

    namespace Event
    {
        public struct Event_CreatePoints : IComponentData { }
    }
}
