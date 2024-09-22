using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler_GridCombatSystem: MonoBehaviour
{
    private Grid<CombatMapGridObject> grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<CombatMapGridObject> (5, 5, 5f, new Vector3 (-20f,-18f), (Grid<CombatMapGridObject> g ,int x, int y) => new CombatMapGridObject(g,x,y));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;
            CombatMapGridObject heatMapGridObject = grid.GetGridObject(worldPosition);
            if(heatMapGridObject != null)
            {
                //Debug.Log(grid.GetXY(worldPosition));
                heatMapGridObject.AddValue(1);
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

    public class CombatMapGridObject
    {

        private Grid<CombatMapGridObject> grid;
        private int x;
        private int y;
        public int value;

        public CombatMapGridObject(Grid<CombatMapGridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }
        public void AddValue(int addValue)
        {
            value += addValue;
            grid.TriggerGridObjectChanged(x, y);
        }

        //public float GetValueNormalized()
        //{
        //    return (float)value / MAX;
        //}

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
