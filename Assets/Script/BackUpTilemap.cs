using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileMap.TilemapObject;
//using static Testing;
/*
public class BackUpTilemap{

    public event EventHandler OnLoaded;

    private Grid<TilemapObject> grid;
    public BackUpTilemap(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new Grid<TilemapObject>(width, height, cellSize, originPosition, (Grid<TilemapObject> g, int x, int y) => new TilemapObject(g, x, y)); 
    }

    public void SetTilemapSprite(Vector3 worldPosition, TilemapObject.TilemapSprite tilemapSprite)
    {
        TilemapObject tilemapObject = grid.GetGridObject(worldPosition);
        //Debug.Log(tilemapObject);
        if (tilemapObject != null)
        {
            //Debug.Log("ada");
            tilemapObject.SetTilemapSprite(tilemapSprite);
            tilemapObject.SetisBlocking(tilemapSprite);
        }
    }

    public void SetTilemapVisual(TileMapVisual tileMapVisual)
    {
        tileMapVisual.SetGrid(this, grid);
    }
    
    public void GetTilemapInformation(Vector3 mousePosition)
    {
        TilemapObject tilemapObject = grid.GetGridObject(mousePosition);
        tilemapObject.GetTilemapInformation();
        
    }
    public void Save()
    {
        List<TilemapObject.SaveObject> tilemapObjectSaveObjectList = new List<TilemapObject.SaveObject>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TilemapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObjectSaveObjectList.Add(tilemapObject.Save());
            }
        }

        SaveObject saveObject = new SaveObject { tilemapObjectSaveObjectArray = tilemapObjectSaveObjectList.ToArray() };

        SaveSystem.SaveObject(saveObject);
    }

    public void Load()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        foreach(TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
        {
            TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);

            tilemapObject.Load(tilemapObjectSaveObject);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }
    public class SaveObject
    {
        public TilemapObject.SaveObject[] tilemapObjectSaveObjectArray;

    }

    public class TilemapObject
    {
        public enum TilemapSprite
        {
            None,
            Ground,
            Forest,
            Mountain,
            GroundDark,
            ForestDark,
            MountainDark,
            Dirt
        }
        private Grid<TilemapObject> grid;
        private int x;
        private int y;
        private bool isBlocking;
        public TilemapSprite tilemapSprite;
        public UnitGridCombat unitGridCombat;

        public TilemapObject(Grid<TilemapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            isBlocking = true;
        }

        public void SetisBlocking(TilemapSprite tilemapSprite)
        {
            if(tilemapSprite == TilemapSprite.Forest|| tilemapSprite == TilemapSprite.ForestDark || tilemapSprite == TilemapSprite.Mountain || tilemapSprite == TilemapSprite.MountainDark)
            {
                this.isBlocking = true;
            }
            else
            {
                this.isBlocking = false;
            }
            grid.TriggerGridObjectChanged(x,y);
        }
        public void SetTilemapSprite(TilemapSprite tilemapSprite)
        {
            this.tilemapSprite = tilemapSprite;
            grid.TriggerGridObjectChanged(x, y);
            
        }

        public TilemapSprite GetTilemapSprite()
        {
            return tilemapSprite;
        }

        public void GetTilemapInformation()
        {
            Debug.Log(x);
            Debug.Log(y);
            Debug.Log(isBlocking);
        }

        public void SetUnitGridCombat(UnitGridCombat unitGridCombat)
        {
            this.unitGridCombat = unitGridCombat;
        }

        public UnitGridCombat GetUnitGridCombat() 
        { 
            return unitGridCombat; 
        }
        public override string ToString()
        {
            return unitGridCombat.ToString();
        }
        #region Saving
        [Serializable]
        public class SaveObject
        {
            public TilemapSprite tilemapSprite;
            public int x;
            public int y;
            public bool isBlocking;
            public UnitGridCombat unitGridCombat;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSprite = tilemapSprite,
                x = x,
                y = y,
                isBlocking = isBlocking,
                unitGridCombat = unitGridCombat
                
            };
        }

        public void Load(SaveObject saveObject)
        {
            tilemapSprite = saveObject.tilemapSprite;
            isBlocking = saveObject.isBlocking;
            unitGridCombat = saveObject.unitGridCombat;
            grid.TriggerGridObjectChanged(x, y);
        }
        #endregion
    }
}
*/