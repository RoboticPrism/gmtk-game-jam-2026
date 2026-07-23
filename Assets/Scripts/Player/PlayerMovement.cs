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

    [SerializeField]
    [Tooltip("How long teleport has to be held to teleport")]
    private float teleportSeconds;

    [SerializeField]
    public float currentTeleportChannel = 0;

    [SerializeField]
    private bool isHoldingTeleport = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new InputSystem_Actions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Jump.started += ctx => isHoldingTeleport = true;
        controls.Player.Jump.canceled += ctx => isHoldingTeleport = false;

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
        CheckTeleport();
    }


    private void CheckMove()
    {
        if(!isMoveOnCooldown)
        {
            Vector3Int targetLocation;
            MoveDirection moveDirection;

            // If moving right
            if (moveInput.x > deadzone && moveInput.x > moveInput.y)
            {
                targetLocation = currentGridLocation + Vector3Int.right;
                moveDirection = MoveDirection.RIGHT;
            }
            // If moving left
            else if(moveInput.x < -deadzone && moveInput.x < moveInput.y)
            {
                targetLocation = currentGridLocation + Vector3Int.left;
                moveDirection = MoveDirection.LEFT;
            }
            // If moving up
            else if(moveInput.y > deadzone && moveInput.y > moveInput.x)
            {
                targetLocation = currentGridLocation + Vector3Int.up;
                moveDirection = MoveDirection.UP;
            }
            // If moving down
            else if(moveInput.y < -deadzone && moveInput.y < moveInput.x)
            {
                targetLocation = currentGridLocation + Vector3Int.down;
                moveDirection = MoveDirection.DOWN;
            }
            else
            {
                return;
            }

            if(!GridManager.singleton.CheckCollisionAtGridPoint(targetLocation))
            {
                UpdatePosition(targetLocation);
                DoMoveCooldown();
                EnemyManager.singleton.DoEnemyTurns();
                Player.singleton.playerAnimator.SetMoveDirection(moveDirection);
            }
            else if (GridManager.singleton.GetBumpableAtGridPoint(targetLocation) != null)
            {
                BumpBumpableAtLocation(targetLocation);
                EnemyManager.singleton.DoEnemyTurns();
            }
        }
    }

    private void CheckTeleport()
    {
        if(isHoldingTeleport)
        {
            currentTeleportChannel += Time.deltaTime;
            if(currentTeleportChannel > teleportSeconds)
            {
                StartCoroutine(TeleportAnimation());
                currentTeleportChannel = teleportSeconds-0.01f;
                isHoldingTeleport = false;
            }
        }
        else
        {
            if (currentTeleportChannel > 0)
            {
                currentTeleportChannel -= Time.deltaTime;
                if(currentTeleportChannel < 0)
                {
                    currentTeleportChannel = 0;
                }
            }
        }
    }

    IEnumerator TeleportAnimation()
    {
        controls.Disable();

        ScreenEffectManager.singleton.FadeOut();

        Player.singleton.playerAnimator.HidePlayer();

        yield return new WaitForSeconds(2f);

        // Teleport to below the pyre
        currentGridLocation = GridManager.singleton.resourceTilemap.WorldToCell(FindAnyObjectByType<Pyre>().transform.position + Vector3Int.down);
        visualTargetLocation = currentGridLocation;

        yield return new WaitForSeconds(1f);

        ScreenEffectManager.singleton.FadeIn();

        Player.singleton.playerAnimator.AnimateTeleport();

        yield return new WaitForSeconds(2f);

        controls.Enable();
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

    private void BumpBumpableAtLocation(Vector3Int location)
    {
        GridManager.singleton.GetBumpableAtGridPoint(location).OnBump();
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
