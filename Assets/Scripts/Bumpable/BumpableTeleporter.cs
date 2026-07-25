using System.Collections.Generic;
using UnityEngine;

public class BumpableTeleporter : BumpableTile
{
    [SerializeField]
    public BumpableTeleporter otherTeleporter;

    [SerializeField]
    public bool isEnabled;

    [SerializeField]
    private AudioClip teleportClip;

    public Vector3Int gridLocation;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite offSprite;

    [SerializeField]
    private List<Sprite> onSprites;

    [SerializeField]
    private float animateSpeedSeconds;

    private int animateIndex = 0; 

    public override void Start()
    {
        base.Start();
        gridLocation = GridManager.singleton.resourceTilemap.WorldToCell(transform.position);
        transform.position = gridLocation;
        if (!otherTeleporter)
        {
            Debug.LogError("Teleporter doesn't have link!");
        }
    }


    float currentTime = 0f;
    public void Update()
    {
        if(isEnabled)
        {
            currentTime += Time.deltaTime;
            if(currentTime >= animateSpeedSeconds)
            {
                currentTime = 0;
                animateIndex++;
                if(animateIndex >= onSprites.Count)
                {
                    animateIndex = 0;
                }
            }
            spriteRenderer.sprite = onSprites[animateIndex];
        }
        else
        {
            spriteRenderer.sprite = offSprite;
        }
    }

    public override void OnBump()
    {
        base.OnBump();
        if(isEnabled)
        {
            if(otherTeleporter)
            {
                // Teleport
                Player.singleton.playerMovement.UseTeleporter(this);

                // Unlock linked teleporter
                otherTeleporter.isEnabled = true;
            }
            else
            {
                Debug.LogError("Tried to teleport to unlinked teleport!");
            }
            if (teleportClip)
            {
                audioSource.PlayOneShot(teleportClip);
            }
        }
    }

}
