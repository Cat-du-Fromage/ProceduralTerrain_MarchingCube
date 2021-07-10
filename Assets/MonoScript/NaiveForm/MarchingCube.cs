using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.noise;
using Unity.Rendering;
using UnityEditor.VisualScripting.Model;
using UnityEngine.Rendering;
using static KaizerWaldCode.TriangulationTable;

namespace KaizerWaldCode
{
    public class MarchingCube : MonoBehaviour
    {
        public int size;
        public int height;
        public float terrainSurface = 0.5f;
        private float[,,] terrainMap;

        private List<Vector3> _verticesPosition = new List<Vector3>();
        private List<int> _triangles = new List<int>();

        private MeshFilter _meshFilter;
        //private int _configIndex = -1;

        // Start is called before the first frame update
        void Start()
        {
            ClearMeshData();
            _meshFilter = GetComponent<MeshFilter>();
            terrainMap = new float[size+1, size + 1, size+1];
            PopulateTerrainMap();
            CreateMeshData();
            BuildMesh();
        }

        void OnValidate()
        {

            ClearMeshData();
            /*
            _meshFilter = GetComponent<MeshFilter>();
            terrainMap = new float[size + 1, size + 1, size + 1];
            PopulateTerrainMap();
            CreateMeshData();
            */
        }

        // Update is called once per frame
        void Update()
        {
        }

        void PopulateTerrainMap()
        {
            for (int x = 0; x < size+1; x++)
            {
                for (int y = 0; y < size + 1; y++)
                {
                    for (int z = 0; z < size+1; z++)
                    {
                        //float thisHeight = (float)height * Mathf.PerlinNoise((float)x/16f*1.5f+0.001f, (float)z /16f*1.5f+0.001f);
                        float thisHeight = (float)height * noise.snoise(new float3((float)x/ 16f * 1.5f + 0.001f, (float)y/ 16f * 1.5f + 0.001f, (float)z/ 16f * 1.5f + 0.001f));
                        float point = 0;

                        if (x == 4 && z == 5)
                            point = 1f;
                        else if (y <= thisHeight - terrainSurface)
                            point = 0f;
                        else if (y > thisHeight + terrainSurface)
                            point = 1f;
                        else if (y > thisHeight)
                            point = (float)y - thisHeight;
                        else
                            point = thisHeight - (float)y;

                        terrainMap[x, y, z] = point;
                    }
                }
            }
        }

        void CreateMeshData()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        float[] cube = new float[8];
                        for (int i = 0; i < 8; i++)
                        {
                            Vector3Int corner = new Vector3Int(x, y, z) + CornerTable[i];
                            cube[i] = terrainMap[corner.x, corner.y, corner.z];
                        }
                        MarchCube(new Vector3(x,y,z), cube);
                    }
                }
            }
        }

        void ClearMeshData()
        {
            _verticesPosition.Clear();
            _triangles.Clear();
        }

        void BuildMesh()
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;

            mesh.vertices = _verticesPosition.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
        }

        int GetCubConfig(float[] cube)
        {
            int configurationIndex = 0;
            // 8 corners
            for (int i = 0; i < 8; i++)
            {
                if (cube[i] > terrainSurface)
                {
                    // 00000000 1<<i put 1 at the i position
                    // if i = 3 => 00100000
                    configurationIndex |= 1 << i;
                }
            }
            
            return configurationIndex;
        }

        void MarchCube(Vector3 position, float[] cube)
        {
            int configIndex = GetCubConfig(cube);

            if (configIndex == 0 || configIndex == 255) {return;}

            int edgeIndex = 0;
            // 5 because there is a max of 5 triangle in a "small cube"
            for (int i = 0; i < 5; i++)
            {
                for (int p = 0; p < 3; p++)
                {
                    int indice = triangulation[configIndex, edgeIndex];
                    if (indice == -1) { return; }

                    Vector3 vert1 = position + EdgeTable[indice, 0];
                    Vector3 vert2 = position + EdgeTable[indice, 1];

                    Vector3 vertPosition = (vert1+ vert2) / 2f;

                    _verticesPosition.Add(vertPosition);
                    _triangles.Add(_verticesPosition.Count-1);
                    edgeIndex++;
                }
            }
            /*
            for (int i = 0; TriangulationTable.triangulation[configIndex,i] != -1; i+=3)
            {

            }
            */
        }

        
    }
}

