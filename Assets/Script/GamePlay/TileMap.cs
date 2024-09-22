using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TileMap.TilemapObject;
//using static Testing;

public class TileMap{

    public event EventHandler OnLoaded;
    private Grid<TilemapObject> grid;
    public TileMap(int width, int height, float cellSize, Vector3 originPosition)
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
            tilemapObject.SetisBlocking();
        }
    }

    public void SetAttackVisual(AttackVisual attackVisual)
    {
        attackVisual.SetGrid(this,grid);
    }

    public void SetTilemapVisual(TileMapVisual tileMapVisual)
    {
        tileMapVisual.SetGrid(this, grid);
    }

    public void SetPossibleDropVisual(PossibleDropVisual possibleDropVisual)
    {
        possibleDropVisual.SetGrid(this, grid);
    }

    public void GetTilemapInformation(Vector3 mousePosition, TMP_Text tileInfo)
    {
        TilemapObject tilemapObject = grid.GetGridObject(mousePosition);
        if (tilemapObject != null)
            tilemapObject.GetTilemapInformation(tileInfo);
        else
            Debug.Log("jan mencet luar map anjeng");
        
    }

    public Vector3Int GetTilemapLocation(Vector3 mousePosition)
    {
        TilemapObject tilemapObject = grid.GetGridObject(mousePosition);
        return tilemapObject.GetTilemapLocation();
    }

    public Grid<TilemapObject> GetGrid()
    {
        return grid;
    }
    public void Save(bool overwrite)
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

        SaveSystem.SaveObject("TileInfo/save",saveObject,overwrite);
    }


    public void Load()
    {
        SaveObject saveObject = SaveSystem.LoadMostRecentObject<SaveObject>();
        if(saveObject == null)
        {
            Debug.Log("No Save File Found");
        }
        else
        {
            foreach (TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
            {
                TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);

                tilemapObject.Load(tilemapObjectSaveObject);
            }
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }
        
    }

    public void Load(string SaveName)
    {
        SaveObject saveObject = SaveSystem.LoadObject<SaveObject>(SaveName);
        if (saveObject == null)
        {
            Debug.Log("No Save File Found");
        }
        else
        {
            foreach (TilemapObject.SaveObject tilemapObjectSaveObject in saveObject.tilemapObjectSaveObjectArray)
            {
                TilemapObject tilemapObject = grid.GetGridObject(tilemapObjectSaveObject.x, tilemapObjectSaveObject.y);

                tilemapObject.Load(tilemapObjectSaveObject);
            }
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
        public enum Deployability
        {
            Deployable,
            NotDeployable
        }

        public enum AttackDisplay
        {
            NotOnSight,
            OnSight,
            TheUnitItself,
            EnemyAttack
        }
        private Grid<TilemapObject> grid;
        public int x;
        public int y;
        public bool isBlocking;
        public bool artilleryShoot;
        public UnitGridCombat unitGridCombat;
        public UnitGridCombat.Unit unitType;
        public bool unitExist;
        public int unitHealth;
        public int unitOrder;
        public TilemapSprite tilemapSprite;
        public Deployability deployability;
        public AttackDisplay attackDisplay;

        public TilemapObject(Grid<TilemapObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            isBlocking = true;
            artilleryShoot = false;
        }

        public void SetisBlocking()
        {
            if (!unitGridCombat)
            {
                if (tilemapSprite == TilemapSprite.Forest || tilemapSprite == TilemapSprite.ForestDark || tilemapSprite == TilemapSprite.Mountain || tilemapSprite == TilemapSprite.MountainDark)
                {
                    this.isBlocking = true;
                    deployability = Deployability.NotDeployable;
                }
                else
                {
                    this.isBlocking = false;
                    deployability = Deployability.Deployable;
                }
            }
            else
            {
                this.isBlocking = true;
                deployability = Deployability.NotDeployable;
            }
            

            grid.TriggerGridObjectChanged(x,y);
        }

        public Deployability GetDeployability()
        {
            return deployability;
        }

        public void SetAttackDisplay(AttackDisplay attackDisplay)
        {
            this.attackDisplay = attackDisplay;

        }

        public AttackDisplay GetAttackDisplay()
        {
            return attackDisplay;
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

        public void GetTilemapInformation(TMP_Text tileInfo)
        {
            tileInfo.text = "x: " + x + "\n" +
                            "y: " + y + "\n" +
                            "isBlocking:" + isBlocking + "\n";

            if (unitExist)
            {
                tileInfo.text = tileInfo.text + "\n" +
                                "Unit type: " + unitType + "\n" +
                                "order: " + unitOrder + "\n" +
                                "health: " + unitHealth + "\n";
            }
            else
            {
                tileInfo.text = tileInfo.text + "\n" +
                                "no unit\n";
            }
            tileInfo.text = tileInfo.text + "\n" +
                            "deployability: " + deployability + "\n" +
                            "attack display: " + attackDisplay + "\n" +
                            "artillery shoot: " + artilleryShoot;
        }

        public Vector3Int GetTilemapLocation()
        {
            return new Vector3Int(x, y);
        }

        public Vector3 GetTilemapLocationFloat()
        {
            return new Vector3(x, y);
        }

        public void SetUnitGridCombat(UnitGridCombat unitGridCombat)
        {
            this.unitGridCombat = unitGridCombat;
            this.unitOrder = unitGridCombat.order;
            this.unitType = unitGridCombat.unitType;
            this.unitHealth = unitGridCombat.health;
            this.unitExist = true;
            SetisBlocking();
        }

        public void ClearUnitGridCombat()
        {
            unitGridCombat = null;

            this.unitOrder = 0;
            this.unitType = 0;
            this.unitHealth = 0;
            this.unitExist = false;
            SetisBlocking();
        }

        public UnitGridCombat GetUnitGridCombat() 
        { 
            return unitGridCombat; 
        }

        public override string ToString()
        {
            return tilemapSprite.ToString();
        }
        #region Saving
        [Serializable]
        public class SaveObject
        {
            public TilemapSprite tilemapSprite;
            public int x;
            public int y;
            public bool isBlocking;
            public UnitGridCombat.Unit unitType;
            public int unitHealth;
            public int unitOrder;
            public bool unitExist;
            public Deployability deployability;
        }

        public SaveObject Save()
        {
            return new SaveObject
            {
                tilemapSprite = tilemapSprite,
                x = x,
                y = y,
                isBlocking = isBlocking,
                unitType = unitType,
                unitHealth = unitHealth,
                unitOrder = unitOrder,
                unitExist = unitExist,
                deployability = deployability
                
            };
        }

        public void Load(SaveObject saveObject)
        {
            tilemapSprite = saveObject.tilemapSprite;
            isBlocking = saveObject.isBlocking;
            unitType = saveObject.unitType;
            unitHealth = saveObject.unitHealth;
            unitOrder = saveObject.unitOrder;
            unitExist = saveObject.unitExist;
            deployability = saveObject.deployability;
            grid.TriggerGridObjectChanged(x, y);
        }
        #endregion
    }
}
