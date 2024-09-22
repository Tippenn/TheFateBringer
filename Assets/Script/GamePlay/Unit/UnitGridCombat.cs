using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TileMap;
using static UnityEngine.GraphicsBuffer;

public abstract class UnitGridCombat : MonoBehaviour
{
    public Team team;
    public int order;
    public int health;
    public Unit unitType;
    public bool isDead = false;
    public Direction facing;
    public TileMapTesting tilemapTesting;
    public AttackVisual attackVisual;
    public GridCombatSystem gridCombatSystem;
    public Animator animator;
    public HealthDisplay healthDisplay;
    public OrderDisplay orderDisplay;
    public AFKDisplay afkDisplay;
    public DeployDisplay deployDisplay;

    //public Grid<TileMap.TilemapObject> grid;

    public enum Direction
    {
        Left,Right, Up, Down
    }

    public enum Unit
    {
        //ally
        Cannon,
        Artillery,
        Flinger,
        Factory,
        Plasma,
        Decoy,
        //enemy
        Lasher,
        Spitter,
        Moleman,
        Bulk,
        Goop,
        Clot
        
    }
    public enum Team
    {
        Ally,
        Enemy
    }

    private void Awake()
    {
        //gabisa dipake soalnya ini gakecantol ke gameobject
    }

    private void Start()
    {
        //gabisa dipake soalnya ini gakecantol ke gameobject
    }

    
    public abstract void AssignUnit();
    public abstract void UnitSelected(Action isSelected);

    //animation
    public IEnumerator CheckForFacing()
    {
        //Debug.Log("Checking for facing");
        //ganti animasi
        if (facing == Direction.Down)
        {
            animator.SetInteger("VerticalFacing", -1);
            animator.SetInteger("HorizontalFacing", 0);
        }
        else if (facing == Direction.Up)
        {
            animator.SetInteger("VerticalFacing", 1);
            animator.SetInteger("HorizontalFacing", 0);
        }
        else if (facing == Direction.Right)
        {
            animator.SetInteger("VerticalFacing", 0);
            animator.SetInteger("HorizontalFacing", 1);
        }
        else
        {
            animator.SetInteger("VerticalFacing", 0);
            animator.SetInteger("HorizontalFacing", -1);
        }
        yield return new WaitForSeconds(1f);
    }
    public void BeginAttackAnimation()
    {
        animator.SetBool("isAttacking",true);
        animator.SetInteger("VerticalFacing", 0);
        animator.SetInteger("HorizontalFacing", 0);
    }
    public void StopAttackAnimation()
    {
        animator.SetBool("isAttacking", false);
        StartCoroutine(CheckForFacing());
    }
    public void AnimateAttackOnce()
    {
        animator.SetTrigger("Attacking");
    }

    //common function
    public IEnumerator MoveTowardsDirection(Direction moveDirection,Action onFinish)
    {
        Debug.Log("masuk6");
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int tempatasal = grid.GetXY(GetPosition());
        Vector2Int MovePosition = Vector2Int.zero;
        TileMap.TilemapObject tilemapObject;
        UnitGridCombat unitGridCombat;

        //kalkulasi posisi
        //kalo arahnya kiri
        if (moveDirection == Direction.Left)
        {
            //Debug.Log(moveDirection);
            for(int x = tempatasal.x-1; x >= -1; x--)
            {
                if (x < 0)
                {
                    MovePosition = new Vector2Int(0, tempatasal.y);
                    break;
                }
                tilemapObject = grid.GetGridObject(x, tempatasal.y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (tilemapObject.isBlocking == true)
                {
                    MovePosition = new Vector2Int(x+1,tempatasal.y);
                    break;
                }
                
            }
        }
        //kalo arahnya kanan
        else if (moveDirection == Direction.Right)
        {
            for (int x = tempatasal.x+1; x <= grid.GetWidth(); x++)
            {
                if (x >= grid.GetWidth())
                {
                    MovePosition = new Vector2Int(grid.GetWidth() - 1, tempatasal.y);
                    break;
                }
                tilemapObject = grid.GetGridObject(x, tempatasal.y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (tilemapObject.isBlocking == true )
                {
                    MovePosition = new Vector2Int(x - 1, tempatasal.y);
                    break;
                }
                
            }
        }
        //kalo arahnya atas
        else if (moveDirection == Direction.Up)
        {
            for (int y = tempatasal.y+1; y <= grid.GetHeight(); y++)
            {
                if (y >= grid.GetHeight())
                {
                    MovePosition = new Vector2Int(tempatasal.x, grid.GetHeight() - 1);
                    break;
                }
                tilemapObject = grid.GetGridObject(tempatasal.x, y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                //kalo kena block terrain
                if (tilemapObject.isBlocking == true)
                {
                    MovePosition = new Vector2Int(tempatasal.x, y-1);
                    break;
                }
                
            }
        }
        //kalo arahnya bawah
        else
        {
            for (int y = tempatasal.y-1; y >= -1; y--)
            {
                if (y < 0)
                {
                    MovePosition = new Vector2Int(tempatasal.x, 0);
                    break;
                }
                tilemapObject = grid.GetGridObject(tempatasal.x, y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (tilemapObject.isBlocking == true)
                {
                    MovePosition = new Vector2Int(tempatasal.x, y + 1);
                    break;
                }
                
            }
        }
        Debug.Log("masuk5");
        //Debug.Log(MovePosition);
        //pake function MoveTo
        yield return StartCoroutine(MoveTo(MovePosition, () => {
            onFinish?.Invoke();
        }));
    }
    public IEnumerator MoveTowardsTarget(UnitGridCombat target,Action onFinish)
    {
        Vector2Int targetPosition = target.GetXY(tilemapTesting);
        Vector2Int unitPosition = GetXY(tilemapTesting);
        //Debug.Log(targetPosition);
        //Debug.Log(unitPosition);
        Vector2Int MovePosition = Vector2Int.zero;
        
        //kalkulasi kemana movenya   
        //kiri
        if(targetPosition.x < unitPosition.x)
        {
            MovePosition = new Vector2Int(targetPosition.x + 1,targetPosition.y);
        }
        //kanan
        else if(targetPosition.x > unitPosition.x)
        {
            MovePosition = new Vector2Int(targetPosition.x - 1, targetPosition.y);
        }
        //atas
        else if(targetPosition.y > unitPosition.y)
        {
            MovePosition = new Vector2Int(targetPosition.x, targetPosition.y - 1);
        }
        //bawah
        else if(targetPosition.y < unitPosition.y)
        {
            MovePosition = new Vector2Int(targetPosition.x, targetPosition.y + 1);
        }

        //pakek function Move To
        yield return StartCoroutine(MoveTo(MovePosition, () => {
            onFinish?.Invoke();
        }));
    }
    public IEnumerator MoveTo(Vector2Int gridposition,Action onFinish)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int tempatasal = grid.GetXY(GetPosition());
        Vector2Int tempattujuan = gridposition;

        // cek tempatnya rill apa fakeh
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(gridposition.x, gridposition.y);
        if(tilemapObject != null)
        {
            //pindahin tempatnya
            Vector3 position = grid.GetWorldPosition(tempattujuan.x, tempattujuan.y);
            position.x = position.x + grid.GetCellSize() / 2;
            position.y = position.y + grid.GetCellSize() / 2;
            position.z = -3f;
            //Debug.Log(transform.position + " | " + position);
            float elapsedTime = 0f;
            float time = 0.25f;
            while (elapsedTime < time)
            {
                // Lerp between the start position and target position
                transform.position = Vector3.Lerp(transform.position, position, elapsedTime / time);
                elapsedTime += Time.deltaTime;

                // Wait for the next frame before continuing the loop
                yield return null;
            }

            //pindahin informasi di gridnya
            grid.GetGridObject(tempatasal.x, tempatasal.y).ClearUnitGridCombat();
            grid.GetGridObject(tempattujuan.x, tempattujuan.y).SetUnitGridCombat(this);
        }
        
        onFinish?.Invoke();
              
    }
    public void TeleportTo(Vector2Int gridposition, Action onFinish)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int tempatasal = grid.GetXY(GetPosition());
        Vector2Int tempattujuan = gridposition;

        // cek tempatnya rill apa fakeh
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(gridposition.x, gridposition.y);
        if (tilemapObject != null)
        {
            //pindahin tempatnya
            Vector3 position = grid.GetWorldPosition(tempattujuan.x, tempattujuan.y);
            position.x = position.x + grid.GetCellSize() / 2;
            position.y = position.y + grid.GetCellSize() / 2;
            position.z = -3f;
            
            transform.position = position;

            //pindahin informasi di gridnya
            grid.GetGridObject(tempatasal.x, tempatasal.y).ClearUnitGridCombat();
            grid.GetGridObject(tempattujuan.x, tempattujuan.y).SetUnitGridCombat(this);
        }

        onFinish?.Invoke();
    }
    public void TakeDamage(int damage)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.unitDamaged);
        health -= damage;
        GiantBaseScript giantBaseScript = this as GiantBaseScript;
        if(giantBaseScript != null)
        {
            giantBaseScript.waitingTime = 0;
        }
        CheckForHealth();
        UpdateDisplay();
    }
    public void CheckForHealth()
    {
        if (health == 0)
        {
            ObjectisDead();           
        }
    }
    public void ObjectisDead()
    {
        //unregister dari tile
        tilemapTesting.tilemap.GetGrid().GetGridObject(GetPosition()).ClearUnitGridCombat();

        //biar ga keliatan
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        //health sama ordernya juga
        spriteRenderer = transform.Find("HealthDisplay").GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        spriteRenderer = transform.Find("OrderDisplay").GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        if(team == Team.Enemy)
        {
            spriteRenderer = transform.Find("AFKDisplay").GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlaySFX(audioManager.monsterDeath);
        }
        else
        {
            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlaySFX(audioManager.robotDeath);
        }

        //unregisted dari list gridcombatsystemnya
        gridCombatSystem.unitToRemoveList.Add(this);

        //objeknya mati
        isDead = true;
    }

    public void SnapInPlace(Vector3 position)
    {
        position.z = -3f;
        transform.position = position;
        
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public Vector2Int GetXY(TileMapTesting tileMapTesting)
    {
        Grid<TileMap.TilemapObject> grid = tileMapTesting.GetGrid();
        return grid.GetXY(GetPosition());
    }
    public Team GetTeam()
    {
        return team;
    }
    public void GetAllComponent()
    {
        tilemapTesting = GameObject.Find("TileMapTesting").GetComponent<TileMapTesting>();
        attackVisual = GameObject.Find("AttackVisual").GetComponent<AttackVisual>();
        gridCombatSystem = GameObject.Find("GridCombatSystem").GetComponent<GridCombatSystem>();
        animator = GetComponent<Animator>();
        healthDisplay = GetComponentInChildren<HealthDisplay>();
        if(team == Team.Enemy)
        {
            orderDisplay = GetComponentInChildren<OrderDisplay>();
            afkDisplay = GetComponentInChildren<AFKDisplay>();
        }
        else
        {
            deployDisplay = GetComponentInChildren<DeployDisplay>();
        }
        
    }
    public void makingsure()
    {
        //Debug.Log("MakingSure");
        //if (tilemapTesting)
        //{
        //    Debug.Log("tilemaptesting ada kok");
        //}
        //if (attackVisual)
        //{
        //    Debug.Log("attackVisual ada kok");
        //}
        //if (gridCombatSystem)
        //{
        //    Debug.Log("gridcombatsystem ada kok");
        //}
    }

    public void UpdateDisplay()
    {
        healthDisplay.UpdateNumber(health);
        if (team == Team.Enemy)
        {
            GiantBaseScript giantBaseScript = this as GiantBaseScript;
            orderDisplay.UpdateNumber(order);
            afkDisplay.UpdateNumber(giantBaseScript.waitingTime);
        }
        else
        {
            RobotBaseScript robotBaseScript = this as RobotBaseScript;
            
            deployDisplay.UpdateNumber(robotBaseScript.deployNumber);
        }
    }
}
