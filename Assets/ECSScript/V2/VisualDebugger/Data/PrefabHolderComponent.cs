using Unity.Entities;

namespace KaizerWaldCode.Debugging.Points.Data.Authoring
{
    [GenerateAuthoringComponent]
    public struct PrefabHolderComponent : IComponentData
    {
        public Entity prefabEntity;
    }
}
