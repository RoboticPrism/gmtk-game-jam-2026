using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions controls;
    [SerializeField]
    private Vector2 moveInput;

    [SerializeField]
    [Tooltip("How fast the player moves towards its target grid tile")]
    private float moveSpeed;

    [SerializeField]
    [Tooltip("Deadzone before considering a direction a move.")]
    private float deadzone = 0.1f;

    [SerializeField]
    [Tooltip("The cooldown in seconds between moving.")]
    private float moveCooldownSeconds;

    [SerializeField]
    [Tooltip("The cooldown in seconds between bumping.")]
    private float bumpCooldownSeconds;

    [SerializeField]
    private bool isMoveOnCooldown = false;

    [SerializeField]
    [Tooltip("The player's current grid location to move to")]
    public Vector3Int currentGridLocation;

    private Vector3 visualTargetLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new InputSystem_Actions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Enable();

        visualTargetLocation = currentGridLocation;
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards target location
        transform.position = Vector3.Lerp(transform.position, visualTargetLocation, moveSpeed);

        // Check Inputs
        CheckMove();
    }


    private void CheckMove()
    {
        if(!isMoveOnCooldown)
        {
            Vector3Int targetLocation;

            // If moving right
            if (moveInput.x > deadzone && moveInput.x > moveInput.y)
            {
                targetLocation = currentGridLocation + Vector3Int.right;
                Player.singleton.playerAnimator.SetMoveDirection(MoveDirection.RIGHT);
            }
            // If moving left
            else if(moveInput.x < -deadzone && moveInput.x < moveInput.y)
            {
                targetLocation = currentGridLocation + Vector3Int.left;
                Player.singleton.playerAnimator.SetMoveDirection(MoveDirection.LEFT);
            }
            // If moving up
            else if(moveInput.y > deadzone && moveInput.y > moveInput.x)
            {
                targetLocation = currentGridLocation + Vector3Int.up;
                Player.singleton.playerAnimator.SetMoveDirection(MoveDirection.UP);
            }
            // If moving down
            else if(moveInput.y < -deadzone && moveInput.y < moveInput.x)
            {
                targetLocation = currentGridLocation + Vector3Int.down;
                Player.singleton.playerAnimator.SetMoveDirection(MoveDirection.DOWN);
            }
            else
            {
                return;
            }

            if(!CheckCollisionAtLocation(targetLocation))
            {
                UpdatePosition(targetLocation);
                DoMoveCooldown();
            }
            else if (CheckBumpableAtLocation(targetLocation))
            {
                BumpBumpableAtLocation(targetLocation);
            }
        }
    }

    private void UpdatePosition(Vector3Int targetPosition)
    {
        currentGridLocation = targetPosition;
        visualTargetLocation = targetPosition;
        FogOfWarManager.TriggerLightingUpdate();
        CounterManager.singleton.UseStep();
    }

    private Coroutine moveCoroutine;
    private void DoMoveCooldown() {
        isMoveOnCooldown = true;
        moveCoroutine = StartCoroutine(MoveCooldown());
    }

    IEnumerator MoveCooldown()
    {
        yield return new WaitForSeconds(moveCooldownSeconds);
        isMoveOnCooldown = false;
    }

    private bool CheckCollisionAtLocation(Vector3Int location)
    {
        return GridManager.singleton.resourceTilemap.GetTile(location) != null;
    }

    private bool CheckBumpableAtLocation(Vector3Int location)
    {
        return GridManager.singleton.resourceTilemap.GetInstantiatedObject(location)?.GetComponent<BumpableTile>() != null;
    }

    private void BumpBumpableAtLocation(Vector3Int location)
    {
        GridManager.singleton.resourceTilemap.GetInstantiatedObject(location).GetComponent<BumpableTile>().OnBump();
        DoBumpCooldown(location);
        CounterManager.singleton.UseStep();
    }

    private Coroutine bumpCoroutine;
    private void DoBumpCooldown(Vector3Int location)
    {
        isMoveOnCooldown = true;
        bumpCoroutine = StartCoroutine(Bump(location));
    }

    IEnumerator Bump(Vector3Int location)
    {
        visualTargetLocation = Vector3.Lerp(location, currentGridLocation, 0.5f);
        yield return new WaitForSeconds(bumpCooldownSeconds/2f);
        visualTargetLocation = currentGridLocation;
        yield return new WaitForSeconds(bumpCooldownSeconds/2f);
        isMoveOnCooldown = false;
    }
}
