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
    private Sprite upAlt;

    [SerializeField]
    private Sprite down;

    [SerializeField]
    private Sprite downAlt;

    [SerializeField]
    private Sprite left;

    [SerializeField]
    private Sprite leftAlt;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private SpriteRenderer teleportRenderer;

    [SerializeField]
    private GameObject slashObject;

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

    [SerializeField]
    private AudioSource teleportSource;

    private AudioSource audioSource;

    private bool altSprite = false;

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
                if (altSprite)
                {
                    spriteRenderer.sprite = upAlt;
                }
                else
                {
                    spriteRenderer.sprite = up;
                }
                break;
            case MoveDirection.DOWN:
                if (altSprite)
                {
                    spriteRenderer.sprite = downAlt;
                }
                else
                {
                    spriteRenderer.sprite = down;
                }
                break;
            case MoveDirection.LEFT:
                if (altSprite)
                {
                    spriteRenderer.sprite = leftAlt;
                }
                else
                {
                    spriteRenderer.sprite = left;
                }
                break;
            case MoveDirection.RIGHT:
                if (altSprite)
                {
                    spriteRenderer.sprite = leftAlt;
                }
                else
                {
                    spriteRenderer.sprite = left;
                }
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
        altSprite = !altSprite;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
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

    public void ShowSlash(MoveDirection moveDirection)
    {
        slashObject.SetActive(true);
        switch(moveDirection)
        {
            case MoveDirection.UP:
                slashObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case MoveDirection.DOWN:
                slashObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDirection.LEFT:
                slashObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case MoveDirection.RIGHT:
                slashObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }

    public void HideSlash()
    {
        slashObject.SetActive(false);
    }

    public void SetTeleportSize()
    {
        if (Player.singleton.playerMovement.currentTeleportChannel > 0)
        {
            teleportSource.volume = 1;
            teleportSource.pitch = 1f + (Player.singleton.playerMovement.currentTeleportChannel / 4f);
        }
        else
        {
            teleportSource.volume = 0;
        }
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
        teleportSource.volume = 1f;
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            teleportRenderer.transform.localScale = Mathf.Lerp(0f, 2f, currentTime / 2f) * teleportAuraRadio * Vector3.one;
            teleportSource.pitch = 1f + (currentTime / 4f);
            
            yield return null;
        }

        teleportRenderer.sortingOrder = teleportSpriteLayer;
        ShowPlayer();
        
        currentTime = 0f;
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            teleportRenderer.transform.localScale = Mathf.Lerp(2f, 0f, currentTime / 2f) * teleportAuraRadio * Vector3.one;
            teleportSource.pitch = 1.5f - (currentTime / 4f);
            yield return null;
        }
        teleportAnimating = false;
        teleportSource.volume = 0f;
    }

    public void AnimateForcedTeleport()
    {
        StartCoroutine(ForcedTeleportAnimation());
    }
    IEnumerator ForcedTeleportAnimation() 
    {
        teleportAnimating = true;
        teleportSource.volume = 1f;
        teleportRenderer.sortingOrder = teleportSpriteLayer;

        float currentTime = 0f;
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            teleportRenderer.transform.localScale = Mathf.Lerp(0f, 2f, currentTime / 2f) * teleportAuraRadio * Vector3.one;
            teleportSource.pitch = 1f + (currentTime / 4f);
            yield return null;
        }

        teleportRenderer.sortingOrder = teleportSpriteAnimationLayer;
        HidePlayer();
        
        currentTime = 0f;
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            teleportRenderer.transform.localScale = Mathf.Lerp(2f, 0f, currentTime / 2f) * teleportAuraRadio * Vector3.one;
            teleportSource.pitch = 1f + (currentTime / 4f);
            yield return null;
        }
        teleportAnimating = false;
        teleportSource.volume = 0f;
    }
}
