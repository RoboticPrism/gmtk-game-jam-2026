using UnityEngine;

public class GooberEnemy : EnemyBase 
{
    public enum MoveDirection { LEFTRIGHT, UPDOWN }

    [SerializeField]
    [Tooltip("Which axis the goober moves on")]
    private MoveDirection moveDirection;

    private bool forward = true;

    public override void DoTurn()
    {
        base.DoTurn();
        
        if(moveDirection == MoveDirection.LEFTRIGHT)
        {
            if(forward)
            {
                TryMoveToLocation(gridLocation + Vector3Int.right);
            }
            else
            {
                TryMoveToLocation(gridLocation + Vector3Int.left);
            }
        }
        else
        {
            if(forward)
            {
                TryMoveToLocation(gridLocation + Vector3Int.up);
            }
            else
            {
                TryMoveToLocation(gridLocation + Vector3Int.down);
            }
        }
    }

    private void TryMoveToLocation(Vector3Int location)
    {
        if (GridManager.singleton.CheckCollisionAtGridPoint(location))
        {
            // If we run into the player, bump
            if (Player.singleton.playerMovement.currentGridLocation == location)
            {
                Bump(location);
            }
            else
            {
                // flip direction
                forward = !forward;
            }
        }
        else
        {
            MoveToLocation(location);
        }
    }
}
