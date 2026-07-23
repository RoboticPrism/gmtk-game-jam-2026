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
    private SpriteRenderer teleportRenderer;

    [SerializeField]
    private float teleportAuraRadio;

    private bool teleportAnimating = false;

    [SerializeField]
    private int teleportSpriteLayer;

    [SerializeField]
    private int teleportSpriteAnimationLayer;

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
        teleportRenderer.sortingOrder = teleportSpriteLayer;
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

        if (!teleportAnimating)
        {
            SetTeleportSize();
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

    public void SetTeleportSize()
    {
        teleportRenderer.transform.localScale = Player.singleton.playerMovement.currentTeleportChannel * teleportAuraRadio * Vector3.one;
    }

    public void AnimateTeleport()
    {
        StartCoroutine(TeleportAnimation());
    }

    IEnumerator TeleportAnimation() 
    {
        teleportAnimating = true;
        teleportRenderer.sortingOrder = teleportSpriteAnimationLayer;

        float currentTime = 0f;
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            teleportRenderer.transform.localScale = Mathf.Lerp(0f, 2f, currentTime / 2f) * teleportAuraRadio * Vector3.one;
            yield return null;
        }

        teleportRenderer.sortingOrder = teleportSpriteLayer;
        ShowPlayer();
        
        currentTime = 0f;
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            teleportRenderer.transform.localScale = Mathf.Lerp(2f, 0f, currentTime / 2f) * teleportAuraRadio * Vector3.one;
            yield return null;
        }
        teleportAnimating = false;
    }
}
