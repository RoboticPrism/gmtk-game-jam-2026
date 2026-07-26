using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turret : BumpableTile 
{
    [SerializeField]
    public float damage;

    private int currentCooldown = 0;

    [SerializeField]
    private int currentAmmo;

    [SerializeField]
    private TurretProjectile turretProjectilePrefab;

    [System.Serializable]
    class TurretLevel
    {
        public int level;
        public int maxAmmo;
        public int cooldownBetweenShots;
        public Cost nextLevelCost;
        public Sprite baseSprite;
        public Sprite outOfAmmoSprite;
    }

    [SerializeField]
    private List<TurretLevel> turretLevels;

    [SerializeField]
    private int currentLevel = 1;

    private TurretLevel currentTurretLevel;


    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private UpgradeCostUI upgradeCostUIPrefab;

    private UpgradeCostUI upgradeCostUIInstance;

    [SerializeField]
    private GameObject outOfAmmoAlert;

    [SerializeField]
    private AudioClip buildClip;

    [SerializeField]
    private AudioClip outOfAmmoClip;

    [SerializeField]
    private AudioClip reloadClip;

    [SerializeField]
    [Tooltip("How many turns the shot cooldown can be off by, to prevent all turrets from firing in the same turn")]
    private int shotOffsetRange;

    [SerializeField]
    [Tooltip("How many ammo shots turrets can be off by, to prevent all turrets from needing reloading in the same turn")]
    private int ammoOffsetRange;

    public void Awake()
    {
        TurretManager.singleton.AddTurret(this);
    }

    public override void Start()
    {
        base.Start();
        if (buildClip)
        {
            audioSource.PlayOneShot(buildClip);
        }
        currentTurretLevel = GetCurrentLevel();
        CreateUI();
        // Add some offset so they don't all fire on the same turn
        currentCooldown = currentTurretLevel.cooldownBetweenShots + Random.Range(-shotOffsetRange,shotOffsetRange);
        currentAmmo = currentTurretLevel.maxAmmo + Random.Range(-ammoOffsetRange, ammoOffsetRange);
    }

    public void Update()
    {
        if (upgradeCostUIInstance)
        {
            upgradeCostUIInstance.gameObject.SetActive(!TowerDefenseManager.singleton.isTowerDefenseMode && Pyre.singleton.hasFinishedAnimation);
        }
        outOfAmmoAlert.SetActive(TowerDefenseManager.singleton.isTowerDefenseMode && currentAmmo <= 0);
        SetSprite();
    }

    private void CreateUI()
    {
        upgradeCostUIInstance = Instantiate(upgradeCostUIPrefab, transform.position, Quaternion.identity);
        upgradeCostUIInstance.Setup("Tower\nLvl. "+(currentLevel+1), currentTurretLevel.nextLevelCost);
    }

    private void SetSprite()
    {
        if(currentAmmo > 0)
        {
            spriteRenderer.sprite = currentTurretLevel.baseSprite; 
        }
        else
        {
            spriteRenderer.sprite = currentTurretLevel.outOfAmmoSprite; 
        }
    }

    private TurretLevel GetCurrentLevel()
    {
        return turretLevels.First(level => level.level == currentLevel);    
    }

    public void DoTurn()
    {
        currentCooldown--;
        if(currentCooldown <= 0)
        {
            if (currentAmmo > 0)
            {
                ShootProjectile();
                currentCooldown = currentTurretLevel.cooldownBetweenShots;
                currentAmmo--;

                if(currentAmmo == 0)
                {
                    if(outOfAmmoClip)
                    {
                        audioSource.PlayOneShot(outOfAmmoClip);
                    }
                }
            }
        }
    }

    private void ShootProjectile()
    {
        TowerDefenseGremlin nearestGremlin = null;
        foreach(TowerDefenseGremlin gremlin in TowerDefenseManager.singleton.instantiatedGremlins)
        {
            if(nearestGremlin == null)
            {
                nearestGremlin = gremlin;
            }
            else if (Vector3.Distance(transform.position, nearestGremlin.transform.position) > Vector3.Distance(transform.position, gremlin.transform.position))
            {
                nearestGremlin = gremlin;
            }
        }
        if (nearestGremlin)
        {
            TurretProjectile turretProjectile = Instantiate(turretProjectilePrefab, transform.position, Quaternion.identity);
            turretProjectile.Setup(this, nearestGremlin);
        }
    }

    public void Reload(bool playClip = true)
    {
        // Add some slight variance so all the turrets don't run out on the same turn
        currentAmmo = currentTurretLevel.maxAmmo + Random.Range(-ammoOffsetRange, ammoOffsetRange);

        if (playClip)
        {
            audioSource.PlayOneShot(reloadClip);
        }
    }

    public override void OnBump()
    {
        base.OnBump();
        // in tower defense mode, a bump reloads
        if(TowerDefenseManager.singleton.isTowerDefenseMode)
        {
            Reload();
        } 
        else
        {
            if (currentLevel < 3 && currentTurretLevel.nextLevelCost.canPlayerAfford())
            {
                currentTurretLevel.nextLevelCost.payCost();
                currentLevel++;
                currentTurretLevel = GetCurrentLevel();

                audioSource.PlayOneShot(buildClip);

                // Recreate cost ui for next level
                Destroy(upgradeCostUIInstance.gameObject);

                if(currentLevel < 3)
                {
                    CreateUI();
                }
            }
        }
    }

    
}
