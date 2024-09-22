using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class AttackVisual : MonoBehaviour
{
    [Serializable]
    public struct TilemapSpriteUV
    {
        public TileMap.TilemapObject.AttackDisplay deployableSprite;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }

    [SerializeField] private TilemapSpriteUV[] deployableSpriteUVarray;
    private Grid<TileMap.TilemapObject> grid;
    private Mesh mesh;
    private bool updateMesh;
    private bool showMesh = false;
    private Dictionary<TileMap.TilemapObject.AttackDisplay, UVCoords> uvCoordsDictionary;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Texture texture = GetComponent<MeshRenderer>().material.mainTexture;
        float textureWidth = texture.width;
        float textureHeight = texture.height;

        uvCoordsDictionary = new Dictionary<TileMap.TilemapObject.AttackDisplay, UVCoords>();
        foreach (TilemapSpriteUV tilemapSpriteUV in deployableSpriteUVarray)
        {
            uvCoordsDictionary[tilemapSpriteUV.deployableSprite] = new UVCoords
            {
                uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight),
            };
        }
    }

    public void SetGrid(TileMap tilemap, Grid<TileMap.TilemapObject> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();
        tilemap.OnLoaded += Tilemap_OnLoaded;
        grid.OnGridObjectChanged += Grid_OnValueChanged;
    }

    private void Tilemap_OnLoaded(object sender, System.EventArgs e)
    {
        //UpdateHeatMapVisual();
        updateMesh = true;
    }

    private void Grid_OnValueChanged(object sender, Grid<TileMap.TilemapObject>.OnGridObjectChangedEventArgs e)
    {
        //UpdateHeatMapVisual();
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }
    public void HideHeatMapVisual()
    {
        showMesh = false;
        UpdateHeatMapVisual();
    }

    public void ShowHeatMapVisual()
    {
        showMesh = true;
        UpdateHeatMapVisual();
    }
    private void UpdateHeatMapVisual()
    {

        CreateEmptyMeshArray(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                //Debug.Log(index);
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                TileMap.TilemapObject gridObject = grid.GetGridObject(x, y);
                TileMap.TilemapObject.AttackDisplay attackDisplay = gridObject.GetAttackDisplay();

                Vector2 gridUV00, gridUV11;
                //Debug.Log(deployability);
                if (attackDisplay == TileMap.TilemapObject.AttackDisplay.NotOnSight || showMesh == false)
                {
                    gridUV00 = Vector2.zero;
                    gridUV11 = Vector2.zero;
                    quadSize = Vector3.zero;
                }
                else
                {
                    UVCoords uVCoords = uvCoordsDictionary[attackDisplay];
                    gridUV00 = uVCoords.uv00;
                    gridUV11 = uVCoords.uv11;
                }

                AddToMeshArray(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                //AddToMeshArray(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        SetTransparency(meshRenderer,0.3f);
        //GetComponent<MeshFilter>().mesh = mesh;

    }

    public void SetTransparency(MeshRenderer meshRenderer, float alpha)
    {
        foreach (Material material in meshRenderer.materials)
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;

        }
    }

    #region Utils
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
        if (skewed)
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
            vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
            vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
            vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
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
