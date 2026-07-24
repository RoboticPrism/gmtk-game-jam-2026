using UnityEngine;

public class Turret : BumpableTile 
{
    [SerializeField]
    public float damage;

    [SerializeField]
    public int cooldownBetweenShots;

    private int currentCooldown = 0;

    [SerializeField]
    private int maxAmmo;

    [SerializeField]
    private int currentAmmo;

    [SerializeField]
    private TurretProjectile turretProjectilePrefab;

    [SerializeField]
    private Cost upgradeCost;

    [SerializeField]
    private UpgradeCostUI upgradeCostUIPrefab;

    private UpgradeCostUI upgradeCostUIInstance;

    public void Awake()
    {
        TurretManager.singleton.AddTurret(this);
    }

    public override void Start()
    {
        base.Start();
        currentCooldown = cooldownBetweenShots;
        currentAmmo = maxAmmo;
        CreateUI();
    }

    public void Update()
    {
        upgradeCostUIInstance.gameObject.SetActive(!TowerDefenseManager.singleton.isTowerDefenseMode);
    }

    private void CreateUI()
    {
        upgradeCostUIInstance = Instantiate(upgradeCostUIPrefab, transform.position, Quaternion.identity);
        upgradeCostUIInstance.Setup("Tower Lvl. 2", upgradeCost);
    }

    public void DoTurn()
    {
        currentCooldown--;
        if(currentCooldown <= 0)
        {
            if (currentAmmo > 0)
            {
                ShootProjectile();
                currentCooldown = cooldownBetweenShots;
                currentAmmo--;
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

    public override void OnBump()
    {
        base.OnBump();
        // in tower defense mode, a bump reloads
        if(TowerDefenseManager.singleton.isTowerDefenseMode)
        {
            currentAmmo = maxAmmo;
        } 
        else
        {
            //upgrade logic
        }
    }
}
