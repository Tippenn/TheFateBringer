using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid<TGridObject> : MonoBehaviour
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }


    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    //private Sprite[,] spriteArray;
    public Grid(int width, int height,float cellsize, Vector3 originPosition, Func<Grid<TGridObject>,int,int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellsize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];
        
        //spriteArray = new Sprite[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this,x,y);
            }
        }

        bool showDebug = false;
        if (showDebug)
        {
            debugTextArray = new TextMesh[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    debugTextArray[x, y] = CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    //spriteArray[x,y] = CreateWorldSprite(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
                //spriteArray[eventArgs.x, eventArgs.y] = gridArray[eventArgs.x, eventArgs.y];
            };
        }
    }

    public int GetWidth()
    {
        return gridArray.GetLength(0);
    }

    public int GetHeight()
    {
        return gridArray.GetLength(1);
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt((worldPosition-originPosition).x / cellSize), Mathf.FloorToInt((worldPosition - originPosition).y / cellSize));
    }

    public Vector3 GetWorldPosition(int x,int y)
    {
        return new Vector3(x,y) * cellSize + originPosition;
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {  
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        Vector2Int XY = GetXY(worldPosition);
        x = XY.x;
        y = XY.y;
        SetGridObject(x, y, value);
    }

    public TGridObject GetGridObject(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        Vector2Int XY = GetXY(worldPosition);
        x = XY.x;
        y = XY.y;
        return GetGridObject(x, y);
    }


    #region utilsTextMesh
    //utils
    public TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localposition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if(color == null) color = Color.white;
        return CreateWorldText(parent, text, localposition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public TextMesh CreateWorldText(Transform parent, string text, Vector3 localposition, int fontSize, Color color, TextAnchor textAnchor,TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent,false);
        transform.localPosition = localposition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    #endregion


    #region utilsSprite
    public Sprite CreateWorldSprite(string text, Transform parent = null, Vector3 localposition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldSprite(parent, text, localposition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    public Sprite CreateWorldSprite(Transform parent, string text, Vector3 localposition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Sprite", typeof(SpriteRenderer));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localposition;
        Sprite sprite = gameObject.GetComponent<Sprite>();
        //textMesh.anchor = textAnchor;
        //textMesh.alignment = textAlignment;
        //textMesh.text = text;
        //textMesh.fontSize = fontSize;
        //textMesh.color = color;
        //textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return sprite;
    }

    #endregion
}
