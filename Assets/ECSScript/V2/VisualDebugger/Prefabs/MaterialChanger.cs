using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[GenerateAuthoringComponent]
public class MaterialChanger : IComponentData
{
    public Material Red;
    public Material Blue;
}
