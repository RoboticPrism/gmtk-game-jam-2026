using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GremlinEnemy : EnemyBase 
{
    public enum GremlinState { WANDER, CHASE, ATTACK }

    public GremlinState gremlinState;

    [SerializeField]
    [Tooltip("How far away the enemy switches to chase mode from")]
    private float agroRange;

    [SerializeField]
    [Tooltip("Sound the gremlin passively makes")]
    private AudioClip passiveClip;

    [SerializeField]
    [Tooltip("How far away you hear the enemy passively from")]
    private float passiveNoiseRange;

    [SerializeField]
    [Tooltip("Sound made when the gremlin detects the player")]
    private AudioClip agroClip;

    [SerializeField]
    private GameObject alert;

    [SerializeField]
    private GameObject confused;

    private bool lastTurnChased = false;

    public override void Start()
    {
        base.Start();
        alert.SetActive(false);
        confused.SetActive(false);
    }

    public override void DoTurn()
    {
        base.DoTurn();

        List<Vector3Int> pathToPlayer = GridManager.singleton.FindPath(gridLocation, Player.singleton.playerMovement.currentGridLocation);

        // If the next path to the player IS the player, attack
        if(pathToPlayer != null && pathToPlayer[0] == Player.singleton.playerMovement.currentGridLocation)
        {
            gremlinState = GremlinState.ATTACK;
            alert.SetActive(false);
            confused.SetActive(false);
        }
        // If the player is in range, and has a path to it
        else if(Vector3.Distance(gridLocation, Player.singleton.playerMovement.currentGridLocation) < agroRange && pathToPlayer != null)
        {
            if(gremlinState == GremlinState.WANDER)
            {
                audioSource.PlayOneShot(agroClip);
                alert.SetActive(true);
            }
            else
            {
                alert.SetActive(false);
                confused.SetActive(false);
            }
            gremlinState = GremlinState.CHASE;
        }
        else
        {
            if(gremlinState == GremlinState.CHASE)
            {
                confused.SetActive(true);
            }
            else
            {
                alert.SetActive(false);
                confused.SetActive(false);
            }
            gremlinState = GremlinState.WANDER;
        }


        if(gremlinState == GremlinState.WANDER)
        {
            DoWander();
        }
        else if(gremlinState == GremlinState.CHASE)
        {
            DoChase(pathToPlayer);
        }
        else
        {
            DoAttack(pathToPlayer[0]);
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
                gridLocation += Vector3Int.up;
                break;
            case EnemyDirections.DOWN:
                gridLocation += Vector3Int.down;
                break;
            case EnemyDirections.LEFT:
                gridLocation += Vector3Int.left;
                spriteRenderer.transform.localScale = Vector3.one;
                break;
            case EnemyDirections.RIGHT:
                gridLocation += Vector3Int.right;
                spriteRenderer.transform.localScale = new Vector3(-1,1,1);
                break;
        }
        visualTargetLocation = gridLocation;
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
            gridLocation = pathToPlayer[0];
            visualTargetLocation = gridLocation;
        }
    }

    public void DoAttack(Vector3Int attackLocation)
    {
        Bump(attackLocation);
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
