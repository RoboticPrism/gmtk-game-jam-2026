using UnityEngine;
using System.Collections;

public enum MoveDirection { IDLE, UP, DOWN, LEFT, RIGHT }

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite idle;

    [SerializeField]
    private Sprite up;

    [SerializeField]
    private Sprite down;

    [SerializeField]
    private Sprite left;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    [Tooltip("How long after movement to return to idle")]
    private float returnToIdleSeconds;

    [SerializeField]
    public MoveDirection moveDirection;

    [SerializeField]
    private AudioClip stepClip;

    private AudioSource audioSource;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.transform.localScale = Vector3.one;
        switch(moveDirection)
        {
            case MoveDirection.IDLE:
                spriteRenderer.sprite = idle;
                break;
            case MoveDirection.UP:
                spriteRenderer.sprite = up;
                break;
            case MoveDirection.DOWN:
                spriteRenderer.sprite = down;
                break;
            case MoveDirection.LEFT:
                spriteRenderer.sprite = left;
                break;
            case MoveDirection.RIGHT:
                spriteRenderer.sprite = left;
                spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
                break;
        } 
    }

    public void SetMoveDirection(MoveDirection moveDirection)
    {
        this.moveDirection = moveDirection;
        audioSource.PlayOneShot(stepClip);

        // Restart the idle cooldown
        if (idleCooldown != null)
        {
            StopCoroutine(idleCooldown);
        }
        idleCooldown = StartCoroutine(IdleCooldown());
    }

    private Coroutine idleCooldown;
    IEnumerator IdleCooldown()
    {
        yield return new WaitForSeconds(returnToIdleSeconds);
        moveDirection = MoveDirection.IDLE;
    } 

    public void HidePlayer()
    {
        spriteRenderer.gameObject.SetActive(false);
    }

    public void ShowPlayer()
    {
        spriteRenderer.gameObject.SetActive(true);
    }
}
