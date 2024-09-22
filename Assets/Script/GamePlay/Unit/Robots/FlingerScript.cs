using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileMap;

public class FlingerScript : RobotBaseScript
{
    public TargetAxis targetAxis;
    public Condition condition;
    public UnitGridCombat target;
    public enum Condition
    {
        Normal,
        TargetSelected
    }

    public enum TargetAxis
    {
        Horizontal,
        Vertical
    }

    private void Awake()
    {
        GetAllComponent();
        AssignUnit();
    }
    private void Start()
    {
        UpdateDisplay();
    }

    public override void AssignUnit()
    {
        StartCoroutine(CheckForFacing());
        team = Team.Ally;
        base.unitType = Unit.Flinger;
        base.health = 2;
        deployNumber = gridCombatSystem.totalRobotDeployed;
        JustDeployed = true;
        attackPhase = 2;
    }

    public override void UnitSelected(Action isSelected)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.unitOnClick);
        showFlingerVisual();
        isSelected?.Invoke();
    }

    public override IEnumerator UseUtility(Vector2 FiringDirection,Action onUse)
    {
        yield return null;
        //ngambil info
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(Mathf.RoundToInt(FiringDirection.x), Mathf.RoundToInt(FiringDirection.y));
        UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
        //kalo ada targetnya
        if(unitGridCombat != null)
        {
            //set target + axisnya
            target = unitGridCombat;
            targetAxis = DetermineTargetAxis(target);
            StartCoroutine(CheckForFacing());

            //update visualnya jadi yang selected
            condition = Condition.TargetSelected;
            showFlingerVisual();
            attackVisual.ShowHeatMapVisual();            
        }
        //kalo gaada targetnya
        else
        {
            //ga ngapa"in sebenarnya wokwokwok

            //update visualnya jadi yang selected
            condition = Condition.TargetSelected;
            showFlingerVisual();
            attackVisual.ShowHeatMapVisual();
        }
        onUse?.Invoke();
    }

    public IEnumerator UseSecondUtility(Vector2 FiringDirection, Action onUse)
    {
        attackVisual.HideHeatMapVisual();
        AnimateAttackOnce();
        yield return new WaitForSeconds(1);
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.flinger);
        //ambil data yang neccesary
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        //cari tile tujuan
        TileMap.TilemapObject tilemapObjectDesignation = grid.GetGridObject(Mathf.RoundToInt(FiringDirection.x), Mathf.RoundToInt(FiringDirection.y));

        //kalo target exist
        if (target)
        {
            yield return StartCoroutine(FlingUnit(target, new Vector2Int(Mathf.RoundToInt(FiringDirection.x), Mathf.RoundToInt(FiringDirection.y))));
        }
        //kalo target ga exist ya gangaruh wir kan
        else
        {
            //gangaruh wirzz
        }
        condition = Condition.Normal;        
        onUse?.Invoke();
    }

    public TargetAxis DetermineTargetAxis(UnitGridCombat target)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());
        Vector2Int targetPosition = grid.GetXY(target.GetPosition());

        if(unitPosition.x == targetPosition.x)
        {
            if(unitPosition.y > targetPosition.y)
            {
                facing = Direction.Down;
            }
            else
            {
                facing = Direction.Up;
            }
            return TargetAxis.Vertical;
        }
        else
        {
            if (unitPosition.x > targetPosition.x)
            {
                facing = Direction.Left;
            }
            else
            {
                facing = Direction.Right;
            }
            return TargetAxis.Horizontal;
        }       
    }

    public void showFlingerVisual()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());
        
        int unitX = unitPosition.x;
        int unitY = unitPosition.y;

        // bersihin semua dulu
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
            }
        }

        if(condition == Condition.Normal)
        {
            
            TileMap.TilemapObject tilemapObject;
            //diri sendiri
            tilemapObject = grid.GetGridObject(unitX, unitY);
            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
            //ke kiri
            tilemapObject = grid.GetGridObject(unitX - 1, unitY);
            //kalo tilenya exist(kalo diujung ga exist)
            if(tilemapObject != null)
            {
                //kalo ngeblock
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                }
                else
                {
                    //kalo ga ngeblock
                    tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                }
            }
            
            //ke kanan
            tilemapObject = grid.GetGridObject(unitX + 1, unitY);
            //kalo tilenya exist(kalo diujung ga exist)
            if (tilemapObject != null)
            {
                //kalo ngeblock
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                }
                else
                {
                    //kalo ga ngeblock
                    tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                }
            }
            
            //ke atas
            tilemapObject = grid.GetGridObject(unitX, unitY + 1);
            //kalo tilenya exist(kalo diujung ga exist)
            if (tilemapObject != null)
            {
                //kalo ngeblock
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                }
                else
                {
                    //kalo ga ngeblock
                    tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                }
            }
            
            //ke bawah
            tilemapObject = grid.GetGridObject(unitX, unitY - 1);
            //kalo tilenya exist(kalo diujung ga exist)
            if (tilemapObject != null)
            {
                //kalo ngeblock
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                }
                else
                {
                    //kalo ga ngeblock
                    tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                }
            }
            
        }
        else
        {           
            //kalo targetnya horizontal
            if (targetAxis == TargetAxis.Horizontal)
            {
                for(int x = 0; x < grid.GetWidth(); x++)
                {
                    TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, unitY);
                    //tilenya diri itu sendiri
                    if (x == unitX)
                    {                       
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                    }
                    //tilenya bukan diri itu sendiri
                    else
                    {
                        if (tilemapObject.isBlocking)
                        {
                            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                        }
                        else
                        {
                            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                        }
                        
                    }
                }
            }
            //kalo targetnya vertikal
            else
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    TileMap.TilemapObject tilemapObject = grid.GetGridObject(unitX, y);
                    //tilenya diri itu sendiri
                    if (y == unitY)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                    }
                    //tilenya bukan diri itu sendiri
                    else
                    {
                        if (tilemapObject.isBlocking)
                        {
                            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                        }
                        else
                        {
                            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                        }
                    }
                }
            }
        }      

        attackVisual.ShowHeatMapVisual();
        
    }

    public IEnumerator FlingUnit(UnitGridCombat target,Vector2Int designantedLocation)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int tempatasal = grid.GetXY(target.GetPosition());
        Vector2Int tempattujuan = designantedLocation;

        // cek tempatnya rill apa fakeh
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(tempattujuan.x, tempattujuan.y);
        if (tilemapObject != null)
        {
            //pindahin tempatnya
            Vector3 position = grid.GetWorldPosition(tempattujuan.x, tempattujuan.y);
            position.x = position.x + grid.GetCellSize() / 2;
            position.y = position.y + grid.GetCellSize() / 2;
            position.z = -3f;

            target.transform.position = target.transform.position + new Vector3(0f, 0f, -2f);
            if (targetAxis == TargetAxis.Horizontal)
            {
                Vector3 LinearStart = target.GetPosition();
                Vector3 LinearEnd = position;
                Vector3 flingStart = target.GetPosition();
                Vector3 flingPeak =new Vector3(0f, 12f, 0f);
                //Debug.Log(transform.position + " | " + position);
                float elapsedTime = 0f;
                float time = 0.5f;

                while (elapsedTime <= time)
                {
                    float t = elapsedTime / time;

                    // Horizontal movement from A to B over full duration
                    Vector3 linearPosition = Vector3.Lerp(LinearStart, LinearEnd, t);

                    // Vertical movement with a parabolic path
                    float flingPosition = flingStart.y + Mathf.Sin(t * Mathf.PI) * flingPeak.y;

                    // Combine both horizontal and vertical movements
                    target.transform.position = new Vector3(linearPosition.x, flingPosition, target.transform.position.z);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                Vector3 LinearStart = target.GetPosition();
                Vector3 LinearEnd = position;
                Vector3 flingStart = target.GetPosition();
                Vector3 flingPeak =new Vector3(3f, 0f, 0f);
                //float flingPeak = 4f;
                //Debug.Log(transform.position + " | " + position);
                float elapsedTime = 0f;
                float time = 0.5f;

                while (elapsedTime <= time)
                {
                    float t = elapsedTime / time;

                    // Horizontal movement from A to B over full duration
                    Vector3 linearPosition = Vector3.Lerp(LinearStart, LinearEnd, t);

                    float flingPosition = flingStart.x + Mathf.Sin(t * Mathf.PI) * flingPeak.x;

                    // Combine both horizontal and vertical movements
                    target.transform.position = new Vector3(flingPosition, linearPosition.y, target.transform.position.z);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            target.transform.position = target.transform.position + new Vector3(0f, 0f, 2f);

            //pindahin informasi di gridnya
            grid.GetGridObject(tempatasal.x, tempatasal.y).ClearUnitGridCombat();
            grid.GetGridObject(tempattujuan.x, tempattujuan.y).SetUnitGridCombat(target);
        }
    }
}
