using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapTesting : MonoBehaviour
{
    [SerializeField] HeatMapVisual heatMapVisual;
    private Grid<HeatMapGridObject> grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<HeatMapGridObject> (5, 5, 5f, new Vector3 (-20f,-18f), (Grid<HeatMapGridObject> g ,int x, int y) => new HeatMapGridObject(g,x,y));

        heatMapVisual.SetGrid(grid);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;
            HeatMapGridObject heatMapGridObject = grid.GetGridObject(worldPosition);
            if(heatMapGridObject != null)
            {
                //Debug.Log(grid.GetXY(worldPosition));
                heatMapGridObject.AddValue(5);
                //Debug.Log(heatMapGridObject.value.ToString());
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;
            Debug.Log(grid.GetGridObject(worldPosition));
        }
    }

    public class HeatMapGridObject
    {
        
        private const int MAX = 100;
        private const int MIN = 0;

        //public enum apasih
        //{
        //    None,
        //    Ground,
        //    Forest,
        //    Mountain
        //}

        private Grid<HeatMapGridObject> grid;
        private int x;
        private int y;
        public int value;
        //public apasih tilemapSprite;

        public HeatMapGridObject(Grid<HeatMapGridObject> grid, int x, int y)
        {

            this.grid = grid;
            this.x = x;
            this.y = y;
        }
        public void AddValue(int addvalue)
        {
            //tilemapSprite = apa;
            value += addvalue;
            value = Mathf.Clamp(value, MIN, MAX);
            grid.TriggerGridObjectChanged(x, y);
        }

        public float GetValueNormalized()
        {
            return (float)value / MAX;
        }

        public override string ToString()
        {
            return value.ToString();
        }

    }
}
