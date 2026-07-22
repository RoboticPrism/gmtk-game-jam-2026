using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
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

    private bool isMoveOnCooldown = false;

    [SerializeField]
    [Tooltip("The tilemap we need to collision check against")]
    private Tilemap tilemap;

    [SerializeField]
    [Tooltip("The player's current grid location to move to")]
    private Vector3Int currentGridLocation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controls = new InputSystem_Actions();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards target location
        transform.position = Vector3.Lerp(transform.position, currentGridLocation, moveSpeed);

        // Check Inputs
        CheckMove();
    }


    private void CheckMove()
    {
        if(!isMoveOnCooldown)
        {
            // If moving right
            if(moveInput.x > deadzone && moveInput.x > moveInput.y)
            {
                if (!CheckCollisionAtLocation(currentGridLocation + Vector3Int.right))
                {
                    currentGridLocation += Vector3Int.right;
                    DoMoveCooldown();
                }
            }
            // if moving left
            if(moveInput.x < -deadzone && moveInput.x < moveInput.y)
            {
                if (!CheckCollisionAtLocation(currentGridLocation + Vector3Int.left))
                {
                    currentGridLocation += Vector3Int.left;
                    DoMoveCooldown();
                }
            }
            // If moving up
            if(moveInput.y > deadzone && moveInput.y > moveInput.x)
            {
                if (!CheckCollisionAtLocation(currentGridLocation + Vector3Int.up))
                {
                    currentGridLocation += Vector3Int.up;
                    DoMoveCooldown();
                }
            }
            // If moving down
            if(moveInput.y < -deadzone && moveInput.y < moveInput.x)
            {
                if (!CheckCollisionAtLocation(currentGridLocation + Vector3Int.down))
                {
                    currentGridLocation += Vector3Int.down;
                    DoMoveCooldown();
                }
            }
        }
    }

    private bool CheckCollisionAtLocation(Vector3Int location)
    {
        return tilemap.GetTile(location) != null;
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
}
