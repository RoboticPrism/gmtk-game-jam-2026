using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : BumpableTile
{
    [SerializeField]
    public Vector3Int gridLocation;

    [SerializeField]
    protected Vector3 visualTargetLocation;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    protected EnemyDefeatEffect defeatEffectPrefab;

    [SerializeField]
    protected AudioClip defeatClip;

    [SerializeField]
    [Tooltip("How fast the enemy visually moves")]
    protected float moveSpeed;


    [SerializeField]
    [Tooltip("How many bumps the enemy takes to kill")]
    protected float maxHealth;

    [SerializeField]
    protected float currentHealth; 

    [SerializeField]
    [Tooltip("How long the enemy takes to do a bump animation")]
    protected float bumpCooldownSeconds;

    [SerializeField]
    [Tooltip("What type of resource does this drop")]
    public ResourceType resourceType;

    [SerializeField]
    [Tooltip("Whats the minimum drop ammount")]
    public int dropMin;

    [SerializeField]
    [Tooltip("Whats the maxmimum drop ammount")]
    public int dropMax;

    public enum EnemyDirections { UP, DOWN, LEFT, RIGHT }

    public EnemyDirections enemyDirection;

    public override void Start()
    {
        base.Start();

        EnemyManager.singleton.AddEnemy(this);
        gridLocation = GridManager.singleton.resourceTilemap.WorldToCell(transform.position);
        visualTargetLocation = gridLocation;

        currentHealth = maxHealth;
    }

    public virtual void Update()
    {
        transform.position = Vector3.Lerp(transform.position, visualTargetLocation, moveSpeed);
    }

    protected void MoveToLocation(Vector3Int gridLocation)
    {
        this.gridLocation = gridLocation;
        visualTargetLocation = gridLocation;
    }

    public virtual void Defeat()
    {
        // Drop resources
        for (int i = 0; i < Random.Range(dropMin + Player.singleton.playerUpgrades.resourceGainIncrease, dropMax + Player.singleton.playerUpgrades.resourceGainIncrease); i++)
        {
            ResourceDrop drop = Instantiate(ResourceDictionary.singleton.resourceDropPrefab, transform.position, Quaternion.identity);
            drop.SetResourceType(resourceType);
        }
        
        // Create effect
        EnemyDefeatEffect effect = Instantiate(defeatEffectPrefab, transform.position, Quaternion.identity);
        effect.Setup(defeatClip);
        Destroy(gameObject);
    }

    public virtual void OnDestroy()
    {
        EnemyManager.singleton.RemoveEnemy(this);
    }

    public virtual void DoTurn()
    {

    }

    public override void OnBump()
    {
        base.OnBump();

        currentHealth -= Player.singleton.playerUpgrades.playerDamage;
        
        if(currentHealth <= 0)
        {
            Defeat(); 
        }
    }

    public void OnShotByTurret(Turret turret)
    {
        currentHealth -= turret.damage; 
        
        if(currentHealth <= 0)
        {
            Defeat(); 
        }
    }

    Coroutine bumpCoroutine;
    protected void Bump(Vector3Int location) {
        if(bumpCoroutine != null)
        {
            StopCoroutine(bumpCoroutine);
        }
        bumpCoroutine = StartCoroutine(BumpAnimation(location));
    }

    IEnumerator BumpAnimation(Vector3Int location)
    {
        visualTargetLocation = Vector3.Lerp(location, gridLocation, 0.5f);
        yield return new WaitForSeconds(bumpCooldownSeconds/2f);
        visualTargetLocation = gridLocation;
        yield return new WaitForSeconds(bumpCooldownSeconds/2f);
    }
}
