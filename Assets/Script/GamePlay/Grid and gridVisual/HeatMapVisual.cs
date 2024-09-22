using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static HeatMapTesting;

public class HeatMapVisual : MonoBehaviour
{
    private Grid<HeatMapGridObject> grid;
    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    public void SetGrid(Grid<HeatMapGridObject> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();
        grid.OnGridObjectChanged += Grid_OnValueChanged;
    }

    private void Grid_OnValueChanged(object sender, Grid<HeatMapGridObject>.OnGridObjectChangedEventArgs e)
    {
        Debug.Log("Grid_OnValueChanged");
        UpdateHeatMapVisual();
    }

    private void UpdateHeatMapVisual()
    {
        
        CreateEmptyMeshArray(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                HeatMapGridObject gridValue = grid.GetGridObject(x, y);
                float gridValueNormalized = gridValue.GetValueNormalized();
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);

                AddToMeshArray(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        //GetComponent<MeshFilter>().mesh = mesh;

    }
    #region utils
    private void CreateEmptyMeshArray(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
    {
        vertices = new Vector3[quadCount * 4];
        uvs = new Vector2[quadCount * 4];
        triangles = new int[quadCount * 6];
    }

    private void AddToMeshArray(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        //Relocate Vertices
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        baseSize *= .5f;

        bool skewed = baseSize.x != baseSize.y;
        if(skewed)
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot-270) * baseSize;
            vertices[vIndex1] = pos + GetQuaternionEuler(rot-180) * baseSize;
            vertices[vIndex2] = pos + GetQuaternionEuler(rot-90) * baseSize;
            vertices[vIndex3] = pos + GetQuaternionEuler(rot-0) * baseSize;
        }

        
        //Relocate UVs
        uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
        uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
        uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
        uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

        //Debug.Log("vertices: "+vertices[vIndex0].ToString());
        //Debug.Log("uvs:"+ uvs[vIndex0].ToString());
        //Debug.Log("vertices: "+vertices[vIndex1].ToString());
        //Debug.Log("uvs:"+ uvs[vIndex1].ToString());
        //Debug.Log("vertices: "+vertices[vIndex2].ToString());
        //Debug.Log("uvs:"+ uvs[vIndex2].ToString());
        //Debug.Log("vertices: "+vertices[vIndex3].ToString());
        //Debug.Log("uvs:"+ uvs[vIndex3].ToString());
        //Relocate Triangle
        int tIndex = index * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex2;
        triangles[tIndex + 2] = vIndex1;
        triangles[tIndex + 3] = vIndex0;
        triangles[tIndex + 4] = vIndex3;
        triangles[tIndex + 5] = vIndex2;
    }

    private Quaternion GetQuaternionEuler(float rot)
    {
        return Quaternion.Euler(0, 0, rot);
    }

    #endregion
}