using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerDefenseGremlin : EnemyBase
{
    public enum GremlinState { WANDER, CHASE, ATTACK }

    public GremlinState gremlinState;

    [SerializeField]
    [Tooltip("Sound the gremlin passively makes")]
    private AudioClip passiveClip;

    [SerializeField]
    [Tooltip("How far away you hear the enemy passively from")]
    private float passiveNoiseRange;

    [SerializeField]
    [Tooltip("Sound made when the gremlin spawns")]
    private AudioClip agroClip;

    private bool lastTurnChased = false;

    private List<PyreAttackPoint> attackPoints;

    public override void Start()
    {
        base.Start();
        audioSource.PlayOneShot(agroClip);
        attackPoints = new List<PyreAttackPoint>(FindObjectsByType<PyreAttackPoint>());
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        TowerDefenseManager.singleton.OnGremlinDefeat(this);
    }

    public override void DoTurn()
    {
        base.DoTurn();

        // Find the nearest corner of the pyre to go chew on
        List<Vector3Int> pathToPyre = null;
        foreach (PyreAttackPoint attackPoint in attackPoints)
        {
            List<Vector3Int> potetentialpathToPyre = GridManager.singleton.FindPath(gridLocation, attackPoint.gridLocation); 
            if(pathToPyre == null)
            {
                pathToPyre = potetentialpathToPyre;
            }
            else if(pathToPyre != null && potetentialpathToPyre.Count < pathToPyre.Count)
            {
                pathToPyre = potetentialpathToPyre;
            }
        }

        // If the next path to the pyre IS the pyre, attack
        if(pathToPyre != null && pathToPyre.Count == 1)
        {
            gremlinState = GremlinState.ATTACK;
        }
        // If the pyre is in range, and has a path to it
        else if(pathToPyre != null)
        {
            gremlinState = GremlinState.CHASE;
        }
        else
        {
            gremlinState = GremlinState.WANDER;
        }


        if(gremlinState == GremlinState.WANDER)
        {
            DoWander();
        }
        else if(gremlinState == GremlinState.CHASE)
        {
            DoChase(pathToPyre);
        }
        else
        {
            DoAttack(GridManager.singleton.resourceTilemap.WorldToCell(Pyre.singleton.transform.position));
        }
    }

    private void DoWander()
    {
        List<EnemyDirections> validDirections = new List<EnemyDirections>();

        // Build up valid move directions
        if(!GridManager.singleton.CheckCollisionAtGridPoint(gridLocation + Vector3Int.up))
        {
            validDirections.Add(EnemyDirections.UP);
        }
        if(!GridManager.singleton.CheckCollisionAtGridPoint(gridLocation + Vector3Int.down))
        {
            validDirections.Add(EnemyDirections.DOWN);
        }
        if(!GridManager.singleton.CheckCollisionAtGridPoint(gridLocation + Vector3Int.left))
        {
            validDirections.Add(EnemyDirections.LEFT);
        }
        if(!GridManager.singleton.CheckCollisionAtGridPoint(gridLocation + Vector3Int.right))
        {
            validDirections.Add(EnemyDirections.RIGHT);
        }

        EnemyDirections chosenDirection = validDirections[Random.Range(0, validDirections.Count)];

        switch(chosenDirection)
        {
            case EnemyDirections.UP:
                MoveToLocation(gridLocation + Vector3Int.up);
                break;
            case EnemyDirections.DOWN:
                MoveToLocation(gridLocation + Vector3Int.down);
                break;
            case EnemyDirections.LEFT:
                MoveToLocation(gridLocation + Vector3Int.left);
                spriteRenderer.transform.localScale = Vector3.one;
                break;
            case EnemyDirections.RIGHT:
                MoveToLocation(gridLocation + Vector3Int.right);
                spriteRenderer.transform.localScale = new Vector3(-1,1,1);
                break;
        }
    }

    public void DoChase(List<Vector3Int> pathToPlayer)
    {
        // If there's no available path, just wander
        if(pathToPlayer == null)
        {
            gremlinState = GremlinState.WANDER;
            DoWander();
        }
        else
        {
            // Move every other turn
            if(lastTurnChased)
            {
                lastTurnChased = false;
                return;
            }
            lastTurnChased = true;

            // Swap sprite to movement direction
            if(pathToPlayer[0] == gridLocation + Vector3Int.left)
            {
                spriteRenderer.transform.localScale = Vector3.one;
            }
            if(pathToPlayer[0] == gridLocation + Vector3Int.right)
            {
                spriteRenderer.transform.localScale = new Vector3(-1,1,1);
            }
            MoveToLocation(pathToPlayer[0]);
        }
    }

    public void DoAttack(Vector3Int attackLocation)
    {
        Bump(attackLocation);
        TowerDefenseManager.singleton.DamagePyre();
    }

    public override void Update()
    {
        base.Update();

        if(!audioSource.isPlaying && !isOnPassiveCooldown && Vector3.Distance(gridLocation, Player.singleton.playerMovement.currentGridLocation) < passiveNoiseRange)
        {
            audioSource.pitch = Random.Range(0.5f, 1.5f);
            audioSource.PlayOneShot(passiveClip);
            isOnPassiveCooldown = true;
            StartCoroutine(PassiveCooldown());
        }
    }

    private bool isOnPassiveCooldown = false;
    IEnumerator PassiveCooldown()
    {
        yield return new WaitForSeconds(Random.Range(0f, 1.5f));
        isOnPassiveCooldown = false;
    }

}
